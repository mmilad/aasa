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

public sealed class SteamCmdClient(
    IServerRepository serverRepository,
    IOptions<SteamCmdOptions> steamCmdOptions,
    ILogger<SteamCmdClient> logger) : ISteamCmdClient
{
    private const int SteamAppId = 2430930;
    private const int SteamCmdMissingExitCode = 127;
    private const int ConflictExitCode = 409;
    private const int TimeoutExitCode = 124;

    public async Task<SteamCmdResult> InstallOrUpdateAsync(Guid serverId, CancellationToken cancellationToken = default)
    {
        var server = await serverRepository.GetAsync(serverId, cancellationToken);
        if (server is null)
        {
            return new SteamCmdResult(1, $"Server {serverId:D} was not found.");
        }

        if (server.State != ServerState.Stopped)
        {
            return new SteamCmdResult(
                ConflictExitCode,
                "Conflict: install/update requires server to be stopped (SERVER_NOT_STOPPED).");
        }

        var startInfo = BuildStartInfo(steamCmdOptions.Value.SteamCmdPath, server.InstallRoot);
        var output = new StringBuilder();
        var outputLock = new object();

        void AppendLine(string channel, string line)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                return;
            }

            lock (outputLock)
            {
                output.Append('[').Append(channel).Append("] ").AppendLine(line);
            }
        }

        try
        {
            using var process = new Process
            {
                StartInfo = startInfo,
                EnableRaisingEvents = true,
            };

            process.OutputDataReceived += (_, eventArgs) =>
            {
                if (eventArgs.Data is not null)
                {
                    AppendLine("stdout", eventArgs.Data);
                }
            };

            process.ErrorDataReceived += (_, eventArgs) =>
            {
                if (eventArgs.Data is not null)
                {
                    AppendLine("stderr", eventArgs.Data);
                }
            };

            if (!TryStartProcess(process, out var startFailure))
            {
                return startFailure!;
            }

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            var timeout = TimeSpan.FromSeconds(Math.Max(1, steamCmdOptions.Value.InstallTimeoutSeconds));
            var completed = await WaitForExitWithTimeoutAsync(process, timeout, cancellationToken);
            if (!completed)
            {
                AppendLine("stderr", $"SteamCMD timed out after {timeout.TotalSeconds:F0} seconds.");
                TryKillProcessTree(process);
                return new SteamCmdResult(
                    TimeoutExitCode,
                    BuildOutput(output, "SteamCMD install timed out. Hint: increase SteamCmd:InstallTimeoutSeconds."));
            }

            process.WaitForExit();
            var exitCode = process.ExitCode;
            if (exitCode != 0)
            {
                AppendLine("stderr", $"SteamCMD exited with code {exitCode}. Hint: install failed.");
            }
            else
            {
                AppendLine("stdout", "SteamCMD install/update completed successfully.");
            }

            return new SteamCmdResult(exitCode, BuildOutput(output));
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "SteamCMD install/update failed for server {ServerId}", serverId);
            return new SteamCmdResult(
                1,
                BuildOutput(output, $"SteamCMD execution failed. Hint: {exception.GetType().Name}."));
        }
    }

    internal static ProcessStartInfo BuildStartInfo(string steamCmdPath, string installRoot)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = steamCmdPath,
            Arguments = BuildInstallArguments(installRoot),
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        var workingDirectory = Path.GetDirectoryName(steamCmdPath);
        if (!string.IsNullOrWhiteSpace(workingDirectory))
        {
            startInfo.WorkingDirectory = workingDirectory;
        }

        return startInfo;
    }

    public static string BuildInstallArguments(string installRoot)
    {
        var normalizedRoot = NormalizeInstallRoot(installRoot);
        return $"+force_install_dir \"{normalizedRoot}\" +login anonymous +app_update {SteamAppId} validate +quit";
    }

    private static string NormalizeInstallRoot(string installRoot)
    {
        return installRoot.TrimEnd('\\', '/');
    }

    private static bool TryStartProcess(Process process, out SteamCmdResult? startFailure)
    {
        try
        {
            if (!process.Start())
            {
                startFailure = new SteamCmdResult(
                    1,
                    "SteamCMD failed to start. Hint: verify SteamCmd:SteamCmdPath points to steamcmd.exe.");
                return false;
            }
        }
        catch (Win32Exception exception) when (exception.NativeErrorCode is 2 or 3)
        {
            startFailure = new SteamCmdResult(
                SteamCmdMissingExitCode,
                "SteamCMD executable was not found. Hint: set SteamCmd:SteamCmdPath to steamcmd.exe.");
            return false;
        }
        catch (FileNotFoundException)
        {
            startFailure = new SteamCmdResult(
                SteamCmdMissingExitCode,
                "SteamCMD executable was not found. Hint: set SteamCmd:SteamCmdPath to steamcmd.exe.");
            return false;
        }

        startFailure = null;
        return true;
    }

    private static async Task<bool> WaitForExitWithTimeoutAsync(
        Process process,
        TimeSpan timeout,
        CancellationToken cancellationToken)
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

    private static void TryKillProcessTree(Process process)
    {
        try
        {
            if (!process.HasExited)
            {
                process.Kill(entireProcessTree: true);
            }
        }
        catch (InvalidOperationException)
        {
            // Process already exited.
        }
    }

    private static string BuildOutput(StringBuilder output, string? hint = null)
    {
        if (!string.IsNullOrWhiteSpace(hint))
        {
            output.AppendLine(hint);
        }

        return output.ToString().TrimEnd();
    }
}
