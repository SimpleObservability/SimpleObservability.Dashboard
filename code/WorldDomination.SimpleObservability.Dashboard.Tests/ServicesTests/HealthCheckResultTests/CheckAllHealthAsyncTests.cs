namespace WorldDomination.SimpleObservability.Dashboard.Tests.ServicesTests.HealthCheckResultTests;

public class CheckAllHealthAsyncTests
{
    private readonly Mock<IHealthHttpClient> _mockHealthHttpClient;
    private readonly Mock<ILogger<HealthCheckService>> _mockLogger;
    private readonly DashboardConfiguration _configuration;

    public CheckAllHealthAsyncTests()
    {
        _mockHealthHttpClient = new Mock<IHealthHttpClient>();
        _mockLogger = new Mock<ILogger<HealthCheckService>>();

        _configuration = new DashboardConfiguration
        {
            Services = [],
            TimeoutSeconds = 5
        };
    }

    [Fact]
    public async Task CheckAllHealthAsync_WithMultipleServices_ShouldReturnAllResults()
    {
        // Arrange.
        var config = new DashboardConfiguration
        {
            Services =
            [
                new() { Name = "Service1", Environment = "DEV", HealthCheckUrl = "http://localhost:5001/healthz" },
                new() { Name = "Service2", Environment = "DEV", HealthCheckUrl = "http://localhost:5002/healthz" },
                new() { Name = "Service3", Environment = "UAT", HealthCheckUrl = "http://localhost:5003/healthz" }
            ],
            TimeoutSeconds = 5
        };

        var healthMetadata = new HealthMetadata
        {
            ServiceName = "Test",
            Version = "1.0.0"
        };

        var responseContent = JsonSerializer.Serialize(healthMetadata);

        _mockHealthHttpClient
            .Setup(h => h.GetAsync(
                It.IsAny<string>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent)
            });

        var service = new HealthCheckService(
            _mockHealthHttpClient.Object,
            config,
            _mockLogger.Object);

        // Act.
        var results = await service.CheckAllHealthAsync(TestContext.Current.CancellationToken);

        // Assert.
        results.ShouldNotBeNull();
        results.Count.ShouldBe(3);
        results.ShouldContainKey("Service1|DEV");
        results.ShouldContainKey("Service2|DEV");
        results.ShouldContainKey("Service3|UAT");
    }

    [Fact]
    public async Task CheckAllHealthAsync_WithEmptyServicesList_ShouldReturnEmptyResults()
    {
        // Arrange.
        var service = new HealthCheckService(
            _mockHealthHttpClient.Object,
            _configuration,
            _mockLogger.Object);

        // Act.
        var results = await service.CheckAllHealthAsync(TestContext.Current.CancellationToken);

        // Assert.
        results.ShouldNotBeNull();
        results.ShouldBeEmpty();
    }

    [Fact]
    public async Task CheckAllHealthAsync_WithCancellationToken_ShouldPassTokenToHealthChecks()
    {
        // Arrange.
        var config = new DashboardConfiguration
        {
            Services =
            [
                new() { Name = "Service1", Environment = "DEV", HealthCheckUrl = "http://localhost:5001/healthz" }
            ],
            TimeoutSeconds = 5
        };

        _mockHealthHttpClient
            .Setup(h => h.GetAsync(
                It.IsAny<string>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new TaskCanceledException());

        var service = new HealthCheckService(
            _mockHealthHttpClient.Object,
            config,
            _mockLogger.Object);

        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act.
        var results = await service.CheckAllHealthAsync(TestContext.Current.CancellationToken);

        // Assert.
        results.ShouldNotBeNull();
        results.Count.ShouldBe(1);
        results.Values.First().IsSuccess.ShouldBeFalse();
    }
}
