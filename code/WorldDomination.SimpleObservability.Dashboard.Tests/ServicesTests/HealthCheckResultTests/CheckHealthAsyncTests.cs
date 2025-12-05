using Bogus;

namespace WorldDomination.SimpleObservability.Dashboard.Tests.ServicesTests.HealthCheckResultTests;

public class CheckHealthAsyncTests
{
    private readonly Faker _faker = new();
    private readonly Mock<IHealthHttpClient> _mockHealthHttpClient;
    private readonly Mock<ILogger<HealthCheckService>> _mockLogger;
    private readonly DashboardConfiguration _configuration;

    public CheckHealthAsyncTests()
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
    public async Task CheckHealthAsync_WithNullServiceEndpoint_ShouldThrowArgumentNullException()
    {
        // Arrange.
        var service = new HealthCheckService(
            _mockHealthHttpClient.Object,
            _configuration,
            _mockLogger.Object);

        // Act.
        var exception = await Should.ThrowAsync<ArgumentNullException>(async () =>
            await service.CheckHealthAsync(null!, TestContext.Current.CancellationToken));

        // Assert.
        exception.ParamName.ShouldBe("serviceEndpoint");
    }

    [Fact]
    public async Task CheckHealthAsync_WithDisabledService_ShouldReturnDisabledResult()
    {
        // Arrange.
        var endpoint = new ServiceEndpoint
        {
            Name = _faker.Company.CompanyName(),
            Environment = "DEV",
            HealthCheckUrl = _faker.Internet.Url(),
            Enabled = false
        };

        var service = new HealthCheckService(
            _mockHealthHttpClient.Object,
            _configuration,
            _mockLogger.Object);

        // Act.
        var result = await service.CheckHealthAsync(endpoint, TestContext.Current.CancellationToken);

        // Assert.
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeFalse();
        result.IsDisabled.ShouldBeTrue();
        result.ErrorMessage.ShouldBe("Service is disabled");
        result.ServiceEndpoint.ShouldBe(endpoint);
    }

    [Fact]
    public async Task CheckHealthAsync_WithSuccessfulResponse_ShouldReturnSuccessResult()
    {
        // Arrange.
        var endpoint = new ServiceEndpoint
        {
            Name = _faker.Company.CompanyName(),
            Environment = "DEV",
            HealthCheckUrl = "http://localhost:5000/healthz"
        };

        var healthMetadata = new HealthMetadata
        {
            ServiceName = endpoint.Name,
            Version = "1.0.0",
            Status = HealthStatus.Healthy
        };

        var responseContent = JsonSerializer.Serialize(healthMetadata);

        _mockHealthHttpClient
            .Setup(h => h.GetAsync(
                endpoint.HealthCheckUrl,
                It.IsAny<TimeSpan>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent)
            });

        var service = new HealthCheckService(
            _mockHealthHttpClient.Object,
            _configuration,
            _mockLogger.Object);

        // Act.
        var result = await service.CheckHealthAsync(endpoint, TestContext.Current.CancellationToken);

