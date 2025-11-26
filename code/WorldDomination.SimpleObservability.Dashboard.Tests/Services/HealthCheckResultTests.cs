using System.Net;
using Bogus;
using WorldDomination.SimpleObservability.Dashboard.Services;

namespace WorldDomination.SimpleObservability.Dashboard.Tests.Services;

/// <summary>
/// Tests for the <see cref="HealthCheckResult"/> record.
/// </summary>
public class HealthCheckResultTests
{
    private readonly Faker _faker = new();

    [Fact]
    public void Constructor_WithRequiredProperties_ShouldCreateInstance()
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
    public void Constructor_WithSuccessfulHealthCheck_ShouldCreateInstance()
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
    public void Constructor_WithFailedHealthCheck_ShouldCreateInstance()
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
    public void Constructor_WithDisabledService_ShouldCreateInstance()
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

    [Theory]
    [InlineData(HttpStatusCode.OK)]
    [InlineData(HttpStatusCode.BadRequest)]
    [InlineData(HttpStatusCode.InternalServerError)]
    [InlineData(HttpStatusCode.ServiceUnavailable)]
    public void StatusCode_WithDifferentCodes_ShouldSetCorrectly(HttpStatusCode statusCode)
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

    [Fact]
    public void CheckedAt_DefaultValue_ShouldBeApproximatelyNow()
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

    [Fact]
    public void Record_WithEqualValues_ShouldBeEqual()
    {
        // Arrange.
        var endpoint = new ServiceEndpoint
        {
            Name = "Test Service",
            Environment = "DEV",
            HealthCheckUrl = "http://localhost:5000"
        };

        var checkedAt = DateTimeOffset.UtcNow;

        var result1 = new HealthCheckResult
        {
            ServiceEndpoint = endpoint,
            IsSuccess = true,
            CheckedAt = checkedAt
        };

        var result2 = new HealthCheckResult
        {
            ServiceEndpoint = endpoint,
            IsSuccess = true,
            CheckedAt = checkedAt
        };

        // Act.
        var areEqual = result1 == result2;

        // Assert.
        areEqual.ShouldBeTrue();
    }

    [Fact]
    public void Record_WithDifferentValues_ShouldNotBeEqual()
    {
        // Arrange.
        var endpoint1 = new ServiceEndpoint
        {
            Name = "Service A",
            Environment = "DEV",
            HealthCheckUrl = "http://localhost:5000"
        };

        var endpoint2 = new ServiceEndpoint
        {
            Name = "Service B",
            Environment = "DEV",
            HealthCheckUrl = "http://localhost:5001"
        };

        var result1 = new HealthCheckResult
        {
            ServiceEndpoint = endpoint1,
            IsSuccess = true
        };

        var result2 = new HealthCheckResult
        {
            ServiceEndpoint = endpoint2,
            IsSuccess = true
        };

        // Act.
        var areEqual = result1 == result2;

        // Assert.
        areEqual.ShouldBeFalse();
    }
}
