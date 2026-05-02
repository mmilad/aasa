using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace ArkServerManager.Admin.Services;

public sealed class WorkerApiClient(HttpClient httpClient)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    private readonly HttpClient _httpClient = httpClient;

    public Task<ApiClientResponse<ServerListResponse>> ListServersAsync(CancellationToken cancellationToken)
    {
        return SendAsync<ServerListResponse>(HttpMethod.Get, "api/v1/servers", payload: null, cancellationToken);
    }

    public Task<ApiClientResponse<ServerResponse>> GetServerAsync(Guid serverId, CancellationToken cancellationToken)
    {
        return SendAsync<ServerResponse>(HttpMethod.Get, $"api/v1/servers/{serverId:D}", payload: null, cancellationToken);
    }

    public Task<ApiClientResponse<JobAcceptedResponse>> EnqueueBackupAsync(Guid serverId, CancellationToken cancellationToken)
    {
        return SendAsync<JobAcceptedResponse>(HttpMethod.Post, $"api/v1/servers/{serverId:D}/jobs/backup", payload: null, cancellationToken);
    }

    public Task<ApiClientResponse<JobResponse>> GetJobAsync(Guid jobId, CancellationToken cancellationToken)
    {
        return SendAsync<JobResponse>(HttpMethod.Get, $"api/v1/jobs/{jobId:D}", payload: null, cancellationToken);
    }

    private async Task<ApiClientResponse<T>> SendAsync<T>(
        HttpMethod method,
        string relativePath,
        object? payload,
        CancellationToken cancellationToken)
    {
        try
        {
            using var request = new HttpRequestMessage(method, relativePath);
            if (payload is not null)
            {
                request.Content = JsonContent.Create(payload, options: JsonOptions);
            }

            using var response = await _httpClient.SendAsync(request, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                var model = await response.Content.ReadFromJsonAsync<T>(JsonOptions, cancellationToken);
                return new ApiClientResponse<T>(response.StatusCode, model, error: null);
            }

            var apiError = await ParseErrorAsync(response, cancellationToken);
            return new ApiClientResponse<T>(response.StatusCode, Value: default, Error: apiError);
        }
        catch (HttpRequestException exception)
        {
            return new ApiClientResponse<T>(
                HttpStatusCode.ServiceUnavailable,
                Value: default,
                Error: new ApiError("WORKER_UNREACHABLE", $"Failed to reach Worker API: {exception.Message}"));
        }
        catch (TaskCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            return new ApiClientResponse<T>(
                HttpStatusCode.RequestTimeout,
                Value: default,
                Error: new ApiError("WORKER_TIMEOUT", "Timed out waiting for Worker API response."));
        }
    }

    private static async Task<ApiError> ParseErrorAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        try
        {
            var envelope = await response.Content.ReadFromJsonAsync<ErrorEnvelope>(JsonOptions, cancellationToken);
            if (envelope?.Error is not null)
            {
                return envelope.Error;
            }
        }
        catch (NotSupportedException)
        {
        }
        catch (JsonException)
        {
        }

        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        var defaultMessage = string.IsNullOrWhiteSpace(body)
            ? $"Worker API request failed with status {(int)response.StatusCode}."
            : body;
        return new ApiError(response.StatusCode.ToString().ToUpperInvariant(), defaultMessage);
    }
}
