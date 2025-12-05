using WorldDomination.SimpleObservability.Dashboard.Configuration;

namespace WorldDomination.SimpleObservability.Dashboard.Tests.JsonSerializerTests;

/// <summary>
/// Tests for JSON serialization to ensure all API responses use camelCase.
/// </summary>
public class DeserializeTests
{
    [Fact]
    public void Deserialize_GivenANumericStatus_ShouldHandleCamelCaseJson()
    {
        // Arrange.
        var camelCaseJson = """
        {
          "serviceName": "Test Service",
          "version": "1.2.3",
          "environment": "Production",
          "status": 0,
          "timestamp": "2024-01-15T10:30:00Z",
          "description": "All systems operational",
          "hostName": "test-server-01",
          "uptime": "1.02:30:00",
          "additionalMetadata": {
            "database": "Connected",
            "cache": "Redis"
          }
        }
        """;

        // Act.
        var metadata = JsonSerializer.Deserialize<HealthMetadata>(camelCaseJson, JsonConfiguration.DefaultOptions);

        // Assert.
        metadata.ShouldNotBeNull();
        metadata.ServiceName.ShouldBe("Test Service");
        metadata.Version.ShouldBe("1.2.3");
        metadata.Environment.ShouldBe("Production");
        metadata.Status.ShouldBe(HealthStatus.Healthy);
        metadata.Description.ShouldBe("All systems operational");
        metadata.HostName.ShouldBe("test-server-01");
        metadata.AdditionalMetadata.ShouldNotBeNull();
        metadata.AdditionalMetadata!["database"].ShouldBe("Connected");
        metadata.AdditionalMetadata["cache"].ShouldBe("Redis");
    }

    [Fact]
    public void Deserialize_GivenAStringStatus_ShouldHandleCamelCaseJson()
    {
        // Arrange.
        var camelCaseJson = """
        {
          "serviceName": "Test Service",
          "version": "1.2.3",
          "environment": "Production",
          "status": "Healthy",
          "timestamp": "2024-01-15T10:30:00Z",
          "description": "All systems operational",
          "hostName": "test-server-01",
          "uptime": "1.02:30:00",
          "additionalMetadata": {
            "database": "Connected",
            "cache": "Redis"
          }
        }
        """;

        // Act.
        var metadata = JsonSerializer.Deserialize<HealthMetadata>(camelCaseJson, JsonConfiguration.DefaultOptions);

        // Assert.
        metadata.ShouldNotBeNull();
        metadata.ServiceName.ShouldBe("Test Service");
        metadata.Version.ShouldBe("1.2.3");
        metadata.Environment.ShouldBe("Production");
        metadata.Status.ShouldBe(HealthStatus.Healthy);
        metadata.Description.ShouldBe("All systems operational");
        metadata.HostName.ShouldBe("test-server-01");
        metadata.AdditionalMetadata.ShouldNotBeNull();
        metadata.AdditionalMetadata!["database"].ShouldBe("Connected");
        metadata.AdditionalMetadata["cache"].ShouldBe("Redis");
    }

    [Theory]
    [InlineData("Healthy", HealthStatus.Healthy)]
    [InlineData("healthy", HealthStatus.Healthy)]
    [InlineData("Degraded", HealthStatus.Degraded)]
    [InlineData("degraded", HealthStatus.Degraded)]
    [InlineData("Unhealthy", HealthStatus.Unhealthy)]
    [InlineData("unhealthy", HealthStatus.Unhealthy)]
    public void Deserialize_GivenVariousStatusStrings_ShouldSucceed(string statusValue, HealthStatus expectedStatus)
    {
        // Arrange.
        var camelCaseJson = $$"""
        {
          "serviceName": "Test Service",
          "version": "1.2.3",
          "status": "{{statusValue}}"
        }
        """;

        // Act.
        var metadata = JsonSerializer.Deserialize<HealthMetadata>(camelCaseJson, JsonConfiguration.DefaultOptions);

        // Assert.
        metadata.ShouldNotBeNull();
        metadata.Status.ShouldBe(expectedStatus);
    }

    [Theory]
    [InlineData(0, HealthStatus.Healthy)]
    [InlineData(1, HealthStatus.Degraded)]
    [InlineData(2, HealthStatus.Unhealthy)]
    public void Deserialize_GivenNumericStatusValues_ShouldSucceed(int statusValue, HealthStatus expectedStatus)
    {
        // Arrange.
        var camelCaseJson = $$"""
        {
          "serviceName": "Test Service",
          "version": "1.2.3",
          "status": {{statusValue}}
        }
        """;

        // Act.
        var metadata = JsonSerializer.Deserialize<HealthMetadata>(camelCaseJson, JsonConfiguration.DefaultOptions);

        // Assert.
        metadata.ShouldNotBeNull();
        metadata.Status.ShouldBe(expectedStatus);
    }