        // Assert.
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.HealthMetadata.ShouldNotBeNull();
        result.HealthMetadata!.ServiceName.ShouldBe(endpoint.Name);
        result.HealthMetadata.Version.ShouldBe("1.0.0");
        result.HealthMetadata.Status.ShouldBe(HealthStatus.Healthy);
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.ErrorMessage.ShouldBeNull();
    }

    [Fact]
    public async Task CheckHealthAsync_WithEmptyResponse_ShouldReturnFailureResult()
    {
        // Arrange.
        var endpoint = new ServiceEndpoint
        {
            Name = _faker.Company.CompanyName(),
            Environment = "DEV",
            HealthCheckUrl = "http://localhost:5000/healthz"
        };

        _mockHealthHttpClient
            .Setup(h => h.GetAsync(
                endpoint.HealthCheckUrl,
                It.IsAny<TimeSpan>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(string.Empty)
            });

        var service = new HealthCheckService(
            _mockHealthHttpClient.Object,
            _configuration,
            _mockLogger.Object);

        // Act.
        var result = await service.CheckHealthAsync(endpoint, TestContext.Current.CancellationToken);

        // Assert.
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeFalse();
        result.ErrorMessage.ShouldBe("Empty response body");
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CheckHealthAsync_WithInvalidJson_ShouldReturnFailureResult()
    {
        // Arrange.
        var endpoint = new ServiceEndpoint
        {
            Name = _faker.Company.CompanyName(),
            Environment = "DEV",
            HealthCheckUrl = "http://localhost:5000/healthz"
        };

        _mockHealthHttpClient
            .Setup(h => h.GetAsync(
                endpoint.HealthCheckUrl,
                It.IsAny<TimeSpan>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{ invalid json }")
            });

        var service = new HealthCheckService(
            _mockHealthHttpClient.Object,
            _configuration,
            _mockLogger.Object);

        // Act.
        var result = await service.CheckHealthAsync(endpoint, TestContext.Current.CancellationToken);

        // Assert.
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeFalse();
        result.ErrorMessage.ShouldBe("Invalid JSON response");
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Theory]
    [InlineData(HttpStatusCode.BadRequest)]
    [InlineData(HttpStatusCode.InternalServerError)]
    [InlineData(HttpStatusCode.ServiceUnavailable)]
    public async Task CheckHealthAsync_WithNonSuccessStatusCode_ShouldReturnFailureResult(HttpStatusCode statusCode)
    {
        // Arrange.
        var endpoint = new ServiceEndpoint
        {
            Name = _faker.Company.CompanyName(),
            Environment = "DEV",
            HealthCheckUrl = "http://localhost:5000/healthz"
        };

        _mockHealthHttpClient
            .Setup(h => h.GetAsync(
                endpoint.HealthCheckUrl,
                It.IsAny<TimeSpan>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent("Error")
            });

        var service = new HealthCheckService(
            _mockHealthHttpClient.Object,
            _configuration,
            _mockLogger.Object);

        // Act.
        var result = await service.CheckHealthAsync(endpoint, TestContext.Current.CancellationToken);

        // Assert.
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeFalse();
        result.ErrorMessage.ShouldBe($"HTTP {(int)statusCode}");
        result.StatusCode.ShouldBe(statusCode);
    }

    [Fact]
    public async Task CheckHealthAsync_WithHttpRequestException_ShouldReturnFailureResult()
    {
        // Arrange.
        var endpoint = new ServiceEndpoint
        {
            Name = _faker.Company.CompanyName(),
            Environment = "DEV",
            HealthCheckUrl = "http://localhost:5000/healthz"
        };

        _mockHealthHttpClient
            .Setup(h => h.GetAsync(
                endpoint.HealthCheckUrl,
                It.IsAny<TimeSpan>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("Connection failed"));

        var service = new HealthCheckService(
            _mockHealthHttpClient.Object,
            _configuration,
            _mockLogger.Object);

        // Act.
        var result = await service.CheckHealthAsync(endpoint, TestContext.Current.CancellationToken);

        // Assert.
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeFalse();
        result.ErrorMessage.ShouldNotBeNull();
        result.ErrorMessage.ShouldContain("HTTP Error");
        result.StatusCode.ShouldBeNull();
    }

    [Fact]
    public async Task CheckHealthAsync_WithTimeout_ShouldReturnFailureResult()
    {
        // Arrange.
        var endpoint = new ServiceEndpoint
        {
            Name = _faker.Company.CompanyName(),
            Environment = "DEV",
            HealthCheckUrl = "http://localhost:5000/healthz"
        };

        _mockHealthHttpClient
            .Setup(h => h.GetAsync(
                endpoint.HealthCheckUrl,
                It.IsAny<TimeSpan>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new TaskCanceledException("Request timed out"));

        var service = new HealthCheckService(
            _mockHealthHttpClient.Object,
            _configuration,
            _mockLogger.Object);

        // Act.
        var result = await service.CheckHealthAsync(endpoint, TestContext.Current.CancellationToken);

        // Assert.
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeFalse();
        result.ErrorMessage.ShouldBe("Request timed out");
    }

    [Fact]
    public async Task CheckHealthAsync_WithServiceSpecificTimeout_ShouldUseServiceTimeout()
    {
        // Arrange.
        var endpoint = new ServiceEndpoint
        {
            Name = _faker.Company.CompanyName(),
            Environment = "DEV",
            HealthCheckUrl = "http://localhost:5000/healthz",
            TimeoutSeconds = 15 // Override the default 5 seconds.
        };

        var healthMetadata = new HealthMetadata
        {
            ServiceName = endpoint.Name,
            Version = "1.0.0",
            Status = HealthStatus.Healthy
        };

        var responseContent = JsonSerializer.Serialize(healthMetadata);
        TimeSpan? capturedTimeout = null;

        _mockHealthHttpClient
            .Setup(h => h.GetAsync(
                endpoint.HealthCheckUrl,
                It.IsAny<TimeSpan>(),
                It.IsAny<CancellationToken>()))
            .Callback<string, TimeSpan, CancellationToken>((_, timeout, _) => capturedTimeout = timeout)
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent)
            });

        var service = new HealthCheckService(
            _mockHealthHttpClient.Object,
            _configuration,
            _mockLogger.Object);

        // Act.
        var result = await service.CheckHealthAsync(endpoint, TestContext.Current.CancellationToken);

        // Assert.
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        capturedTimeout.ShouldNotBeNull();
        capturedTimeout!.Value.ShouldBe(TimeSpan.FromSeconds(15));
    }

    [Fact]
    public async Task CheckHealthAsync_WithoutServiceSpecificTimeout_ShouldUseDefaultTimeout()
    {
        // Arrange.
        var endpoint = new ServiceEndpoint
        {
            Name = _faker.Company.CompanyName(),
            Environment = "DEV",
            HealthCheckUrl = "http://localhost:5000/healthz"
            // No TimeoutSeconds specified - should use default.
        };

        var healthMetadata = new HealthMetadata
        {
            ServiceName = endpoint.Name,
            Version = "1.0.0",
            Status = HealthStatus.Healthy
        };

        var responseContent = JsonSerializer.Serialize(healthMetadata);
        TimeSpan? capturedTimeout = null;

        _mockHealthHttpClient
            .Setup(h => h.GetAsync(
                endpoint.HealthCheckUrl,
                It.IsAny<TimeSpan>(),
                It.IsAny<CancellationToken>()))
            .Callback<string, TimeSpan, CancellationToken>((_, timeout, _) => capturedTimeout = timeout)
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent)
            });

        var service = new HealthCheckService(
            _mockHealthHttpClient.Object,
            _configuration,
            _mockLogger.Object);

        // Act.
        var result = await service.CheckHealthAsync(endpoint, TestContext.Current.CancellationToken);

        // Assert.
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        capturedTimeout.ShouldNotBeNull();
        capturedTimeout!.Value.ShouldBe(TimeSpan.FromSeconds(5)); // Default from _configuration.
    }

    [Fact]
    public async Task CheckHealthAsync_WithAdditionalMetadata_ShouldIncludeInResult()
    {
        // Arrange.
        var endpoint = new ServiceEndpoint
        {
            Name = _faker.Company.CompanyName(),
            Environment = "DEV",
            HealthCheckUrl = "http://localhost:5000/healthz"
        };

        var healthMetadata = new HealthMetadata
        {
            ServiceName = endpoint.Name,
            Version = "1.0.0",
            Status = HealthStatus.Healthy,
            AdditionalMetadata = new Dictionary<string, string>
            {
                ["Database"] = "Connected",
                ["Cache"] = "Redis v7.0",
                ["HostName"] = "server-01",
                ["Region"] = "us-east-1"
            }
        };

        var responseContent = JsonSerializer.Serialize(healthMetadata);

        _mockHealthHttpClient
            .Setup(h => h.GetAsync(
                endpoint.HealthCheckUrl,
                It.IsAny<TimeSpan>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent)
            });

        var service = new HealthCheckService(
            _mockHealthHttpClient.Object,
            _configuration,
            _mockLogger.Object);

        // Act.
        var result = await service.CheckHealthAsync(endpoint, TestContext.Current.CancellationToken);

        // Assert.
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.HealthMetadata.ShouldNotBeNull();
        result.HealthMetadata!.AdditionalMetadata.ShouldNotBeNull();
        result.HealthMetadata.AdditionalMetadata!.Count.ShouldBe(4);
        result.HealthMetadata.AdditionalMetadata["Database"].ShouldBe("Connected");
        result.HealthMetadata.AdditionalMetadata["Cache"].ShouldBe("Redis v7.0");
        result.HealthMetadata.AdditionalMetadata["HostName"].ShouldBe("server-01");
        result.HealthMetadata.AdditionalMetadata["Region"].ShouldBe("us-east-1");
    }
}
