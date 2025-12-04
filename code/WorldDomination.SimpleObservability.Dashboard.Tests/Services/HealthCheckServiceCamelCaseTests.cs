using WorldDomination.SimpleObservability.Dashboard.Configuration;

namespace WorldDomination.SimpleObservability.Dashboard.Tests.Services;

/// <summary>
/// Tests for HealthCheckService to ensure it can deserialize both camelCase and PascalCase JSON responses.
/// </summary>
public class HealthCheckServiceCamelCaseTests
{
    [Fact]
    public void DeserializeHealthMetadata_WithCamelCaseJson_ShouldSucceed()
    {
        // Arrange.
        var camelCaseJson = """
        {
          "serviceName": "Payment API",
          "version": "1.2.3",
          "environment": "Production",
          "status": "Healthy",
          "timestamp": "2024-01-15T10:30:00Z",
          "description": "All systems operational",
          "hostName": "payment-api-01",
          "uptime": "1.02:30:00",
          "additionalMetadata": {
            "database": "PostgreSQL",
            "cache": "Redis"
          }
        }
        """;

        // Act.
        var metadata = JsonSerializer.Deserialize<HealthMetadata>(camelCaseJson, JsonConfiguration.DefaultOptions);

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
        metadata.AdditionalMetadata["database"].ShouldBe("PostgreSQL");
        metadata.AdditionalMetadata["cache"].ShouldBe("Redis");
    }

    [Fact]
    public void DeserializeHealthMetadata_WithCamelCaseJsonAndNumericStatus_ShouldSucceed()
    {
        // Arrange.
        var camelCaseJson = """
        {
          "serviceName": "Payment API",
          "version": "1.2.3",
          "environment": "Production",
          "status": 0,
          "timestamp": "2024-01-15T10:30:00Z",
          "description": "All systems operational",
          "hostName": "payment-api-01",
          "uptime": "1.02:30:00",
          "additionalMetadata": {
            "database": "PostgreSQL",
            "cache": "Redis"
          }
        }
        """;

        // Act.
        var metadata = JsonSerializer.Deserialize<HealthMetadata>(camelCaseJson, JsonConfiguration.DefaultOptions);

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
        metadata.AdditionalMetadata["database"].ShouldBe("PostgreSQL");
        metadata.AdditionalMetadata["cache"].ShouldBe("Redis");
    }

    [Fact]
    public void DeserializeHealthMetadata_WithPascalCaseJson_ShouldSucceed()
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
    public void DeserializeHealthMetadata_WithPascalCaseJsonAndNumericStatus_ShouldSucceed()
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
    public void DeserializeHealthMetadata_WithMinimalCamelCaseJson_ShouldSucceed()
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
    public void DeserializeHealthStatus_WithPascalCaseString_ShouldSucceed()
    {
        // Arrange.
        var json = """
        {
          "serviceName": "Test",
          "version": "1.0",
          "status": "Degraded"
        }
        """;

        // Act.
        var metadata = JsonSerializer.Deserialize<HealthMetadata>(json, JsonConfiguration.DefaultOptions);

        // Assert.
        metadata.ShouldNotBeNull();
        metadata.Status.ShouldBe(HealthStatus.Degraded);
    }

    [Fact]
    public void DeserializeHealthStatus_WithCamelCaseString_ShouldSucceed()
    {
        // Arrange.
        var json = """
        {
          "serviceName": "Test",
          "version": "1.0",
          "status": "degraded"
        }
        """;

        // Act.
        var metadata = JsonSerializer.Deserialize<HealthMetadata>(json, JsonConfiguration.DefaultOptions);

        // Assert.
        metadata.ShouldNotBeNull();
        metadata.Status.ShouldBe(HealthStatus.Degraded);
    }

    [Fact]
    public void DeserializeHealthStatus_WithNumericValue_ShouldSucceed()
    {
        // Arrange.
        var json = """
        {
          "serviceName": "Test",
          "version": "1.0",
          "status": 1
        }
        """;

        // Act.
        var metadata = JsonSerializer.Deserialize<HealthMetadata>(json, JsonConfiguration.DefaultOptions);

        // Assert.
        metadata.ShouldNotBeNull();
        metadata.Status.ShouldBe(HealthStatus.Degraded);
    }

    [Fact]
    public void DeserializeHealthStatus_WithUnhealthyString_ShouldSucceed()
    {
        // Arrange.
        var json = """
        {
          "ServiceName": "Test",
          "Version": "1.0",
          "Status": "Unhealthy"
        }
        """;

        // Act.
        var metadata = JsonSerializer.Deserialize<HealthMetadata>(json, JsonConfiguration.DefaultOptions);

        // Assert.
        metadata.ShouldNotBeNull();
        metadata.Status.ShouldBe(HealthStatus.Unhealthy);
    }

    [Fact]
    public void DeserializeHealthStatus_WithUnhealthyNumeric_ShouldSucceed()
    {
        // Arrange.
        var json = """
        {
          "ServiceName": "Test",
          "Version": "1.0",
          "Status": 2
        }
        """;

        // Act.
        var metadata = JsonSerializer.Deserialize<HealthMetadata>(json, JsonConfiguration.DefaultOptions);

        // Assert.
        metadata.ShouldNotBeNull();
        metadata.Status.ShouldBe(HealthStatus.Unhealthy);
    }

    [Theory]
    [InlineData("Healthy", HealthStatus.Healthy)]
    [InlineData("healthy", HealthStatus.Healthy)]
    [InlineData("Degraded", HealthStatus.Degraded)]
    [InlineData("degraded", HealthStatus.Degraded)]
    [InlineData("Unhealthy", HealthStatus.Unhealthy)]
    [InlineData("unhealthy", HealthStatus.Unhealthy)]
    public void DeserializeHealthStatus_WithVariousCasings_ShouldSucceed(string statusValue, HealthStatus expectedStatus)
    {
        // Arrange.
        var json = $$"""
        {
          "serviceName": "Test",
          "version": "1.0",
          "status": "{{statusValue}}"
        }
        """;

        // Act.
        var metadata = JsonSerializer.Deserialize<HealthMetadata>(json, JsonConfiguration.DefaultOptions);

        // Assert.
        metadata.ShouldNotBeNull();
        metadata.Status.ShouldBe(expectedStatus);
    }

    [Fact]
    public void DeserializeHealthMetadata_WithMixedCasing_ShouldSucceed()
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
    public void DeserializeHealthMetadata_WithMixedCasingAndNumericStatus_ShouldSucceed()
    {
        // Arrange.
        var mixedJson = """
        {
          "serviceName": "Test Service",
          "Version": "2.0.0",
          "environment": "UAT",
          "Status": 0,
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
    public void DeserializeHealthMetadata_WithGitBranchVersionAndStringStatus_ShouldSucceed()
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

    [Fact]
    public void DeserializeHealthMetadata_WithGitBranchVersionAndNumericStatus_ShouldSucceed()
    {
        // Arrange.
        var json = """
        {
          "serviceName": "User Service",
          "version": "feature/new-authentication",
          "environment": "DEV",
          "status": 1,
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
