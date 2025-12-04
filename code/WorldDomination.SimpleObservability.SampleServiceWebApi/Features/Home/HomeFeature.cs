namespace SimpleObservability.SampleService.WebApi.Features.Home;

/// <summary>
/// Feature class for home-related endpoints.
/// </summary>
public static class HomeFeature
{
    /// <summary>
    /// Maps all home-related endpoints.
    /// </summary>
    /// <param name="app">The endpoint route builder.</param>
    /// <returns>The endpoint route builder for chaining.</returns>
    public static IEndpointRouteBuilder MapHomeEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapHelloWorld();

        return app;
    }
}
