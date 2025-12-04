namespace WorldDomination.SimpleObservability.Dashboard.Features.Health;

/// <summary>
/// Feature class for health-related endpoints.
/// </summary>
public static class HealthFeature
{
    /// <summary>
    /// Maps all health-related endpoints.
    /// </summary>
    /// <param name="app">The endpoint route builder.</param>
    /// <returns>The endpoint route builder for chaining.</returns>
    public static IEndpointRouteBuilder MapHealthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGetAllHealth();
        app.MapGetServiceHealth();

        return app;
    }
}
