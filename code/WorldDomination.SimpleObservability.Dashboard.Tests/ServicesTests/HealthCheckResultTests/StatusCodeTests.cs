using Bogus;

namespace WorldDomination.SimpleObservability.Dashboard.Tests.ServicesTests.HealthCheckResultTests;

/// <summary>
/// Tests for the <see cref="HealthCheckResult.StatusCode"/> property.
/// </summary>
public class StatusCodeTests
{
    private readonly Faker _faker = new();

    [Theory]
    [InlineData(HttpStatusCode.OK)]
    [InlineData(HttpStatusCode.BadRequest)]
    [InlineData(HttpStatusCode.InternalServerError)]
    [InlineData(HttpStatusCode.ServiceUnavailable)]
    public void WithDifferentCodes_ShouldSetCorrectly(HttpStatusCode statusCode)
    {
        // Arrange.
        var endpoint = new ServiceEndpoint
        {
            Name = _faker.Company.CompanyName(),
            Environment = "DEV",
            HealthCheckUrl = _faker.Internet.Url()
        };

        // Act.
        var result = new HealthCheckResult
        {
            ServiceEndpoint = endpoint,
            StatusCode = statusCode
        };

        // Assert.
        result.StatusCode.ShouldBe(statusCode);
    }
}
