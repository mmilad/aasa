using ArkServerManager.Worker.Configuration;
using ArkServerManager.Worker.Contracts;
using ArkServerManager.Worker.Data.Entities;
using ArkServerManager.Worker.Data.Repositories;
using ArkServerManager.Worker.Domain;
using ArkServerManager.Worker.Infrastructure;
using ArkServerManager.Worker.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace ArkServerManager.Worker.Tests.Services;

public sealed class RuntimeIntegrationTests
{
    [Fact]
    public void SteamCmd_BuildInstallArguments_MatchesNormativeCommand()
    {
        var installRoot = @"C:\Ark\servers\abc\binaries\";

        var args = SteamCmdClient.BuildInstallArguments(installRoot);

        Assert.Equal(
            "+force_install_dir \"C:\\Ark\\servers\\abc\\binaries\" +login anonymous +app_update 2430930 validate +quit",
            args);
    }

    [Fact]
    public void ProcessSupervisor_BuildPaths_ComposeFromInstallRoot()
    {
        var installRoot = @"C:\Ark\servers\abc\binaries";

        var exePath = ProcessSupervisor.BuildExecutablePath(installRoot);
        var cwd = ProcessSupervisor.BuildWorkingDirectory(installRoot);

        Assert.Equal(
            @"C:\Ark\servers\abc\binaries\ShooterGame\Binaries\Win64\ArkAscendedServer.exe",
            exePath);
        Assert.Equal(
            @"C:\Ark\servers\abc\binaries\ShooterGame\Binaries\Win64",
            cwd);
    }

    [Fact]
    public void ProcessSupervisor_BuildLaunchArguments_UsesRunbookTemplate()
    {
        var args = ProcessSupervisor.BuildLaunchArguments(
            mapName: "TheIsland_WP",
            sessionName: "My Session",
            gamePort: 7777,
            queryPort: 27015,
            rconPort: 27020,
            rconPassword: "secret");

        Assert.Equal(
            "TheIsland_WP?listen?SessionName=\"My Session\"?Port=7777?QueryPort=27015?RCONPort=27020?ServerPassword=\"\"?ServerAdminPassword=\"secret\"",
            args);
    }

    [Fact]
    public async Task SteamCmd_InstallOrUpdateAsync_ReturnsConflict_WhenServerIsRunning()
    {
        var serverId = Guid.NewGuid();
        var server = new ServerEntity
        {
            Id = serverId,
            Name = "Runtime Test",
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow,
            State = ServerState.Running,
            InstallRoot = @"C:\Ark\servers\abc\binaries",
            GamePort = 7777,
            QueryPort = 27015,
            RconPort = 27020,
            RconPassword = "placeholder",
            MapName = "TheIsland_WP",
            SessionName = "Runtime Session",
            Pid = 1234,
        };

        var repository = new InMemoryServerRepository(server);
        var options = Options.Create(new SteamCmdOptions
        {
            SteamCmdPath = "steamcmd.exe",
            InstallTimeoutSeconds = 60,
        });

        var sut = new SteamCmdClient(repository, options, NullLogger<SteamCmdClient>.Instance);

        var result = await sut.InstallOrUpdateAsync(serverId);

        Assert.Equal(409, result.ExitCode);
        Assert.Contains("SERVER_NOT_STOPPED", result.Output);
    }

