namespace WorldDomination.SimpleObservability.Dashboard.Services;

/// <summary>
/// Abstraction for making health check HTTP requests.
/// </summary>
public interface IHealthHttpClient
{
    /// <summary>
    /// Sends a GET request to the specified URI.
    /// </summary>
    /// <param name="requestUri">The URI to request.</param>
    /// <param name="timeout">The timeout for the request.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The HTTP response message.</returns>
    Task<HttpResponseMessage> GetAsync(string requestUri, TimeSpan timeout, CancellationToken cancellationToken);
}
