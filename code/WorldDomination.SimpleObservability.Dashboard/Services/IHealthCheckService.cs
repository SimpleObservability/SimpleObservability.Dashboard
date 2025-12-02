namespace WorldDomination.SimpleObservability.Dashboard.Services;

/// <summary>
/// Service responsible for checking the health of monitored services.
/// </summary>
public interface IHealthCheckService
{
    /// <summary>
    /// Checks the health of a specific service endpoint.
    /// </summary>
    /// <param name="serviceEndpoint">The service endpoint to check.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The health metadata from the service, or null if the check failed.</returns>
    Task<HealthCheckResult> CheckHealthAsync(ServiceEndpoint serviceEndpoint, CancellationToken cancellationToken);

    /// <summary>
    /// Checks the health of all configured service endpoints.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A dictionary of service names to their health check results.</returns>
    Task<Dictionary<string, HealthCheckResult>> CheckAllHealthAsync(CancellationToken cancellationToken);
}
