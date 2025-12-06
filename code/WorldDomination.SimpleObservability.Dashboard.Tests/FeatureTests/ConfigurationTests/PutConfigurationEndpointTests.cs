using Microsoft.AspNetCore.Mvc.Testing;

namespace WorldDomination.SimpleObservability.Dashboard.Tests.FeatureTests.ConfigurationTests;

public class PutConfigurationEndpointTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task PutConfigEndpoint_ShouldAcceptCamelCaseJson()
    {
        // Arrange.
        using var client = factory.CreateClient();

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
        var response = await client.PutAsync("/api/config", content, TestContext.Current.CancellationToken);

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
        using var client = factory.CreateClient();

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
        var response = await client.PutAsync("/api/config", content, TestContext.Current.CancellationToken);

        // Assert.
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        // Verify response is camelCase even when input was PascalCase.
        responseContent.ShouldContain("\"message\"");
        responseContent.ShouldContain("\"config\"");
    }
}
