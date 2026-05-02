using ArkServerManager.Worker.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ArkServerManager.Worker.Infrastructure;

public sealed class JobProcessorHostedService(
    ChannelJobWorkQueue workQueue,
    IServiceScopeFactory scopeFactory,
    ILogger<JobProcessorHostedService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var jobId in workQueue.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                await using var scope = scopeFactory.CreateAsyncScope();
                var app = scope.ServiceProvider.GetRequiredService<IServerApplicationService>();
                await app.ProcessQueuedJobAsync(jobId, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Unhandled failure while processing job {JobId}", jobId);
            }
        }
    }
}