    [Fact]
    public void Deserialize_GivenPascalCaseJson_ShouldSucceed()
    {
        // Arrange.
        var pascalCaseJson = """
        {
          "ServiceName": "Payment API",
          "Version": "1.2.3",
          "Environment": "Production",
          "Status": "Healthy",
          "Timestamp": "2024-01-15T10:30:00Z",
          "Description": "All systems operational",
          "HostName": "payment-api-01",
          "Uptime": "1.02:30:00",
          "AdditionalMetadata": {
            "Database": "PostgreSQL",
            "Cache": "Redis"
          }
        }
        """;

        // Act.
        var metadata = JsonSerializer.Deserialize<HealthMetadata>(pascalCaseJson, JsonConfiguration.DefaultOptions);

        // Assert.
        metadata.ShouldNotBeNull();
        metadata.ServiceName.ShouldBe("Payment API");
        metadata.Version.ShouldBe("1.2.3");
        metadata.Environment.ShouldBe("Production");
        metadata.Status.ShouldBe(HealthStatus.Healthy);
        metadata.Description.ShouldBe("All systems operational");
        metadata.HostName.ShouldBe("payment-api-01");
        metadata.AdditionalMetadata.ShouldNotBeNull();
        metadata.AdditionalMetadata!.Count.ShouldBe(2);
        metadata.AdditionalMetadata["Database"].ShouldBe("PostgreSQL");
        metadata.AdditionalMetadata["Cache"].ShouldBe("Redis");
    }

    [Fact]
    public void Deserialize_GivenPascalCaseJsonAndNumericStatus_ShouldSucceed()
    {
        // Arrange.
        var pascalCaseJson = """
        {
          "ServiceName": "Payment API",
          "Version": "1.2.3",
          "Environment": "Production",
          "Status": 0,
          "Timestamp": "2024-01-15T10:30:00Z",
          "Description": "All systems operational",
          "HostName": "payment-api-01",
          "Uptime": "1.02:30:00",
          "AdditionalMetadata": {
            "Database": "PostgreSQL",
            "Cache": "Redis"
          }
        }
        """;

        // Act.
        var metadata = JsonSerializer.Deserialize<HealthMetadata>(pascalCaseJson, JsonConfiguration.DefaultOptions);

        // Assert.
        metadata.ShouldNotBeNull();
        metadata.ServiceName.ShouldBe("Payment API");
        metadata.Version.ShouldBe("1.2.3");
        metadata.Environment.ShouldBe("Production");
        metadata.Status.ShouldBe(HealthStatus.Healthy);
        metadata.Description.ShouldBe("All systems operational");
        metadata.HostName.ShouldBe("payment-api-01");
        metadata.AdditionalMetadata.ShouldNotBeNull();
        metadata.AdditionalMetadata!.Count.ShouldBe(2);
        metadata.AdditionalMetadata["Database"].ShouldBe("PostgreSQL");
        metadata.AdditionalMetadata["Cache"].ShouldBe("Redis");
    }

    [Fact]
    public void Deserialize_GivenMinimalCamelCaseJson_ShouldSucceed()
    {
        // Arrange.
        var minimalJson = """
        {
          "serviceName": "My API",
          "version": "1.0.0"
        }
        """;

        // Act.
        var metadata = JsonSerializer.Deserialize<HealthMetadata>(minimalJson, JsonConfiguration.DefaultOptions);

        // Assert.
        metadata.ShouldNotBeNull();
        metadata.ServiceName.ShouldBe("My API");
        metadata.Version.ShouldBe("1.0.0");
    }

    [Fact]
    public void Deserialize_GivenMixedCasing_ShouldSucceed()
    {
        // Arrange.
        var mixedJson = """
        {
          "serviceName": "Test Service",
          "Version": "2.0.0",
          "environment": "UAT",
          "Status": "Healthy",
          "Timestamp": "2024-01-15T10:30:00Z"
        }
        """;

        // Act.
        var metadata = JsonSerializer.Deserialize<HealthMetadata>(mixedJson, JsonConfiguration.DefaultOptions);

        // Assert.
        metadata.ShouldNotBeNull();
        metadata.ServiceName.ShouldBe("Test Service");
        metadata.Version.ShouldBe("2.0.0");
        metadata.Environment.ShouldBe("UAT");
        metadata.Status.ShouldBe(HealthStatus.Healthy);
    }

    [Fact]
    public void Deserialize_GivenGitBranchVersionAndStringStatus_ShouldSucceed()
    {
        // Arrange.
        var json = """
        {
          "serviceName": "User Service",
          "version": "feature/new-authentication",
          "environment": "DEV",
          "status": "Degraded",
          "description": "Testing new authentication flow",
          "additionalMetadata": {
            "commit": "abc123def456",
            "branch": "feature/new-authentication"
          }
        }
        """;

        // Act.
        var metadata = JsonSerializer.Deserialize<HealthMetadata>(json, JsonConfiguration.DefaultOptions);

        // Assert.
        metadata.ShouldNotBeNull();
        metadata.ServiceName.ShouldBe("User Service");
        metadata.Version.ShouldBe("feature/new-authentication");
        metadata.Environment.ShouldBe("DEV");
        metadata.Status.ShouldBe(HealthStatus.Degraded);
        metadata.AdditionalMetadata.ShouldNotBeNull();
        metadata.AdditionalMetadata!["commit"].ShouldBe("abc123def456");
        metadata.AdditionalMetadata["branch"].ShouldBe("feature/new-authentication");
    }
}
