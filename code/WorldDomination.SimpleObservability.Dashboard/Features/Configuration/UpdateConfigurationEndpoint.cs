using System.Text.Json;
using WorldDomination.SimpleObservability.Dashboard.Configuration;

namespace WorldDomination.SimpleObservability.Dashboard.Features.Configuration;

/// <summary>
/// Endpoint handler for updating the dashboard configuration.
/// </summary>
public static class UpdateConfigurationEndpoint
{
    /// <summary>
    /// Maps the PUT /api/config endpoint.
    /// </summary>
    /// <param name="app">The endpoint route builder.</param>
    /// <returns>The endpoint route builder for chaining.</returns>
    public static IEndpointRouteBuilder MapUpdateConfiguration(this IEndpointRouteBuilder app)
    {
        app.MapPut("/api/config", async (HttpContext context, ConfigurationHolder configHolder, ILogger<DashboardConfiguration> logger) =>
        {
            logger.LogInformation("Updating dashboard configuration.");

            // Enable buffering to allow reading the body multiple times.
            context.Request.EnableBuffering();

            // Read the raw JSON to see what's actually being sent.
            using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
            var jsonBody = await reader.ReadToEndAsync().ConfigureAwait(false);
            logger.LogDebug("Raw JSON received: {Json}", jsonBody);

            // Reset the stream position so it can be read again for deserialization.
            context.Request.Body.Position = 0;

            // Manually deserialize to see what we get.
            var updatedConfig = JsonSerializer.Deserialize<DashboardConfiguration>(jsonBody, JsonConfiguration.DefaultOptions);

            if (updatedConfig is null)
            {
                logger.LogDebug("Deserialization failed: Invalid configuration data.");
                return Results.BadRequest(new { Error = "Invalid configuration data" });
            }

            logger.LogDebug("Deserialized configuration. Services: {ServiceCount}, RefreshInterval: {RefreshInterval}s, Timeout: {Timeout}s, EnvironmentOrder: {EnvironmentOrder}",
                updatedConfig.Services?.Count ?? 0,
                updatedConfig.RefreshIntervalSeconds,
                updatedConfig.TimeoutSeconds,
                updatedConfig.EnvironmentOrder != null ? string.Join(", ", updatedConfig.EnvironmentOrder) : "null");

            // Validation.
            if (updatedConfig.Services is null || updatedConfig.Services.Count == 0)
            {
                logger.LogDebug("Validation failed: Services list cannot be empty.");
                return Results.BadRequest(new { Error = "Services list cannot be empty" });
            }

            if (updatedConfig.TimeoutSeconds <= 0)
            {
                logger.LogDebug("Validation failed: TimeoutSeconds must be greater than 0. Received: {TimeoutSeconds}", updatedConfig.TimeoutSeconds);
                return Results.BadRequest(new { Error = "TimeoutSeconds must be greater than 0" });
            }

            if (updatedConfig.RefreshIntervalSeconds <= 0)
            {
                logger.LogDebug("Validation failed: RefreshIntervalSeconds must be greater than 0. Received: {RefreshIntervalSeconds}", updatedConfig.RefreshIntervalSeconds);
                return Results.BadRequest(new { Error = "RefreshIntervalSeconds must be greater than 0" });
            }

            // Update the configuration in-memory by creating a new instance.
            configHolder.Config = updatedConfig;

            logger.LogInformation("Configuration updated successfully. Services: {ServiceCount}, RefreshInterval: {RefreshInterval}s, Timeout: {Timeout}s",
                configHolder.Config.Services.Count,
                configHolder.Config.RefreshIntervalSeconds,
                configHolder.Config.TimeoutSeconds);

            return Results.Ok(new { Message = "Configuration updated successfully (in-memory only)", Config = configHolder.Config });
        })
        .WithName("UpdateConfiguration");

        return app;
    }
}
