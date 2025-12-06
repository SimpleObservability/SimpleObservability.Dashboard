using Microsoft.AspNetCore.Mvc.Testing;
using WorldDomination.SimpleObservability.Dashboard.Configuration;

namespace WorldDomination.SimpleObservability.Dashboard.Tests.FeatureTests.ConfigurationTests;

public class GetConfigurationEndpointTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task GetConfigEndpoint_ShouldReturnCamelCaseJson()
    {
        // Arrange.
        using var client = factory.CreateClient();

        // Act.
        var response = await client.GetAsync("/api/config", TestContext.Current.CancellationToken);

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
    public async Task GetConfigEndpoint_WithEmptyServices_ShouldReturnDefaultConfiguration()
    {
        // Arrange.
        using var client = factory.CreateClient();

        // Act.
        var response = await client.GetAsync("/api/config", TestContext.Current.CancellationToken);

        // Assert.
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        var config = JsonSerializer.Deserialize<DashboardConfiguration>(content, JsonConfiguration.DefaultOptions);

        config.ShouldNotBeNull();
        config.Services.ShouldNotBeNull();
        config.RefreshIntervalSeconds.ShouldBeGreaterThan(0);
        config.TimeoutSeconds.ShouldBeGreaterThan(0);
    }
}
