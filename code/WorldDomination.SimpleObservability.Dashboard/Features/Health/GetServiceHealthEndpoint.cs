using WorldDomination.SimpleObservability.Dashboard.Configuration;
using WorldDomination.SimpleObservability.Dashboard.Services;

namespace WorldDomination.SimpleObservability.Dashboard.Features.Health;

/// <summary>
/// Endpoint handler for getting health data for a specific service.
/// </summary>
public static class GetServiceHealthEndpoint
{
    /// <summary>
    /// Maps the GET /api/health/{serviceName} endpoint.
    /// </summary>
    /// <param name="app">The endpoint route builder.</param>
    /// <returns>The endpoint route builder for chaining.</returns>
    public static IEndpointRouteBuilder MapGetServiceHealth(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/health/{serviceName}", async (string serviceName, ConfigurationHolder configHolder, IHealthCheckService healthCheckService, ILogger<IHealthCheckService> logger, CancellationToken cancellationToken) =>
        {
            logger.LogInformation("Retrieving health status for service. ServiceName: {ServiceName}", serviceName);

            var service = configHolder.Config.Services.FirstOrDefault(s =>
                s.Name.Equals(serviceName, StringComparison.OrdinalIgnoreCase));

            if (service is null)
            {
                logger.LogDebug("Service not found. ServiceName: {ServiceName}", serviceName);
                return Results.NotFound(new { Error = $"Service '{serviceName}' not found" });
            }

            logger.LogDebug("Found service. Name: {ServiceName}, Environment: {Environment}, HealthCheckUrl: {HealthCheckUrl}",
                service.Name,
                service.Environment,
                service.HealthCheckUrl);

            var result = await healthCheckService.CheckHealthAsync(service, cancellationToken).ConfigureAwait(false);

            logger.LogDebug("Health check result for {ServiceName}. IsSuccess: {IsSuccess}, StatusCode: {StatusCode}",
                serviceName,
                result.IsSuccess,
                result.StatusCode);

            logger.LogInformation("Health status retrieved successfully for service. ServiceName: {ServiceName}, IsSuccess: {IsSuccess}",
                serviceName,
                result.IsSuccess);

            return Results.Ok(result);
        })
        .WithName("GetServiceHealth");

        return app;
    }
}
