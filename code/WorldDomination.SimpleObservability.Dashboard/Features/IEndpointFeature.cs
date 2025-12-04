namespace WorldDomination.SimpleObservability.Dashboard.Features;

/// <summary>
/// Interface for endpoint features that can be registered with the application.
/// </summary>
public interface IEndpointFeature
{
    /// <summary>
    /// Maps the endpoints for this feature to the application.
    /// </summary>
    /// <param name="app">The endpoint route builder.</param>
    /// <returns>The endpoint route builder for chaining.</returns>
    static abstract IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder app);
}
