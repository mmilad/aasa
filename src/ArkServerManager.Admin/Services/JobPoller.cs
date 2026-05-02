using System.Net;

namespace ArkServerManager.Admin.Services;

public static class JobPoller
{
    public static async Task<ApiClientResponse<JobResponse>> PollToTerminalAsync(
        WorkerApiClient client,
        Guid jobId,
        TimeSpan pollInterval,
        TimeSpan timeout,
        Func<JobResponse, Task>? onProgress,
        CancellationToken cancellationToken)
    {
        var started = DateTimeOffset.UtcNow;

        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var current = await client.GetJobAsync(jobId, cancellationToken);
            if (!current.IsSuccess || current.Value is null)
            {
                return current;
            }

            if (onProgress is not null)
            {
                await onProgress(current.Value);
            }

            if (JobStatusNames.IsTerminal(current.Value.Status))
            {
                return current;
            }

            if (DateTimeOffset.UtcNow - started > timeout)
            {
                return new ApiClientResponse<JobResponse>(
                    HttpStatusCode.RequestTimeout,
                    current.Value,
                    new ApiError("TIMEOUT", "Timed out waiting for job to reach terminal state."));
            }

            await Task.Delay(pollInterval, cancellationToken);
        }
    }
}
