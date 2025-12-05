using Microsoft.AspNetCore.Mvc.Testing;

namespace WorldDomination.SimpleObservability.Dashboard.Tests.FeatureTests.HealthTests;

public class GetAllHealthEndpointTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

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
}
