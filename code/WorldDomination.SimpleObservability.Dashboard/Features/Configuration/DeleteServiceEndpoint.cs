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
        app.MapDelete("/api/config/services/{serviceName}/{environment}", (string serviceName, string environment, ConfigurationHolder configHolder) =>
        {
            var service = configHolder.Config.Services.FirstOrDefault(s =>
                s.Name.Equals(serviceName, StringComparison.OrdinalIgnoreCase) &&
                s.Environment.Equals(environment, StringComparison.OrdinalIgnoreCase));

            if (service is null)
            {
                return Results.NotFound(new { Error = $"Service '{serviceName}' in environment '{environment}' not found" });
            }

            // Create new configuration without the deleted service.
            var updatedServices = configHolder.Config.Services
                .Where(s => !(s.Name.Equals(serviceName, StringComparison.OrdinalIgnoreCase) &&
                              s.Environment.Equals(environment, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            configHolder.Config = configHolder.Config with { Services = updatedServices };

            return Results.Ok(new { Message = "Service deleted successfully" });
        })
        .WithName("DeleteService");

        return app;
    }
}
