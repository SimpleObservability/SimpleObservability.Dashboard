namespace SimpleObservability.SampleService.WebApi.Features.Health;

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
        app.MapGetHealth();
        app.MapUpdateHealth();

        return app;
    }

    /// <summary>
    /// Adds health feature services to the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddHealthFeature(this IServiceCollection services, IConfiguration configuration)
    {
        var healthState = new HealthState
        {
            CurrentServiceName = configuration["HealthMetadata:ServiceName"] ?? "Sample Service",
            CurrentVersion = configuration["HealthMetadata:Version"] ?? "1.0.0",
            CurrentEnvironment = configuration["HealthMetadata:Environment"] ?? "Development",
            CurrentDescription = configuration["HealthMetadata:Description"] ?? "Sample service for testing Simple Observability",
            CurrentHostName = System.Net.Dns.GetHostName()
        };

        services.AddSingleton(healthState);

        return services;
    }
}
