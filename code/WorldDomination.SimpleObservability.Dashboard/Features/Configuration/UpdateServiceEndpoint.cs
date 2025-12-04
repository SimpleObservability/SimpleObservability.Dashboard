using WorldDomination.SimpleObservability.Dashboard.Configuration;

namespace WorldDomination.SimpleObservability.Dashboard.Features.Configuration;

/// <summary>
/// Endpoint handler for updating a service.
/// </summary>
public static class UpdateServiceEndpoint
{
    /// <summary>
    /// Maps the PUT /api/config/services/{serviceName}/{environment} endpoint.
    /// </summary>
    /// <param name="app">The endpoint route builder.</param>
    /// <returns>The endpoint route builder for chaining.</returns>
    public static IEndpointRouteBuilder MapUpdateService(this IEndpointRouteBuilder app)
    {
        app.MapPut("/api/config/services/{serviceName}/{environment}", (string serviceName, string environment, ServiceEndpoint updatedService, ConfigurationHolder configHolder) =>
        {
            var service = configHolder.Config.Services.FirstOrDefault(s =>
                s.Name.Equals(serviceName, StringComparison.OrdinalIgnoreCase) &&
                s.Environment.Equals(environment, StringComparison.OrdinalIgnoreCase));

            if (service is null)
            {
                return Results.NotFound(new { Error = $"Service '{serviceName}' in environment '{environment}' not found" });
            }

            // Create new configuration with updated service.
            var updatedServices = configHolder.Config.Services
                .Where(s => !(s.Name.Equals(serviceName, StringComparison.OrdinalIgnoreCase) &&
                              s.Environment.Equals(environment, StringComparison.OrdinalIgnoreCase)))
                .Append(updatedService)
                .ToList();

            configHolder.Config = configHolder.Config with { Services = updatedServices };

            return Results.Ok(new { Message = "Service updated successfully", Service = updatedService });
        })
        .WithName("UpdateService");

        return app;
    }
}
