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
            // Enable buffering to allow reading the body multiple times.
            context.Request.EnableBuffering();

            // Read the raw JSON to see what's actually being sent.
            using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
            var jsonBody = await reader.ReadToEndAsync().ConfigureAwait(false);
            logger.LogInformation("Raw JSON received: {Json}", jsonBody);

            // Reset the stream position so it can be read again for deserialization.
            context.Request.Body.Position = 0;

            // Manually deserialize to see what we get.
            var updatedConfig = JsonSerializer.Deserialize<DashboardConfiguration>(jsonBody, JsonConfiguration.DefaultOptions);

            if (updatedConfig is null)
            {
                return Results.BadRequest(new { Error = "Invalid configuration data" });
            }

            logger.LogInformation("Deserialized configuration. EnvironmentOrder: {EnvironmentOrder}",
                updatedConfig.EnvironmentOrder != null
                ? string.Join(", ", updatedConfig.EnvironmentOrder)
                : "null");

            // Validation.
            if (updatedConfig.Services is null || updatedConfig.Services.Count == 0)
            {
                return Results.BadRequest(new { Error = "Services list cannot be empty" });
            }

            if (updatedConfig.TimeoutSeconds <= 0)
            {
                return Results.BadRequest(new { Error = "TimeoutSeconds must be greater than 0" });
            }

            if (updatedConfig.RefreshIntervalSeconds <= 0)
            {
                return Results.BadRequest(new { Error = "RefreshIntervalSeconds must be greater than 0" });
            }

            // Update the configuration in-memory by creating a new instance.
            configHolder.Config = updatedConfig;

            logger.LogInformation("Configuration updated. Current EnvironmentOrder: {EnvironmentOrder}",
                configHolder.Config.EnvironmentOrder != null
                    ? string.Join(", ", configHolder.Config.EnvironmentOrder)
                    : "null");

            return Results.Ok(new { Message = "Configuration updated successfully (in-memory only)", Config = configHolder.Config });
        })
        .WithName("UpdateConfiguration");

        return app;
    }
}
