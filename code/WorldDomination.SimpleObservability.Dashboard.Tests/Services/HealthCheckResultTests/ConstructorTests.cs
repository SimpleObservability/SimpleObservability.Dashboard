using System.Net;
using Bogus;
using WorldDomination.SimpleObservability.Dashboard.Services;

namespace WorldDomination.SimpleObservability.Dashboard.Tests.Services.HealthCheckResultTests;

/// <summary>
/// Tests for the <see cref="HealthCheckResult"/> constructor.
/// </summary>
public class ConstructorTests
{
    private readonly Faker _faker = new();

    [Fact]
    public void WithRequiredProperties_ShouldCreateInstance()
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
            ServiceEndpoint = endpoint
        };

        // Assert.
        result.ServiceEndpoint.ShouldBe(endpoint);
        result.IsSuccess.ShouldBeFalse();
        result.HealthMetadata.ShouldBeNull();
        result.ErrorMessage.ShouldBeNull();
        result.StatusCode.ShouldBeNull();
        result.CheckedAt.ShouldBeInRange(DateTimeOffset.UtcNow.AddSeconds(-5), DateTimeOffset.UtcNow.AddSeconds(1));
        result.IsDisabled.ShouldBeFalse();
    }

    [Fact]
    public void WithSuccessfulHealthCheck_ShouldCreateInstance()
    {
        // Arrange.
        var endpoint = new ServiceEndpoint
        {
            Name = _faker.Company.CompanyName(),
            Environment = "DEV",
            HealthCheckUrl = _faker.Internet.Url()
        };

        var healthMetadata = new HealthMetadata
        {
            ServiceName = endpoint.Name,
            Version = _faker.System.Version().ToString()
        };

        // Act.
        var result = new HealthCheckResult
        {
            ServiceEndpoint = endpoint,
            IsSuccess = true,
            HealthMetadata = healthMetadata,
            StatusCode = HttpStatusCode.OK
        };

        // Assert.
        result.ServiceEndpoint.ShouldBe(endpoint);
        result.IsSuccess.ShouldBeTrue();
        result.HealthMetadata.ShouldBe(healthMetadata);
        result.ErrorMessage.ShouldBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.IsDisabled.ShouldBeFalse();
    }

    [Fact]
    public void WithFailedHealthCheck_ShouldCreateInstance()
    {
        // Arrange.
        var endpoint = new ServiceEndpoint
        {
            Name = _faker.Company.CompanyName(),
            Environment = "DEV",
            HealthCheckUrl = _faker.Internet.Url()
        };

        var errorMessage = _faker.Lorem.Sentence();

        // Act.
        var result = new HealthCheckResult
        {
            ServiceEndpoint = endpoint,
            IsSuccess = false,
            ErrorMessage = errorMessage,
            StatusCode = HttpStatusCode.ServiceUnavailable
        };

        // Assert.
        result.ServiceEndpoint.ShouldBe(endpoint);
        result.IsSuccess.ShouldBeFalse();
        result.HealthMetadata.ShouldBeNull();
        result.ErrorMessage.ShouldBe(errorMessage);
        result.StatusCode.ShouldBe(HttpStatusCode.ServiceUnavailable);
    }

    [Fact]
    public void WithDisabledService_ShouldCreateInstance()
    {
        // Arrange.
        var endpoint = new ServiceEndpoint
        {
            Name = _faker.Company.CompanyName(),
            Environment = "DEV",
            HealthCheckUrl = _faker.Internet.Url(),
            Enabled = false
        };

        // Act.
        var result = new HealthCheckResult
        {
            ServiceEndpoint = endpoint,
            IsSuccess = false,
            ErrorMessage = "Service is disabled",
            IsDisabled = true
        };

        // Assert.
        result.ServiceEndpoint.ShouldBe(endpoint);
        result.IsSuccess.ShouldBeFalse();
        result.ErrorMessage.ShouldBe("Service is disabled");
        result.IsDisabled.ShouldBeTrue();
    }
}
