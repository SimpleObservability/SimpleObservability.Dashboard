using Bogus;

namespace WorldDomination.SimpleObservability.Dashboard.Tests.ServicesTests.HealthCheckResultTests;

/// <summary>
/// Tests for the <see cref="HealthCheckResult.CheckedAt"/> property.
/// </summary>
public class CheckedAtTests
{
    private readonly Faker _faker = new();

    [Fact]
    public void CheckedAt_GivenADefaultValue_ShouldBeApproximatelyNow()
    {
        // Arrange.
        var now = DateTimeOffset.UtcNow;
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
        result.CheckedAt.ShouldBe(now, TimeSpan.FromSeconds(5));
    }
}
