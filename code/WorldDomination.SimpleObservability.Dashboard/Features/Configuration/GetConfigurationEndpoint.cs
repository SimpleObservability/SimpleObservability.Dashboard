using WorldDomination.SimpleObservability.Dashboard.Configuration;

namespace WorldDomination.SimpleObservability.Dashboard.Features.Configuration;

/// <summary>
/// Endpoint handler for getting the current configuration.
/// </summary>
public static class GetConfigurationEndpoint
{
    /// <summary>
    /// Maps the GET /api/config endpoint.
    /// </summary>
    /// <param name="app">The endpoint route builder.</param>
    /// <returns>The endpoint route builder for chaining.</returns>
    public static IEndpointRouteBuilder MapGetConfiguration(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/config", (ConfigurationHolder configHolder, ILogger<DashboardConfiguration> logger) =>
        {
            logger.LogInformation("Returning configuration. EnvironmentOrder: {EnvironmentOrder}",
                configHolder.Config.EnvironmentOrder != null ? string.Join(", ", configHolder.Config.EnvironmentOrder) : "null");

            return Results.Ok(configHolder.Config);
        })
        .WithName("GetConfiguration");

        return app;
    }
}
