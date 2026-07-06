using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using ArkServerManager.Worker.Configuration;
using ArkServerManager.Worker.Contracts;
using ArkServerManager.Worker.Data.Repositories;
using ArkServerManager.Worker.Domain;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ArkServerManager.Worker.Services;

public sealed class ProcessSupervisor(
    IServerRepository serverRepository,
    IOptions<ProcessSupervisorOptions> options,
    ILogger<ProcessSupervisor> logger) : IProcessSupervisor
{
    private const int ConflictExitCode = 409;
    private const int NotFoundExitCode = 2;
    private const string ShutdownCommand = "DoExit";

    public async Task<ProcessExecutionResult> StartAsync(Guid serverId, CancellationToken cancellationToken = default)
    {
        var server = await serverRepository.GetAsync(serverId, cancellationToken);
        if (server is null)
        {
            return new ProcessExecutionResult(1, $"Server {serverId:D} was not found.");
        }

        if (server.Pid is int pid && TryGetProcessById(pid, out _))
        {
            return new ProcessExecutionResult(ConflictExitCode, $"Server is already running with PID {pid}.");
        }

        var executablePath = BuildExecutablePath(server.InstallRoot);
        var workingDirectory = BuildWorkingDirectory(server.InstallRoot);
        if (!File.Exists(executablePath))
        {
            return new ProcessExecutionResult(
                NotFoundExitCode,
                $"Server executable was not found at '{executablePath}'. Hint: run install/update first.");
        }

        var startInfo = BuildStartInfo(executablePath, workingDirectory, server);
        try
        {
            using var process = new Process { StartInfo = startInfo };
            if (!process.Start())
            {
                return new ProcessExecutionResult(1, "Failed to start ArkAscendedServer process.");
            }

            server.Pid = process.Id;
            server.State = ServerState.Running;
            server.UpdatedAtUtc = DateTime.UtcNow;
            await serverRepository.SaveChangesAsync(cancellationToken);

            return new ProcessExecutionResult(
                0,
                $"Started ArkAscendedServer (PID {process.Id}) using '{startInfo.FileName}'.");
        }
        catch (Win32Exception exception) when (exception.NativeErrorCode is 2 or 3)
        {
            return new ProcessExecutionResult(
                NotFoundExitCode,
                $"Server executable path is invalid. Hint: verify install root '{server.InstallRoot}'.");
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to start server process for {ServerId}", serverId);
            return new ProcessExecutionResult(1, $"Failed to start process. Hint: {exception.GetType().Name}.");
        }
    }

    public async Task<ProcessExecutionResult> StopAsync(Guid serverId, CancellationToken cancellationToken = default)
    {
        var server = await serverRepository.GetAsync(serverId, cancellationToken);
        if (server is null)
        {
            return new ProcessExecutionResult(1, $"Server {serverId:D} was not found.");
        }

        if (server.Pid is null)
        {
            server.State = ServerState.Stopped;
            server.UpdatedAtUtc = DateTime.UtcNow;
            await serverRepository.SaveChangesAsync(cancellationToken);
            return new ProcessExecutionResult(0, "Server is already stopped (no PID set).");
        }

        if (!TryGetProcessById(server.Pid.Value, out var process))
        {
            server.Pid = null;
            server.State = ServerState.Stopped;
            server.UpdatedAtUtc = DateTime.UtcNow;
            await serverRepository.SaveChangesAsync(cancellationToken);
            return new ProcessExecutionResult(0, "Server process was not found; state reset to Stopped.");
        }

        ArgumentNullException.ThrowIfNull(process);

        var output = new StringBuilder();
        server.State = ServerState.Stopping;
        server.UpdatedAtUtc = DateTime.UtcNow;
        await serverRepository.SaveChangesAsync(cancellationToken);

        var gracefulAttempted = false;
        try
        {
            gracefulAttempted = await TryGracefulStopAsync(server, output, cancellationToken);
        }
        catch (Exception exception)
        {
            // A single RCON failure must not be treated as a crash if process is still running.
            output.AppendLine($"Graceful stop failed ({exception.GetType().Name}); continuing with process checks.");
            logger.LogWarning(exception, "Graceful stop attempt failed for {ServerId}", serverId);
        }

        var shutdownGrace = TimeSpan.FromSeconds(Math.Max(1, options.Value.ShutdownGracePeriodSeconds));
        var exitedAfterGrace = await WaitForExitAsync(process, shutdownGrace, cancellationToken);
        if (!exitedAfterGrace)
        {
            output.AppendLine(
                gracefulAttempted
                    ? $"Process still alive after graceful stop wait ({shutdownGrace.TotalSeconds:F0}s). Killing process tree."
                    : $"No graceful stop confirmation after {shutdownGrace.TotalSeconds:F0}s. Killing process tree.");

            try
            {
                process.Kill(entireProcessTree: true);
            }
            catch (InvalidOperationException)
            {
                // Process exited in the meantime.
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Failed to kill process tree for {ServerId}", serverId);
                return new ProcessExecutionResult(1, $"Failed to kill process tree. Hint: {exception.GetType().Name}.");
            }

            var killGrace = TimeSpan.FromSeconds(Math.Max(1, options.Value.KillGracePeriodSeconds));
            var exitedAfterKill = await WaitForExitAsync(process, killGrace, cancellationToken);
            if (!exitedAfterKill && !process.HasExited)
            {
                return new ProcessExecutionResult(1, "Process remained alive after kill attempt.");
            }
        }

        server.Pid = null;
        server.State = ServerState.Stopped;
        server.UpdatedAtUtc = DateTime.UtcNow;
        await serverRepository.SaveChangesAsync(cancellationToken);

        output.AppendLine("Server stopped.");
        return new ProcessExecutionResult(0, output.ToString().TrimEnd());
    }

    public async Task<ProcessExecutionResult> RestartAsync(Guid serverId, CancellationToken cancellationToken = default)
    {
        var stop = await StopAsync(serverId, cancellationToken);
        if (stop.ExitCode != 0)
        {
            return stop;
        }

        return await StartAsync(serverId, cancellationToken);
    }

    public static string BuildExecutablePath(string installRoot)
    {
        return JoinWindowsGameLayoutPath(
            installRoot,
            "ShooterGame",
            "Binaries",
            "Win64",
            "ArkAscendedServer.exe");
    }

    public static string BuildWorkingDirectory(string installRoot)
    {
        return JoinWindowsGameLayoutPath(installRoot, "ShooterGame", "Binaries", "Win64");
    }

    /// <summary>
    /// Joins install root and relative segments using backslashes so composed paths match Windows
    /// layout regardless of host OS (Linux CI runs the same assertions as Windows).
    /// </summary>
    private static string JoinWindowsGameLayoutPath(string installRoot, params string[] segments)
    {
        var root = installRoot.TrimEnd('\\', '/');
        var parts = new string[segments.Length + 1];
        parts[0] = root;
        Array.Copy(segments, 0, parts, 1, segments.Length);
        return string.Join('\\', parts);
    }

    public static string BuildLaunchArguments(
        string mapName,
        string sessionName,
        int gamePort,
        int queryPort,
        int rconPort,
        string rconPassword)
    {
        var escapedSession = EscapeQuotedValue(sessionName);
        var escapedRconPassword = EscapeQuotedValue(rconPassword);
        return
            $"{mapName}?listen?SessionName=\"{escapedSession}\"?Port={gamePort}?QueryPort={queryPort}?RCONPort={rconPort}?ServerPassword=\"\"?ServerAdminPassword=\"{escapedRconPassword}\"";
    }

    private static ProcessStartInfo BuildStartInfo(string executablePath, string workingDirectory, Data.Entities.ServerEntity server)
    {
        return new ProcessStartInfo
        {
            FileName = executablePath,
            WorkingDirectory = workingDirectory,
            Arguments = BuildLaunchArguments(
                server.MapName,
                server.SessionName,
                server.GamePort,
                server.QueryPort,
                server.RconPort,
                server.RconPassword),
            UseShellExecute = false,
            CreateNoWindow = true,
        };
    }

    private static bool TryGetProcessById(int pid, out Process? process)
    {
        try
        {
            process = Process.GetProcessById(pid);
            if (process.HasExited)
            {
                process.Dispose();
                process = null;
                return false;
            }

            return true;
        }
        catch (ArgumentException)
        {
            process = null;
            return false;
        }
    }

    private static async Task<bool> WaitForExitAsync(Process process, TimeSpan timeout, CancellationToken cancellationToken)
    {
        using var timeoutCts = new CancellationTokenSource(timeout);
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

        try
        {
            await process.WaitForExitAsync(linkedCts.Token);
            return true;
        }
        catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested && !cancellationToken.IsCancellationRequested)
        {
            return process.HasExited;
        }
    }

    private static Task<bool> TryGracefulStopAsync(
        Data.Entities.ServerEntity server,
        StringBuilder output,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(server.RconPassword))
        {
            output.AppendLine("Skipping graceful stop because no RCON password is configured.");
            return Task.FromResult(false);
        }

        output.AppendLine($"Attempting graceful shutdown via RCON command '{ShutdownCommand}' on port {server.RconPort}.");
        // TODO(WIN-SMOKE): Replace placeholder with real RCON client command execution on Windows host.
        return Task.FromResult(false);
    }

    private static string EscapeQuotedValue(string value)
    {
        return value.Replace("\"", "\\\"");
    }
}
