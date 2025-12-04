using WorldDomination.SimpleObservability.Dashboard.Configuration;

namespace WorldDomination.SimpleObservability.Dashboard.Features.Configuration;

/// <summary>
/// Endpoint handler for adding a new service.
/// </summary>
public static class AddServiceEndpoint
{
    /// <summary>
    /// Maps the POST /api/config/services endpoint.
    /// </summary>
    /// <param name="app">The endpoint route builder.</param>
    /// <returns>The endpoint route builder for chaining.</returns>
    public static IEndpointRouteBuilder MapAddService(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/config/services", (ServiceEndpoint newService, ConfigurationHolder configHolder) =>
        {
            // Validation.
            if (string.IsNullOrWhiteSpace(newService.Name))
            {
                return Results.BadRequest(new { Error = "Service name is required" });
            }

            if (string.IsNullOrWhiteSpace(newService.Environment))
            {
                return Results.BadRequest(new { Error = "Environment is required" });
            }

            if (string.IsNullOrWhiteSpace(newService.HealthCheckUrl))
            {
                return Results.BadRequest(new { Error = "Health check URL is required" });
            }

            // Check for duplicates (same name + environment).
            var exists = configHolder.Config.Services.Any(s =>
                s.Name.Equals(newService.Name, StringComparison.OrdinalIgnoreCase) &&
                s.Environment.Equals(newService.Environment, StringComparison.OrdinalIgnoreCase));

            if (exists)
            {
                return Results.Conflict(new { Error = $"Service '{newService.Name}' already exists in environment '{newService.Environment}'" });
            }

            // Create new configuration with added service.
            var updatedServices = new List<ServiceEndpoint>(configHolder.Config.Services) { newService };
            configHolder.Config = configHolder.Config with { Services = updatedServices };

            return Results.Created($"/api/config/services/{newService.Name}", new { Message = "Service added successfully", Service = newService });
        })
        .WithName("AddService");

        return app;
    }
}
