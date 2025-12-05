namespace WorldDomination.SimpleObservability.Dashboard.Tests.ServicesTests.HealthCheckServiceTests;

/// <summary>
/// Tests for the <see cref="HealthCheckService"/> class.
/// </summary>
public class ConstructorTests
{
    private readonly Mock<IHealthHttpClient> _mockHealthHttpClient;
    private readonly Mock<ILogger<HealthCheckService>> _mockLogger;
    private readonly DashboardConfiguration _configuration;

    public ConstructorTests()
    {
        _mockHealthHttpClient = new Mock<IHealthHttpClient>();
        _mockLogger = new Mock<ILogger<HealthCheckService>>();
        
        _configuration = new DashboardConfiguration
        {
            Services = new List<ServiceEndpoint>(),
            TimeoutSeconds = 5
        };
    }

    [Fact]
    public void Constructor_WithNullHttpClient_ShouldThrowArgumentNullException()
    {
        // Arrange.
        // Act.
        var exception = Should.Throw<ArgumentNullException>(() =>
            new HealthCheckService(null!, _configuration, _mockLogger.Object));

        // Assert.
        exception.ParamName.ShouldBe("healthHttpClient");
    }

    [Fact]
    public void Constructor_WithNullConfiguration_ShouldThrowArgumentNullException()
    {
        // Arrange.
        // Act.
        var exception = Should.Throw<ArgumentNullException>(() =>
            new HealthCheckService(_mockHealthHttpClient.Object, null!, _mockLogger.Object));

        // Assert.
        exception.ParamName.ShouldBe("configuration");
    }

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Arrange.
        // Act.
        var exception = Should.Throw<ArgumentNullException>(() =>
            new HealthCheckService(_mockHealthHttpClient.Object, _configuration, null!));

        // Assert.
        exception.ParamName.ShouldBe("logger");
    }

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // Arrange.
        // Act.
        var service = new HealthCheckService(
            _mockHealthHttpClient.Object,
            _configuration,
            _mockLogger.Object);

        // Assert.
        service.ShouldNotBeNull();
    }
    
}
