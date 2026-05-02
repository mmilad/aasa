using ArkServerManager.Admin.Configuration;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;

namespace ArkServerManager.Admin.Services;

public sealed class WorkerApiKeyHeaderHandler(IOptions<WorkerApiOptions> options) : DelegatingHandler
{
    private readonly WorkerApiOptions _options = options.Value;

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.Remove("X-Api-Key");
        request.Headers.TryAddWithoutValidation("X-Api-Key", _options.ApiKey);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        return base.SendAsync(request, cancellationToken);
    }
}
