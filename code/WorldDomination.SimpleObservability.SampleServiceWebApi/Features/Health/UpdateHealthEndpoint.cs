namespace SimpleObservability.SampleService.WebApi.Features.Health;

/// <summary>
/// Endpoint handler for updating the health metadata.
/// </summary>
public static class UpdateHealthEndpoint
{
    /// <summary>
    /// Maps the PUT /health endpoint.
    /// </summary>
    /// <param name="app">The endpoint route builder.</param>
    /// <returns>The endpoint route builder for chaining.</returns>
    public static IEndpointRouteBuilder MapUpdateHealth(this IEndpointRouteBuilder app)
    {
        app.MapPut("/health", (UpdateHealthMetadataRequest request, HealthState healthState) =>
        {
            // Validate status if provided.
            if (request.Status.HasValue && !Enum.IsDefined(request.Status.Value))
            {
                return Results.BadRequest(new { Error = "Invalid status. Use 'Healthy', 'Degraded', or 'Unhealthy'." });
            }

            // Update only the properties that were provided.
            if (request.Status.HasValue)
            {
                healthState.CurrentStatus = request.Status.Value;
            }

            if (!string.IsNullOrWhiteSpace(request.ServiceName))
            {
                healthState.CurrentServiceName = request.ServiceName;
            }

            if (!string.IsNullOrWhiteSpace(request.Version))
            {
                healthState.CurrentVersion = request.Version;
            }

            if (request.Environment is not null)
            {
                healthState.CurrentEnvironment = request.Environment;
            }

            if (request.Description is not null)
            {
                healthState.CurrentDescription = request.Description;
            }

            if (request.HostName is not null)
            {
                healthState.CurrentHostName = request.HostName;
            }

            // Build response showing what was updated.
            var updatedFields = new List<string>();
            if (request.Status.HasValue)
            {
                updatedFields.Add($"Status={healthState.CurrentStatus}");
            }

            if (request.ServiceName is not null)
            {
                updatedFields.Add($"ServiceName={healthState.CurrentServiceName}");
            }

            if (request.Version is not null)
            {
                updatedFields.Add($"Version={healthState.CurrentVersion}");
            }

            if (request.Environment is not null)
            {
                updatedFields.Add($"Environment={healthState.CurrentEnvironment}");
            }

            if (request.Description is not null)
            {
                updatedFields.Add($"Description={healthState.CurrentDescription}");
            }

            if (request.HostName is not null)
            {
                updatedFields.Add($"HostName={healthState.CurrentHostName}");
            }

            var result = new
            {
                Message = $"Health metadata updated: {string.Join(", ", updatedFields)}",
                CurrentMetadata = new
                {
                    Status = healthState.CurrentStatus,
                    ServiceName = healthState.CurrentServiceName,
                    Version = healthState.CurrentVersion,
                    Environment = healthState.CurrentEnvironment,
                    Description = healthState.CurrentDescription,
                    HostName = healthState.CurrentHostName
                }
            };

            return Results.Ok(result);
        })
        .WithName("UpdateHealthMetadata")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);

        return app;
    }
}
