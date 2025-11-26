using System.Text.Json.Serialization;
using WorldDomination.SimpleObservability;

namespace SimpleObservability.SampleService.WebApi;

/// <summary>
/// Request model for updating health metadata.
/// All properties are optional - only provided values will be updated.
/// </summary>
public record UpdateHealthMetadataRequest
{
    /// <summary>
    /// The new health status. Accepts: "Healthy", "Degraded", or "Unhealthy".
    /// </summary>
    [JsonConverter(typeof(HealthStatusJsonConverter))]
    public HealthStatus? Status { get; init; }

    /// <summary>
    /// The new service name.
    /// </summary>
    public string? ServiceName { get; init; }

    /// <summary>
    /// The new version.
    /// </summary>
    public string? Version { get; init; }

    /// <summary>
    /// The new environment.
    /// </summary>
    public string? Environment { get; init; }

    /// <summary>
    /// The new description.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// The new hostname.
    /// </summary>
    public string? HostName { get; init; }
}
