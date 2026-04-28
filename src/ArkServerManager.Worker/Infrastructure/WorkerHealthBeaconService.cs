using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ArkServerManager.Worker.Infrastructure;

public sealed class WorkerHealthBeaconService(ILogger<WorkerHealthBeaconService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogDebug("Worker heartbeat at {TimestampUtc}", DateTime.UtcNow);
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}
