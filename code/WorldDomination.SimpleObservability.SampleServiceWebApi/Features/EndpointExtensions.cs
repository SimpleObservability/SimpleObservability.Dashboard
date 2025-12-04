using SimpleObservability.SampleService.WebApi.Features.Health;
using SimpleObservability.SampleService.WebApi.Features.Home;

namespace SimpleObservability.SampleService.WebApi.Features;

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
        app.MapHomeEndpoints();

        return app;
    }

    /// <summary>
    /// Adds all feature services to the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddAllFeatures(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHealthFeature(configuration);

        return services;
    }
}
