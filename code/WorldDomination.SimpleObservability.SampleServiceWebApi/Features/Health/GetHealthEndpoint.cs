using WorldDomination.SimpleObservability;

namespace SimpleObservability.SampleService.WebApi.Features.Health;

/// <summary>
/// Endpoint handler for getting the health status.
/// </summary>
public static class GetHealthEndpoint
{
    /// <summary>
    /// Maps the GET /healthz endpoint.
    /// </summary>
    /// <param name="app">The endpoint route builder.</param>
    /// <returns>The endpoint route builder for chaining.</returns>
    public static IEndpointRouteBuilder MapGetHealth(this IEndpointRouteBuilder app)
    {
        app.MapGet("/healthz", (HealthState healthState) =>
        {
            var uptime = DateTimeOffset.UtcNow - healthState.StartTime;

            var response = new HealthMetadata
            {
                ServiceName = healthState.CurrentServiceName,
                Version = healthState.CurrentVersion,
                Environment = healthState.CurrentEnvironment,
                Status = healthState.CurrentStatus,
                Timestamp = DateTimeOffset.UtcNow,
                Uptime = uptime,
                Description = healthState.CurrentDescription,
                HostName = healthState.CurrentHostName,
                AdditionalMetadata = new Dictionary<string, string>
                {
                    ["Runtime"] = ".NET 10",
                    ["MachineName"] = System.Environment.MachineName,
                    ["ProcessorCount"] = System.Environment.ProcessorCount.ToString()
                }
            };

            return Results.Ok(response);
        })
        .WithName("HealthCheck")
        .Produces<HealthMetadata>(StatusCodes.Status200OK);

        return app;
    }
}
