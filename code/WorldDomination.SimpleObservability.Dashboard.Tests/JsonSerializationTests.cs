using Microsoft.AspNetCore.Mvc.Testing;
using WorldDomination.SimpleObservability.Dashboard.Configuration;

namespace WorldDomination.SimpleObservability.Dashboard.Tests;

/// <summary>
/// Tests for JSON serialization to ensure all API responses use camelCase.
/// </summary>
public class JsonSerializationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

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
        content.ShouldContain("\"environments\"");
        content.ShouldContain("\"services\"");
        content.ShouldContain("\"results\"");
        content.ShouldContain("\"refreshIntervalSeconds\"");
        content.ShouldContain("\"timestamp\"");
        
        // Verify PascalCase property names are NOT present (case-sensitive check).
        content.IndexOf("\"Environments\"", StringComparison.Ordinal).ShouldBe(-1);
        content.IndexOf("\"Services\"", StringComparison.Ordinal).ShouldBe(-1);
        content.IndexOf("\"Results\"", StringComparison.Ordinal).ShouldBe(-1);
        content.IndexOf("\"RefreshIntervalSeconds\"", StringComparison.Ordinal).ShouldBe(-1);
        content.IndexOf("\"Timestamp\"", StringComparison.Ordinal).ShouldBe(-1);
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
        content.ShouldContain("\"services\"");
        content.ShouldContain("\"timeoutSeconds\"");
        content.ShouldContain("\"refreshIntervalSeconds\"");
        
        // Verify PascalCase property names are NOT present (case-sensitive check).
        content.IndexOf("\"Services\"", StringComparison.Ordinal).ShouldBe(-1);
        content.IndexOf("\"TimeoutSeconds\"", StringComparison.Ordinal).ShouldBe(-1);
        content.IndexOf("\"RefreshIntervalSeconds\"", StringComparison.Ordinal).ShouldBe(-1);
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
        responseContent.ShouldContain("\"message\"");
        responseContent.ShouldContain("\"config\"");
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
        responseContent.ShouldContain("\"message\"");
        responseContent.ShouldContain("\"config\"");
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
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.Created);
        
        var responseContent = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        
        // Verify response is camelCase.
        responseContent.ShouldContain("\"message\"");
        responseContent.ShouldContain("\"service\"");
    }

    [Fact]
    public void HealthMetadataDeserialization_WithNumericStatus_ShouldHandleCamelCaseJson()
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
    public void HealthMetadataDeserialization_WithStringStatus_ShouldHandleCamelCaseJson()
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
    public void HealthMetadataDeserialization_WithVariousStatusStrings_ShouldSucceed(string statusValue, HealthStatus expectedStatus)
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
    public void HealthMetadataDeserialization_WithNumericStatusValues_ShouldSucceed(int statusValue, HealthStatus expectedStatus)
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
    public void HealthMetadataSerialization_ShouldProduceCamelCaseJson()
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

    [Fact]
    public void HealthStatusEnum_ShouldSerializeAsString()
    {
        // Arrange.
        var metadata = new HealthMetadata
        {
            ServiceName = "Test",
            Version = "1.0",
            Status = HealthStatus.Degraded
        };

        // Act.
        var json = JsonSerializer.Serialize(metadata, JsonConfiguration.DefaultOptions);

        // Assert.
        // JsonStringEnumConverter writes enum values as their string names.
        json.Replace(" ", "").ShouldContain("\"status\":\"Degraded\"");
        json.Replace(" ", "").IndexOf("\"status\":1", StringComparison.Ordinal).ShouldBe(-1);
    }
}
