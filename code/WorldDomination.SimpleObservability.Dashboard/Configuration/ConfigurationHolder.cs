namespace WorldDomination.SimpleObservability.Dashboard.Configuration;

/// <summary>
/// Mutable holder for the dashboard configuration to allow in-memory updates.
/// </summary>
public class ConfigurationHolder
{
    /// <summary>
    /// Gets or sets the current dashboard configuration.
    /// Initialized with a default configuration to ensure it's never null.
    /// </summary>
    public DashboardConfiguration Config { get; set; } = DashboardConfigurationLoader.CreateDefaultConfiguration();
}
