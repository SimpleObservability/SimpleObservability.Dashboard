namespace WorldDomination.SimpleObservability.Dashboard.Tests.Services;

/// <summary>
/// Tests for HealthCheckService to ensure it can deserialize both camelCase and PascalCase JSON responses.
/// </summary>
public class HealthCheckServiceCamelCaseTests
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    [Fact]
    public void DeserializeHealthMetadata_WithCamelCaseJson_ShouldSucceed()
    {
        // Arrange.
        var camelCaseJson = """
        {
          "serviceName": "Payment API",
          "version": "1.2.3",
          "environment": "Production",
          "status": "healthy",
          "timestamp": "2024-01-15T10:30:00Z",
          "description": "All systems operational",
          "hostName": "payment-api-01",
          "uptime": "P1DT2H30M",
          "additionalMetadata": {
            "database": "PostgreSQL",
            "cache": "Redis"
          }
        }
        """;

        // Act.
        var metadata = JsonSerializer.Deserialize<HealthMetadata>(camelCaseJson, _jsonOptions);

        // Assert.
        Assert.NotNull(metadata);
        Assert.Equal("Payment API", metadata.ServiceName);
        Assert.Equal("1.2.3", metadata.Version);
        Assert.Equal("Production", metadata.Environment);
        Assert.Equal(HealthStatus.Healthy, metadata.Status);
        Assert.Equal("All systems operational", metadata.Description);
        Assert.Equal("payment-api-01", metadata.HostName);
        Assert.NotNull(metadata.AdditionalMetadata);
        Assert.Equal(2, metadata.AdditionalMetadata.Count);
        Assert.Equal("PostgreSQL", metadata.AdditionalMetadata["database"]);
        Assert.Equal("Redis", metadata.AdditionalMetadata["cache"]);
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
          "Uptime": "P1DT2H30M",
          "AdditionalMetadata": {
            "Database": "PostgreSQL",
            "Cache": "Redis"
          }
        }
        """;

        // Act.
        var metadata = JsonSerializer.Deserialize<HealthMetadata>(pascalCaseJson, _jsonOptions);

        // Assert.
        Assert.NotNull(metadata);
        Assert.Equal("Payment API", metadata.ServiceName);
        Assert.Equal("1.2.3", metadata.Version);
        Assert.Equal("Production", metadata.Environment);
        Assert.Equal(HealthStatus.Healthy, metadata.Status);
        Assert.Equal("All systems operational", metadata.Description);
        Assert.Equal("payment-api-01", metadata.HostName);
        Assert.NotNull(metadata.AdditionalMetadata);
        Assert.Equal(2, metadata.AdditionalMetadata.Count);
        Assert.Equal("PostgreSQL", metadata.AdditionalMetadata["Database"]);
        Assert.Equal("Redis", metadata.AdditionalMetadata["Cache"]);
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
        var metadata = JsonSerializer.Deserialize<HealthMetadata>(minimalJson, _jsonOptions);

        // Assert.
        Assert.NotNull(metadata);
        Assert.Equal("My API", metadata.ServiceName);
        Assert.Equal("1.0.0", metadata.Version);
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
        var metadata = JsonSerializer.Deserialize<HealthMetadata>(json, _jsonOptions);

        // Assert.
        Assert.NotNull(metadata);
        Assert.Equal(HealthStatus.Degraded, metadata.Status);
    }

    [Fact]
    public void DeserializeHealthStatus_WithPascalCaseString_ShouldSucceed()
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
        var metadata = JsonSerializer.Deserialize<HealthMetadata>(json, _jsonOptions);

        // Assert.
        Assert.NotNull(metadata);
        Assert.Equal(HealthStatus.Unhealthy, metadata.Status);
    }

    [Fact]
    public void DeserializeHealthStatus_WithMixedCaseString_ShouldSucceed()
    {
        // Arrange.
        var json = """
        {
          "serviceName": "Test",
          "version": "1.0",
          "status": "HEALTHY"
        }
        """;

        // Act.
        var metadata = JsonSerializer.Deserialize<HealthMetadata>(json, _jsonOptions);

        // Assert.
        Assert.NotNull(metadata);
        Assert.Equal(HealthStatus.Healthy, metadata.Status);
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
          "Status": "healthy",
          "Timestamp": "2024-01-15T10:30:00Z"
        }
        """;

        // Act.
        var metadata = JsonSerializer.Deserialize<HealthMetadata>(mixedJson, _jsonOptions);

        // Assert.
        Assert.NotNull(metadata);
        Assert.Equal("Test Service", metadata.ServiceName);
        Assert.Equal("2.0.0", metadata.Version);
        Assert.Equal("UAT", metadata.Environment);
        Assert.Equal(HealthStatus.Healthy, metadata.Status);
    }

    [Fact]
    public void DeserializeHealthMetadata_WithGitBranchVersion_ShouldSucceed()
    {
        // Arrange.
        var json = """
        {
          "serviceName": "User Service",
          "version": "feature/new-authentication",
          "environment": "DEV",
          "status": "degraded",
          "description": "Testing new authentication flow",
          "additionalMetadata": {
            "commit": "abc123def456",
            "branch": "feature/new-authentication"
          }
        }
        """;

        // Act.
        var metadata = JsonSerializer.Deserialize<HealthMetadata>(json, _jsonOptions);

        // Assert.
        Assert.NotNull(metadata);
        Assert.Equal("User Service", metadata.ServiceName);
        Assert.Equal("feature/new-authentication", metadata.Version);
        Assert.Equal("DEV", metadata.Environment);
        Assert.Equal(HealthStatus.Degraded, metadata.Status);
        Assert.NotNull(metadata.AdditionalMetadata);
        Assert.Equal("abc123def456", metadata.AdditionalMetadata["commit"]);
        Assert.Equal("feature/new-authentication", metadata.AdditionalMetadata["branch"]);
    }
}
