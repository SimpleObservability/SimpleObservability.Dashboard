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
        app.MapGet("/api/health/{serviceName}", async (string serviceName, ConfigurationHolder configHolder, IHealthCheckService healthCheckService, CancellationToken cancellationToken) =>
        {
            var service = configHolder.Config.Services.FirstOrDefault(s =>
                s.Name.Equals(serviceName, StringComparison.OrdinalIgnoreCase));

            if (service is null)
            {
                return Results.NotFound(new { Error = $"Service '{serviceName}' not found" });
            }

            var result = await healthCheckService.CheckHealthAsync(service, cancellationToken).ConfigureAwait(false);
            return Results.Ok(result);
        })
        .WithName("GetServiceHealth");

        return app;
    }
}
