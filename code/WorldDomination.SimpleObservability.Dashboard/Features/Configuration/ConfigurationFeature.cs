namespace WorldDomination.SimpleObservability.Dashboard.Features.Configuration;

/// <summary>
/// Feature class for configuration-related endpoints.
/// </summary>
public static class ConfigurationFeature
{
    /// <summary>
    /// Maps all configuration-related endpoints.
    /// </summary>
    /// <param name="app">The endpoint route builder.</param>
    /// <returns>The endpoint route builder for chaining.</returns>
    public static IEndpointRouteBuilder MapConfigurationEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGetConfiguration();
        app.MapUpdateConfiguration();
        app.MapAddService();
        app.MapUpdateService();
        app.MapDeleteService();

        return app;
    }
}
