using WorldDomination.SimpleObservability.Dashboard.Features.Configuration;
using WorldDomination.SimpleObservability.Dashboard.Features.Health;

namespace WorldDomination.SimpleObservability.Dashboard.Features;

/// <summary>
/// Extension methods for mapping all application endpoints.
/// </summary>
public static class EndpointExtensions
{
    /// <summary>
    /// Maps all application endpoints by calling each feature's endpoint registration.
    /// </summary>
    /// <param name="app">The endpoint route builder.</param>
    /// <returns>The endpoint route builder for chaining.</returns>
    public static IEndpointRouteBuilder MapAllEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapHealthEndpoints();
        app.MapConfigurationEndpoints();

        return app;
    }
}
