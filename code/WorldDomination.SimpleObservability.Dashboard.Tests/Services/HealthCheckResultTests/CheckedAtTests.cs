using Bogus;
using WorldDomination.SimpleObservability.Dashboard.Services;

namespace WorldDomination.SimpleObservability.Dashboard.Tests.Services.HealthCheckResultTests;

/// <summary>
/// Tests for the <see cref="HealthCheckResult.CheckedAt"/> property.
/// </summary>
public class CheckedAtTests
{
    private readonly Faker _faker = new();

    [Fact]
    public void DefaultValue_ShouldBeApproximatelyNow()
    {
        // Arrange.
        var before = DateTimeOffset.UtcNow;
        var endpoint = new ServiceEndpoint
        {
            Name = _faker.Company.CompanyName(),
            Environment = "DEV",
            HealthCheckUrl = _faker.Internet.Url()
        };

        // Act.
        var result = new HealthCheckResult
        {
            ServiceEndpoint = endpoint
        };

        // Assert.
        var after = DateTimeOffset.UtcNow;
        result.CheckedAt.ShouldBeInRange(before, after);
    }
}
