using System.Net;

namespace WorldDomination.SimpleObservability.Dashboard.Services;

/// <summary>
/// Represents the result of a health check operation.
/// </summary>
public record HealthCheckResult
{
    /// <summary>
    /// The service endpoint that was checked. Required.
    /// </summary>
    public required ServiceEndpoint ServiceEndpoint { get; init; }

    /// <summary>
    /// Whether the health check was successful.
    /// </summary>
    public bool IsSuccess { get; init; }

    /// <summary>
    /// The health metadata returned by the service, if successful.
    /// </summary>
    public HealthMetadata? HealthMetadata { get; init; }

    /// <summary>
    /// Error message if the health check failed.
    /// </summary>
    public string? ErrorMessage { get; init; }

    /// <summary>
    /// HTTP status code returned by the health check endpoint.
    /// </summary>
    public HttpStatusCode? StatusCode { get; init; }

    /// <summary>
    /// Timestamp when the health check was performed.
    /// </summary>
    public DateTimeOffset CheckedAt { get; init; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Indicates whether the service is intentionally disabled via configuration.
    /// </summary>
    public bool IsDisabled { get; init; }
}
