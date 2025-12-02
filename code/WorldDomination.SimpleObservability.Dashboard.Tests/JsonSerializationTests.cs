using Microsoft.AspNetCore.Mvc.Testing;
using System.Text.Json.Serialization;

namespace WorldDomination.SimpleObservability.Dashboard.Tests;

/// <summary>
/// Tests for JSON serialization to ensure all API responses use camelCase.
/// </summary>
public class JsonSerializationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    private static readonly JsonSerializerOptions _caseInsensitiveOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private static readonly JsonSerializerOptions _camelCaseOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private static readonly JsonSerializerOptions _camelCaseWithEnumOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    public JsonSerializationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetHealthEndpoint_ShouldReturnCamelCaseJson()
    {
        // Arrange.
        // (The test factory will use in-memory configuration)

        // Act.
        var response = await _client.GetAsync("/api/health", TestContext.Current.CancellationToken);

        // Assert.
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        
        // Verify camelCase property names are present.
        Assert.Contains("\"environments\"", content);
        Assert.Contains("\"services\"", content);
        Assert.Contains("\"results\"", content);
        Assert.Contains("\"refreshIntervalSeconds\"", content);
        Assert.Contains("\"timestamp\"", content);
        
        // Verify PascalCase property names are NOT present.
        Assert.DoesNotContain("\"Environments\"", content);
        Assert.DoesNotContain("\"Services\"", content);
        Assert.DoesNotContain("\"Results\"", content);
        Assert.DoesNotContain("\"RefreshIntervalSeconds\"", content);
        Assert.DoesNotContain("\"Timestamp\"", content);
    }

    [Fact]
    public async Task GetConfigEndpoint_ShouldReturnCamelCaseJson()
    {
        // Arrange.
        // (The test factory will use in-memory configuration)

        // Act.
        var response = await _client.GetAsync("/api/config", TestContext.Current.CancellationToken);

        // Assert.
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        
        // Verify camelCase property names are present.
        Assert.Contains("\"services\"", content);
        Assert.Contains("\"timeoutSeconds\"", content);
        Assert.Contains("\"refreshIntervalSeconds\"", content);
        
        // Verify PascalCase property names are NOT present.
        Assert.DoesNotContain("\"Services\"", content);
        Assert.DoesNotContain("\"TimeoutSeconds\"", content);
        Assert.DoesNotContain("\"RefreshIntervalSeconds\"", content);
    }

    [Fact]
    public async Task PutConfigEndpoint_ShouldAcceptCamelCaseJson()
    {
        // Arrange.
        var configJson = """
        {
          "services": [
            {
              "name": "Test Service",
              "environment": "TEST",
              "healthCheckUrl": "http://test.local/healthz",
              "enabled": true,
              "timeoutSeconds": 10
            }
          ],
          "timeoutSeconds": 5,
          "refreshIntervalSeconds": 30,
          "environmentOrder": ["TEST"]
        }
        """;

        var content = new StringContent(configJson, System.Text.Encoding.UTF8, "application/json");

        // Act.
        var response = await _client.PutAsync("/api/config", content, TestContext.Current.CancellationToken);

        // Assert.
        response.EnsureSuccessStatusCode();
        
        var responseContent = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        
        // Verify response is also camelCase.
        Assert.Contains("\"message\"", responseContent);
        Assert.Contains("\"config\"", responseContent);
    }

    [Fact]
    public async Task PutConfigEndpoint_ShouldAcceptPascalCaseJson()
    {
        // Arrange.
        var configJson = """
        {
          "Services": [
            {
              "Name": "Test Service",
              "Environment": "TEST",
              "HealthCheckUrl": "http://test.local/healthz",
              "Enabled": true,
              "TimeoutSeconds": 10
            }
          ],
          "TimeoutSeconds": 5,
          "RefreshIntervalSeconds": 30,
          "EnvironmentOrder": ["TEST"]
        }
        """;

        var content = new StringContent(configJson, System.Text.Encoding.UTF8, "application/json");

        // Act.
        var response = await _client.PutAsync("/api/config", content, TestContext.Current.CancellationToken);

        // Assert.
        response.EnsureSuccessStatusCode();
        
        var responseContent = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        
        // Verify response is camelCase even when input was PascalCase.
        Assert.Contains("\"message\"", responseContent);
        Assert.Contains("\"config\"", responseContent);
    }

    [Fact]
    public async Task PostServiceEndpoint_ShouldAcceptCamelCaseJson()
    {
        // Arrange.
        var serviceJson = """
        {
          "name": "New Test Service",
          "environment": "DEV",
          "healthCheckUrl": "http://newtest.local/healthz",
          "description": "A test service",
          "enabled": true,
          "timeoutSeconds": 15
        }
        """;

        var content = new StringContent(serviceJson, System.Text.Encoding.UTF8, "application/json");

        // Act.
        var response = await _client.PostAsync("/api/config/services", content, TestContext.Current.CancellationToken);

        // Assert.
        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
        
        var responseContent = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        
        // Verify response is camelCase.
        Assert.Contains("\"message\"", responseContent);
        Assert.Contains("\"service\"", responseContent);
    }

    [Fact]
    public async Task HealthMetadataDeserialization_WithNumericStatus_ShouldHandleCamelCaseJson()
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
        var metadata = JsonSerializer.Deserialize<HealthMetadata>(camelCaseJson, _caseInsensitiveOptions);

        // Assert.
        Assert.NotNull(metadata);
        Assert.Equal("Test Service", metadata.ServiceName);
        Assert.Equal("1.2.3", metadata.Version);
        Assert.Equal("Production", metadata.Environment);
        Assert.Equal(HealthStatus.Healthy, metadata.Status);
        Assert.Equal("All systems operational", metadata.Description);
        Assert.Equal("test-server-01", metadata.HostName);
        Assert.NotNull(metadata.AdditionalMetadata);
        Assert.Equal("Connected", metadata.AdditionalMetadata["database"]);
        Assert.Equal("Redis", metadata.AdditionalMetadata["cache"]);
    }

    [Fact]
    public async Task HealthMetadataDeserialization_WithStringStatus_ShouldHandleCamelCaseJson()
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
        var metadata = JsonSerializer.Deserialize<HealthMetadata>(camelCaseJson, _caseInsensitiveOptions);

        // Assert.
        Assert.NotNull(metadata);
        Assert.Equal("Test Service", metadata.ServiceName);
        Assert.Equal("1.2.3", metadata.Version);
        Assert.Equal("Production", metadata.Environment);
        Assert.Equal(HealthStatus.Healthy, metadata.Status);
        Assert.Equal("All systems operational", metadata.Description);
        Assert.Equal("test-server-01", metadata.HostName);
        Assert.NotNull(metadata.AdditionalMetadata);
        Assert.Equal("Connected", metadata.AdditionalMetadata["database"]);
        Assert.Equal("Redis", metadata.AdditionalMetadata["cache"]);
    }

    [Theory]
    [InlineData("Healthy", HealthStatus.Healthy)]
    [InlineData("healthy", HealthStatus.Healthy)]
    [InlineData("HEALTHY", HealthStatus.Healthy)]
    [InlineData("Degraded", HealthStatus.Degraded)]
    [InlineData("degraded", HealthStatus.Degraded)]
    [InlineData("Unhealthy", HealthStatus.Unhealthy)]
    [InlineData("unhealthy", HealthStatus.Unhealthy)]
    public async Task HealthMetadataDeserialization_WithVariousStatusStrings_ShouldSucceed(string statusValue, HealthStatus expectedStatus)
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
        var metadata = JsonSerializer.Deserialize<HealthMetadata>(camelCaseJson, _caseInsensitiveOptions);

        // Assert.
        Assert.NotNull(metadata);
        Assert.Equal(expectedStatus, metadata.Status);
    }

    [Theory]
    [InlineData(0, HealthStatus.Healthy)]
    [InlineData(1, HealthStatus.Degraded)]
    [InlineData(2, HealthStatus.Unhealthy)]
    public async Task HealthMetadataDeserialization_WithNumericStatusValues_ShouldSucceed(int statusValue, HealthStatus expectedStatus)
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
        var metadata = JsonSerializer.Deserialize<HealthMetadata>(camelCaseJson, _caseInsensitiveOptions);

        // Assert.
        Assert.NotNull(metadata);
        Assert.Equal(expectedStatus, metadata.Status);
    }

    [Fact]
    public async Task HealthMetadataSerialization_ShouldProduceCamelCaseJson()
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
        var json = JsonSerializer.Serialize(metadata, _camelCaseOptions);

        // Assert.
        Assert.Contains("\"serviceName\"", json);
        Assert.Contains("\"version\"", json);
        Assert.Contains("\"environment\"", json);
        Assert.Contains("\"status\"", json);
        Assert.Contains("\"description\"", json);
        Assert.Contains("\"hostName\"", json);
        Assert.Contains("\"uptime\"", json);
        Assert.Contains("\"additionalMetadata\"", json);
        
        // Verify PascalCase is NOT present.
        Assert.DoesNotContain("\"ServiceName\"", json);
        Assert.DoesNotContain("\"Version\"", json);
        Assert.DoesNotContain("\"AdditionalMetadata\"", json);
    }

    [Fact]
    public async Task HealthStatusEnum_ShouldSerializeAsString()
    {
        // Arrange.
        var metadata = new HealthMetadata
        {
            ServiceName = "Test",
            Version = "1.0",
            Status = HealthStatus.Degraded
        };

        // Act.
        var json = JsonSerializer.Serialize(metadata, _camelCaseOptions);

        // Assert.
        // Our custom converter writes PascalCase enum values for clarity and readability.
        Assert.Contains("\"status\":\"Degraded\"", json.Replace(" ", ""));
        Assert.DoesNotContain("\"status\":1", json.Replace(" ", ""));
    }
}
