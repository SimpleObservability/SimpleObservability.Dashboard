using WorldDomination.SimpleObservability.Dashboard.Configuration;

namespace WorldDomination.SimpleObservability.Dashboard.Tests.Configuration;

/// <summary>
/// Tests for the <see cref="ConfigurationHolder"/> class.
/// </summary>
public class ConfigurationHolderTests
{
    [Fact]
    public void Constructor_ShouldCreateInstance()
    {
        // Arrange.
        // Act.
        var holder = new ConfigurationHolder();

        // Assert.
        holder.ShouldNotBeNull();
    }

    [Fact]
    public void Config_ShouldBeSettable()
    {
        // Arrange.
        var holder = new ConfigurationHolder();
        var config = new DashboardConfiguration
        {
            Services = new List<ServiceEndpoint>
            {
                new()
                {
                    Name = "Test Service",
                    Environment = "DEV",
                    HealthCheckUrl = "http://localhost:5000/healthz"
                }
            },
            RefreshIntervalSeconds = 60,
            TimeoutSeconds = 10
        };

        // Act.
        holder.Config = config;

        // Assert.
        holder.Config.ShouldBe(config);
        holder.Config.Services.Count.ShouldBe(1);
        holder.Config.RefreshIntervalSeconds.ShouldBe(60);
        holder.Config.TimeoutSeconds.ShouldBe(10);
    }

    [Fact]
    public void Config_ShouldBeMutable()
    {
        // Arrange.
        var holder = new ConfigurationHolder();
        var config1 = new DashboardConfiguration
        {
            Services = new List<ServiceEndpoint>(),
            RefreshIntervalSeconds = 30
        };

        var config2 = new DashboardConfiguration
        {
            Services = new List<ServiceEndpoint>
            {
                new()
                {
                    Name = "New Service",
                    Environment = "PROD",
                    HealthCheckUrl = "http://localhost:6000/healthz"
                }
            },
            RefreshIntervalSeconds = 120
        };

        // Act.
        holder.Config = config1;
        var firstConfig = holder.Config;
        
        holder.Config = config2;
        var secondConfig = holder.Config;

        // Assert.
        firstConfig.RefreshIntervalSeconds.ShouldBe(30);
        firstConfig.Services.ShouldBeEmpty();
        
        secondConfig.RefreshIntervalSeconds.ShouldBe(120);
        secondConfig.Services.Count.ShouldBe(1);
        secondConfig.Services[0].Name.ShouldBe("New Service");
    }

    [Fact]
    public void Config_WithMultipleUpdates_ShouldRetainLatestValue()
    {
        // Arrange.
        var holder = new ConfigurationHolder();

        // Act.
        for (int i = 1; i <= 5; i++)
        {
            holder.Config = new DashboardConfiguration
            {
                Services = new List<ServiceEndpoint>(),
                RefreshIntervalSeconds = i * 10
            };
        }

        // Assert.
        holder.Config.RefreshIntervalSeconds.ShouldBe(50);
    }
}
