using ArkServerManager.Worker.Data;
using ArkServerManager.Worker.Data.Entities;
using ArkServerManager.Worker.Data.Repositories;
using ArkServerManager.Worker.Domain;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ArkServerManager.Worker.Tests.Data;

public sealed class RepositoryTests
{
    [Fact]
    public async Task ServerRepository_AddAndGet_PersistsEntity()
    {
        await using var fixture = await SqliteFixture.CreateAsync();
        var repo = new ServerRepository(fixture.DbContext);

        var entity = new ServerEntity
        {
            Id = Guid.NewGuid(),
            Name = "Test Server",
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow,
            State = ServerState.Stopped,
            InstallRoot = "/tmp/servers/test/binaries",
            GamePort = 7777,
            QueryPort = 27015,
            RconPort = 27020,
            RconPassword = "placeholder",
            MapName = "TheIsland_WP",
            SessionName = "Session A",
        };

        await repo.AddAsync(entity);
        await repo.SaveChangesAsync();

        var fetched = await repo.GetAsync(entity.Id);
        Assert.NotNull(fetched);
        Assert.Equal(entity.Name, fetched!.Name);
        Assert.Equal(entity.InstallRoot, fetched.InstallRoot);
        Assert.Equal(ServerState.Stopped, fetched.State);
    }

    [Fact]
    public async Task ServerRepository_Delete_RemovesEntity()
    {
        await using var fixture = await SqliteFixture.CreateAsync();
        var repo = new ServerRepository(fixture.DbContext);

        var server = new ServerEntity
        {
            Id = Guid.NewGuid(),
            Name = "Delete Me",
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow,
            State = ServerState.Stopped,
            InstallRoot = "/tmp/servers/delete/binaries",
            GamePort = 7778,
            QueryPort = 27016,
            RconPort = 27021,
            RconPassword = "placeholder",
            MapName = "TheIsland_WP",
            SessionName = "Delete Session",
        };

        await repo.AddAsync(server);
        await repo.SaveChangesAsync();

        var deleted = await repo.DeleteAsync(server.Id);
        await repo.SaveChangesAsync();

        var fetched = await repo.GetAsync(server.Id);
        Assert.True(deleted);
        Assert.Null(fetched);
    }

    [Fact]
    public async Task JobRepository_RunningChecks_RespectTypeAndServer()
    {
        await using var fixture = await SqliteFixture.CreateAsync();
        var serverRepo = new ServerRepository(fixture.DbContext);
        var jobRepo = new JobRepository(fixture.DbContext);

        var server = new ServerEntity
        {
            Id = Guid.NewGuid(),
            Name = "Job Server",
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow,
            State = ServerState.Stopped,
            InstallRoot = "/tmp/servers/jobs/binaries",
            GamePort = 7779,
            QueryPort = 27017,
            RconPort = 27022,
            RconPassword = "placeholder",
            MapName = "TheIsland_WP",
            SessionName = "Jobs Session",
        };

        await serverRepo.AddAsync(server);
        await serverRepo.SaveChangesAsync();

        var runningInstall = new JobEntity
        {
            Id = Guid.NewGuid(),
            ServerId = server.Id,
            Type = JobType.InstallOrUpdate,
            Status = JobStatus.Running,
            CreatedAtUtc = DateTime.UtcNow,
            StartedAtUtc = DateTime.UtcNow,
        };

        await jobRepo.AddAsync(runningInstall);
        await jobRepo.SaveChangesAsync();

        Assert.True(await jobRepo.HasRunningJobForServerAsync(server.Id));
        Assert.True(await jobRepo.HasRunningInstallAsync());

        runningInstall.Status = JobStatus.Done;
        await jobRepo.SaveChangesAsync();

        Assert.False(await jobRepo.HasRunningInstallAsync());
    }

    [Fact]
    public async Task JobRepository_GetTrackedAsync_AllowsStatusUpdate()
    {
        await using var fixture = await SqliteFixture.CreateAsync();
        var serverRepo = new ServerRepository(fixture.DbContext);
        var jobRepo = new JobRepository(fixture.DbContext);

        var server = new ServerEntity
        {
            Id = Guid.NewGuid(),
            Name = "Tracked Job Server",
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow,
            State = ServerState.Stopped,
            InstallRoot = "/tmp/servers/tracked/binaries",
            GamePort = 7780,
            QueryPort = 27018,
            RconPort = 27023,
            RconPassword = "placeholder",
            MapName = "TheIsland_WP",
            SessionName = "Tracked Session",
        };

        await serverRepo.AddAsync(server);
        await serverRepo.SaveChangesAsync();

        var job = new JobEntity
        {
            Id = Guid.NewGuid(),
            ServerId = server.Id,
            Type = JobType.Start,
            Status = JobStatus.Running,
            CreatedAtUtc = DateTime.UtcNow,
            StartedAtUtc = DateTime.UtcNow,
        };

        await jobRepo.AddAsync(job);
        await jobRepo.SaveChangesAsync();

        var tracked = await jobRepo.GetTrackedAsync(job.Id);
        Assert.NotNull(tracked);
        tracked!.Status = JobStatus.Done;
        tracked.FinishedAtUtc = DateTime.UtcNow;
        await jobRepo.SaveChangesAsync();

        var readOnly = await jobRepo.GetAsync(job.Id);
        Assert.NotNull(readOnly);
        Assert.Equal(JobStatus.Done, readOnly!.Status);
        Assert.NotNull(readOnly.FinishedAtUtc);
    }

    private sealed class SqliteFixture : IAsyncDisposable
    {
        private readonly SqliteConnection _connection;

        private SqliteFixture(SqliteConnection connection, AppDbContext dbContext)
        {
            _connection = connection;
            DbContext = dbContext;
        }

        public AppDbContext DbContext { get; }

        public static async Task<SqliteFixture> CreateAsync()
        {
            var connection = new SqliteConnection("Data Source=:memory:");
            await connection.OpenAsync();
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(connection)
                .Options;

            var context = new AppDbContext(options);
            await context.Database.EnsureCreatedAsync();
            return new SqliteFixture(connection, context);
        }

        public async ValueTask DisposeAsync()
        {
            await DbContext.DisposeAsync();
            await _connection.DisposeAsync();
        }
    }
}
