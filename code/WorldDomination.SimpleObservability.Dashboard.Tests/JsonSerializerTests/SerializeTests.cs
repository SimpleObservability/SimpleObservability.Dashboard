using WorldDomination.SimpleObservability.Dashboard.Configuration;

namespace WorldDomination.SimpleObservability.Dashboard.Tests.JsonSerializerTests;
public class SerializeTests
{
    [Fact]
    public void Serialize_ShouldProduceCamelCaseJson()
    {
        // Arrange.
        var metadata = new HealthMetadata
        {
            ServiceName = "Test Service",
            Version = "1.2.3",
            Environment = "Production",
            Status = HealthStatus.Healthy,
            Description = "All systems operational",
            HostName = "test-server-01",
            Uptime = TimeSpan.FromDays(1).Add(TimeSpan.FromHours(2)).Add(TimeSpan.FromMinutes(30)),
            AdditionalMetadata = new Dictionary<string, string>
            {
                ["database"] = "Connected",
                ["cache"] = "Redis"
            }
        };

        // Act.
        var json = JsonSerializer.Serialize(metadata, JsonConfiguration.DefaultOptions);

        // Assert.
        json.ShouldContain("\"serviceName\"");
        json.ShouldContain("\"version\"");
        json.ShouldContain("\"environment\"");
        json.ShouldContain("\"status\"");
        json.ShouldContain("\"description\"");
        json.ShouldContain("\"hostName\"");
        json.ShouldContain("\"uptime\"");
        json.ShouldContain("\"additionalMetadata\"");

        // Verify PascalCase is NOT present (case-sensitive check).
        json.IndexOf("\"ServiceName\"", StringComparison.Ordinal).ShouldBe(-1);
        json.IndexOf("\"Version\"", StringComparison.Ordinal).ShouldBe(-1);
        json.IndexOf("\"AdditionalMetadata\"", StringComparison.Ordinal).ShouldBe(-1);
    }

    [Theory]
    [InlineData(HealthStatus.Healthy)]
    [InlineData(HealthStatus.Degraded)]
    [InlineData(HealthStatus.Unhealthy)]
    public void Serialize_GivenSomeHealthMetadataAndASpecificHealthStatus_ShouldSerializeAsString(HealthStatus healthStatus)
    {
        // Arrange.
        var metadata = new HealthMetadata
        {
            ServiceName = "Test",
            Version = "1.0",
            Status = healthStatus
        };

        // Act.
        var json = JsonSerializer.Serialize(metadata, JsonConfiguration.DefaultOptions);

        // Assert.
        // JsonStringEnumConverter writes enum values as their string names.
        json.Replace(" ", "").ShouldContain($"\"status\":\"{healthStatus}\"");
        json.Replace(" ", "").IndexOf("\"status\":1", StringComparison.Ordinal).ShouldBe(-1);
    }
}
