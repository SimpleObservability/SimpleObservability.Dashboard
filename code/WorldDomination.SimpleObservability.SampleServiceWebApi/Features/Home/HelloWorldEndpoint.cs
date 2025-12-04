using SimpleObservability.SampleService.WebApi.Features.Health;

namespace SimpleObservability.SampleService.WebApi.Features.Home;

/// <summary>
/// Endpoint handler for the hello world home page.
/// </summary>
public static class HelloWorldEndpoint
{
    /// <summary>
    /// Maps the GET / endpoint.
    /// </summary>
    /// <param name="app">The endpoint route builder.</param>
    /// <returns>The endpoint route builder for chaining.</returns>
    public static IEndpointRouteBuilder MapHelloWorld(this IEndpointRouteBuilder app)
    {
        app.MapGet("/", (HealthState healthState) =>
        {
            return Results.Ok(new
            {
                Message = "Hello from Sample Service!",
                Service = healthState.CurrentServiceName,
                Version = healthState.CurrentVersion,
                Environment = healthState.CurrentEnvironment,
                CurrentStatus = healthState.CurrentStatus.ToString()
            });
        })
        .WithName("HelloWorld");

        return app;
    }
}
