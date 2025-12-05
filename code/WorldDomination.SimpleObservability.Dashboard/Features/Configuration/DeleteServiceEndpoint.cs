using WorldDomination.SimpleObservability.Dashboard.Configuration;

namespace WorldDomination.SimpleObservability.Dashboard.Features.Configuration;

/// <summary>
/// Endpoint handler for deleting a service.
/// </summary>
public static class DeleteServiceEndpoint
{
    /// <summary>
    /// Maps the DELETE /api/config/services/{serviceName}/{environment} endpoint.
    /// </summary>
    /// <param name="app">The endpoint route builder.</param>
    /// <returns>The endpoint route builder for chaining.</returns>
    public static IEndpointRouteBuilder MapDeleteService(this IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/config/services/{serviceName}/{environment}", (string serviceName, string environment, ConfigurationHolder configHolder, ILogger<ConfigurationHolder> logger) =>
        {
            logger.LogInformation("Deleting service. Name: {ServiceName}, Environment: {Environment}",
                serviceName,
                environment);

            var service = configHolder.Config.Services.FirstOrDefault(s =>
                s.Name.Equals(serviceName, StringComparison.OrdinalIgnoreCase) &&
                s.Environment.Equals(environment, StringComparison.OrdinalIgnoreCase));

            if (service is null)
            {
                logger.LogDebug("Service not found for deletion. Name: {ServiceName}, Environment: {Environment}",
                    serviceName,
                    environment);
                return Results.NotFound(new { Error = $"Service '{serviceName}' in environment '{environment}' not found" });
            }

            logger.LogDebug("Found service to delete. HealthCheckUrl: {HealthCheckUrl}, Enabled: {Enabled}",
                service.HealthCheckUrl,
                service.Enabled);

            // Create new configuration without the deleted service.
            var updatedServices = configHolder.Config.Services
                .Where(s => !(s.Name.Equals(serviceName, StringComparison.OrdinalIgnoreCase) &&
                              s.Environment.Equals(environment, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            configHolder.Config = configHolder.Config with { Services = updatedServices };

            logger.LogInformation("Service deleted successfully. Name: {ServiceName}, Environment: {Environment}. Remaining services: {RemainingServices}",
                serviceName,
                environment,
                updatedServices.Count);

            return Results.Ok(new { Message = "Service deleted successfully" });
        })
        .WithName("DeleteService");

        return app;
    }
}
