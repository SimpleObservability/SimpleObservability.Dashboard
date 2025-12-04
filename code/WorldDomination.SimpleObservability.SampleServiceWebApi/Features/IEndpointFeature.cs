namespace SimpleObservability.SampleService.WebApi.Features;

/// <summary>
/// Interface for endpoint features that can be registered with the application.
/// </summary>
public interface IEndpointFeature
{
    /// <summary>
    /// Maps the endpoints for this feature to the application.
    /// </summary>
    /// <param name="app">The web application to map endpoints to.</param>
    /// <returns>The web application for chaining.</returns>
    static abstract IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder app);
}
