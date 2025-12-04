using WorldDomination.SimpleObservability;

namespace SimpleObservability.SampleService.WebApi.Features.Health;

/// <summary>
/// Holds the current health state for the sample service.
/// This is a mutable singleton that tracks the health metadata values.
/// </summary>
public sealed class HealthState
{
    /// <summary>
    /// Gets or sets the current health status.
    /// </summary>
    public HealthStatus CurrentStatus { get; set; } = HealthStatus.Healthy;

    /// <summary>
    /// Gets or sets the current service name.
    /// </summary>
    public string CurrentServiceName { get; set; } = "Sample Service";

    /// <summary>
    /// Gets or sets the current version.
    /// </summary>
    public string CurrentVersion { get; set; } = "1.0.0";

    /// <summary>
    /// Gets or sets the current environment.
    /// </summary>
    public string CurrentEnvironment { get; set; } = "Development";

    /// <summary>
    /// Gets or sets the current description.
    /// </summary>
    public string CurrentDescription { get; set; } = "Sample service for testing Simple Observability";

    /// <summary>
    /// Gets or sets the current hostname.
    /// </summary>
    public string CurrentHostName { get; set; } = System.Net.Dns.GetHostName();

    /// <summary>
    /// Gets the time when the service started.
    /// </summary>
    public DateTimeOffset StartTime { get; } = DateTimeOffset.UtcNow;
}
