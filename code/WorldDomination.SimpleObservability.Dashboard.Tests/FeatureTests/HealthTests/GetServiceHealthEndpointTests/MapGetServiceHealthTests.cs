using Bogus;
using Microsoft.AspNetCore.Mvc.Testing;
using WorldDomination.SimpleObservability.Dashboard.Configuration;

namespace WorldDomination.SimpleObservability.Dashboard.Tests.FeatureTests.HealthTests.GetServiceHealthEndpointTests;

public class MapGetServiceHealthTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly Faker _faker = new();

    [Fact]
    public async Task MapGetServiceHealth_WithValidServiceName_ShouldReturnOk()
    {
        // Arrange.
        using var client = factory.CreateClient();

        // Act.
        var response = await client.GetAsync("/api/health/Google", TestContext.Current.CancellationToken);

        // Assert.
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        content.ShouldNotBeNullOrEmpty();

        var result = JsonSerializer.Deserialize<HealthCheckResult>(content, JsonConfiguration.DefaultOptions);
        result.ShouldNotBeNull();
        result.ServiceEndpoint.ShouldNotBeNull();
        result.ServiceEndpoint.Name.ShouldBe("Google");
    }

    [Fact]
    public async Task MapGetServiceHealth_WithNonExistentServiceName_ShouldReturnNotFound()
    {
        // Arrange.
        using var client = factory.CreateClient();
        var nonExistentService = _faker.Random.AlphaNumeric(20);

        // Act.
        var response = await client.GetAsync($"/api/health/{nonExistentService}", TestContext.Current.CancellationToken);

        // Assert.
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);

        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        content.ShouldContain("not found");
        content.ShouldContain(nonExistentService);
    }

    [Fact]
    public async Task MapGetServiceHealth_WithDifferentCasing_ShouldReturnOk()
    {
        // Arrange.
        using var client = factory.CreateClient();

        // Act.
        var response = await client.GetAsync("/api/health/google", TestContext.Current.CancellationToken);

        // Assert.
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        content.ShouldNotBeNullOrEmpty();

        var result = JsonSerializer.Deserialize<HealthCheckResult>(content, JsonConfiguration.DefaultOptions);
        result.ShouldNotBeNull();
        result.ServiceEndpoint.ShouldNotBeNull();
        result.ServiceEndpoint.Name.ShouldBe("Google", StringCompareShould.IgnoreCase);
    }

    [Fact]
    public async Task MapGetServiceHealth_WithSuccessfulHealthCheck_ShouldReturnCamelCaseJson()
    {
        // Arrange.
        using var client = factory.CreateClient();

        // Act.
        var response = await client.GetAsync("/api/health/Google", TestContext.Current.CancellationToken);

        // Assert.
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        content.ShouldContain("\"serviceEndpoint\"");
        content.ShouldContain("\"isSuccess\"");
        content.ShouldContain("\"statusCode\"");
        content.ShouldContain("\"checkedAt\"");

        content.IndexOf("\"ServiceEndpoint\"", StringComparison.Ordinal).ShouldBe(-1);
        content.IndexOf("\"IsSuccess\"", StringComparison.Ordinal).ShouldBe(-1);
        content.IndexOf("\"StatusCode\"", StringComparison.Ordinal).ShouldBe(-1);
        content.IndexOf("\"CheckedAt\"", StringComparison.Ordinal).ShouldBe(-1);
    }

    [Fact]
    public async Task MapGetServiceHealth_WithValidServiceName_ShouldReturnHealthCheckResult()
    {
        // Arrange.
        using var client = factory.CreateClient();

        // Act.
        var response = await client.GetAsync("/api/health/Google", TestContext.Current.CancellationToken);

        // Assert.
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        var result = JsonSerializer.Deserialize<HealthCheckResult>(content, JsonConfiguration.DefaultOptions);

        result.ShouldNotBeNull();
        result.ServiceEndpoint.ShouldNotBeNull();
        result.ServiceEndpoint.Name.ShouldBe("Google");
        result.CheckedAt.ShouldBeInRange(DateTimeOffset.UtcNow.AddMinutes(-1), DateTimeOffset.UtcNow.AddMinutes(1));
    }

    [Fact]
    public async Task MapGetServiceHealth_WithNotFoundError_ShouldContainErrorProperty()
    {
        // Arrange.
        using var client = factory.CreateClient();
        var nonExistentService = _faker.Random.AlphaNumeric(20);

        // Act.
        var response = await client.GetAsync($"/api/health/{nonExistentService}", TestContext.Current.CancellationToken);

        // Assert.
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);

        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        content.ShouldContain("\"error\"");
    }
}
