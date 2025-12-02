namespace WorldDomination.SimpleObservability.Dashboard.Services;

/// <summary>
/// Default implementation of IHealthHttpClient using HttpClient.
/// </summary>
public class HealthHttpClient(HttpClient httpClient) : IHealthHttpClient
{
    private readonly HttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

    public async Task<HttpResponseMessage> GetAsync(string requestUri, TimeSpan timeout, CancellationToken cancellationToken)
    {
        using var timeoutCts = new CancellationTokenSource(timeout);
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

        return await _httpClient.GetAsync(requestUri, linkedCts.Token).ConfigureAwait(false);
    }
}
