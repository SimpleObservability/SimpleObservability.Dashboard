using Microsoft.AspNetCore.Mvc.Testing;

namespace WorldDomination.SimpleObservability.Dashboard.Tests.FeatureTests.ConfigurationTests;

public class PostConfigurationEndpointTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

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
}
