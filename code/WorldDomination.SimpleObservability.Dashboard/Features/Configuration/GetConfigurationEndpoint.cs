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
            logger.LogInformation("Retrieving dashboard configuration.");

            logger.LogDebug("Configuration details. Services: {ServiceCount}, RefreshInterval: {RefreshInterval}s, Timeout: {Timeout}s, EnvironmentOrder: {EnvironmentOrder}",
                configHolder.Config.Services?.Count ?? 0,
                configHolder.Config.RefreshIntervalSeconds,
                configHolder.Config.TimeoutSeconds,
                configHolder.Config.EnvironmentOrder != null ? string.Join(", ", configHolder.Config.EnvironmentOrder) : "null");

            return Results.Ok(configHolder.Config);
        })
        .WithName("GetConfiguration");

        return app;
    }
}
