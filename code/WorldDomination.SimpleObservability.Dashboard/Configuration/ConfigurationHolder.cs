namespace WorldDomination.SimpleObservability.Dashboard.Configuration;

/// <summary>
/// Mutable holder for the dashboard configuration to allow in-memory updates.
/// </summary>
public class ConfigurationHolder
{
    /// <summary>
    /// Gets or sets the current dashboard configuration.
    /// </summary>
    public DashboardConfiguration Config { get; set; } = null!;
}
