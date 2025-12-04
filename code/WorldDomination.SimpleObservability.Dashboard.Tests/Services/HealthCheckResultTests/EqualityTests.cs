using WorldDomination.SimpleObservability.Dashboard.Services;

namespace WorldDomination.SimpleObservability.Dashboard.Tests.Services.HealthCheckResultTests;

/// <summary>
/// Tests for the <see cref="HealthCheckResult"/> record equality.
/// </summary>
public class EqualityTests
{
    [Fact]
    public void WithEqualValues_ShouldBeEqual()
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
    public void WithDifferentValues_ShouldNotBeEqual()
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
