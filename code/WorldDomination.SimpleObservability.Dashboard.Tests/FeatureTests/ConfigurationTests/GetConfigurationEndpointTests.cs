using Microsoft.AspNetCore.Mvc.Testing;

namespace WorldDomination.SimpleObservability.Dashboard.Tests.FeatureTests.ConfigurationTests;

public class GetConfigurationEndpointTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

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
}