    [Fact]
    public async Task EnqueueInstallJob_ReturnsServerNotStoppedConflict_WhenServerIsRunning()
    {
        var serverId = Guid.NewGuid();
        var server = new ServerEntity
        {
            Id = serverId,
            Name = "Runtime Test",
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow,
            State = ServerState.Running,
            InstallRoot = @"C:\Ark\servers\abc\binaries",
            GamePort = 7777,
            QueryPort = 27015,
            RconPort = 27020,
            RconPassword = "placeholder",
            MapName = "TheIsland_WP",
            SessionName = "Runtime Session",
            Pid = 1234,
        };

        var serverRepository = new InMemoryServerRepository(server);
        var jobRepository = new InMemoryJobRepository();
        var service = new ServerApplicationService(
            serverRepository,
            jobRepository,
            new NoOpDataDirectoryService(),
            new AlwaysAvailablePortService(),
            new NoOpSteamCmdClient(),
            new NoOpProcessSupervisor());

        var result = await service.EnqueueJobAsync(serverId, JobType.InstallOrUpdate);

        Assert.Null(result.Job);
        Assert.NotNull(result.Error);
        Assert.Equal("SERVER_NOT_STOPPED", result.Error!.Code);
        Assert.Empty(jobRepository.Jobs);
    }

    private sealed class InMemoryServerRepository(ServerEntity? server) : IServerRepository
    {
        public Task<IReadOnlyList<ServerEntity>> ListAsync(CancellationToken cancellationToken = default)
        {
            IReadOnlyList<ServerEntity> result = server is null ? [] : [server];
            return Task.FromResult(result);
        }

        public Task<ServerEntity?> GetAsync(Guid serverId, CancellationToken cancellationToken = default)
        {
            if (server?.Id == serverId)
            {
                return Task.FromResult<ServerEntity?>(server);
            }

            return Task.FromResult<ServerEntity?>(null);
        }

        public Task<ServerEntity> AddAsync(ServerEntity entity, CancellationToken cancellationToken = default)
        {
            server = entity;
            return Task.FromResult(entity);
        }

        public Task<bool> DeleteAsync(Guid serverId, CancellationToken cancellationToken = default)
        {
            if (server?.Id != serverId)
            {
                return Task.FromResult(false);
            }

            server = null;
            return Task.FromResult(true);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }

    private sealed class InMemoryJobRepository : IJobRepository
    {
        private readonly List<JobEntity> _jobs = [];

        public IReadOnlyList<JobEntity> Jobs => _jobs;

        public Task<JobEntity?> GetAsync(Guid jobId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_jobs.FirstOrDefault(x => x.Id == jobId));
        }

        public Task<JobEntity> AddAsync(JobEntity entity, CancellationToken cancellationToken = default)
        {
            _jobs.Add(entity);
            return Task.FromResult(entity);
        }

        public Task<bool> HasRunningJobForServerAsync(Guid serverId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_jobs.Any(x => x.ServerId == serverId && x.Status == JobStatus.Running));
        }

        public Task<bool> HasRunningInstallAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_jobs.Any(x => x.Type == JobType.InstallOrUpdate && x.Status == JobStatus.Running));
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }

    private sealed class NoOpDataDirectoryService : IDataDirectoryService
    {
        public string ManagerDataRoot => @"C:\Ark";

        public string GetServerRoot(Guid serverId) => Path.Combine(ManagerDataRoot, "servers", serverId.ToString("D"));

        public string GetServerBinariesRoot(Guid serverId) => Path.Combine(GetServerRoot(serverId), "binaries");

        public Task EnsureServerTreeAsync(Guid serverId, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }

    private sealed class AlwaysAvailablePortService : IPortAvailabilityService
    {
        public bool IsAvailable(int port) => true;
    }

    private sealed class NoOpSteamCmdClient : ISteamCmdClient
    {
        public Task<SteamCmdResult> InstallOrUpdateAsync(Guid serverId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new SteamCmdResult(0, "ok"));
        }
    }

    private sealed class NoOpProcessSupervisor : IProcessSupervisor
    {
        public Task<ProcessExecutionResult> StartAsync(Guid serverId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new ProcessExecutionResult(0, "ok"));
        }

        public Task<ProcessExecutionResult> StopAsync(Guid serverId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new ProcessExecutionResult(0, "ok"));
        }

        public Task<ProcessExecutionResult> RestartAsync(Guid serverId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new ProcessExecutionResult(0, "ok"));
        }
    }
}
