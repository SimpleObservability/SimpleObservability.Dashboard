using WorldDomination.SimpleObservability.Dashboard.Configuration;
using WorldDomination.SimpleObservability.Dashboard.Services;

namespace WorldDomination.SimpleObservability.Dashboard.Features.Health;

/// <summary>
/// Endpoint handler for getting all health data.
/// </summary>
public static class GetAllHealthEndpoint
{
    /// <summary>
    /// Maps the GET /api/health endpoint.
    /// </summary>
    /// <param name="app">The endpoint route builder.</param>
    /// <returns>The endpoint route builder for chaining.</returns>
    public static IEndpointRouteBuilder MapGetAllHealth(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/health", async (ConfigurationHolder configHolder, IHealthCheckService healthCheckService, CancellationToken cancellationToken) =>
        {
            var results = await healthCheckService.CheckAllHealthAsync(cancellationToken).ConfigureAwait(false);

            return Results.Ok(new
            {
                Environments = configHolder.Config.Environments,
                Services = configHolder.Config.Services,
                Results = results,
                RefreshIntervalSeconds = configHolder.Config.RefreshIntervalSeconds,
                Timestamp = DateTimeOffset.UtcNow
            });
        })
        .WithName("GetHealthStatus");

        return app;
    }
}
