using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using WorldDomination.SimpleObservability;

namespace WorldDomination.SimpleObservability.Dashboard.Services;

/// <summary>
/// Implementation of the health check service.
/// </summary>
public class HealthCheckService : IHealthCheckService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly DashboardConfiguration _configuration;
    private readonly ILogger<HealthCheckService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public HealthCheckService(
        IHttpClientFactory httpClientFactory,
        DashboardConfiguration configuration,
        ILogger<HealthCheckService> logger)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<HealthCheckResult> CheckHealthAsync(ServiceEndpoint serviceEndpoint, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(serviceEndpoint);

        if (!serviceEndpoint.Enabled)
        {
            _logger.LogDebug("Service {ServiceName} is disabled, skipping health check.", serviceEndpoint.Name);
            return new HealthCheckResult
            {
                ServiceEndpoint = serviceEndpoint,
                IsSuccess = false,
                ErrorMessage = "Service is disabled",
                IsDisabled = true
            };
        }

        var httpClient = _httpClientFactory.CreateClient("HealthCheck");
        httpClient.Timeout = TimeSpan.FromSeconds(serviceEndpoint.TimeoutSeconds ?? _configuration.TimeoutSeconds);

        try
        {
            _logger.LogDebug("Checking health for {ServiceName} at {Url}.", serviceEndpoint.Name, serviceEndpoint.HealthCheckUrl);

            var response = await httpClient.GetAsync(serviceEndpoint.HealthCheckUrl, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                
                // Check if content is empty.
                if (string.IsNullOrWhiteSpace(content))
                {
                    _logger.LogWarning("Health check for {ServiceName} returned empty response.", serviceEndpoint.Name);
                    return new HealthCheckResult
                    {
                        ServiceEndpoint = serviceEndpoint,
                        IsSuccess = false,
                        ErrorMessage = "Empty response body",
                        StatusCode = response.StatusCode
                    };
                }

                HealthMetadata? healthMetadata = null;
                
                try
                {
                    healthMetadata = JsonSerializer.Deserialize<HealthMetadata>(content, _jsonOptions);
                }
                catch (JsonException jsonEx)
                {
                    _logger.LogWarning("Health check for {ServiceName} returned invalid JSON. Error: {Error}. Content preview: {Content}",
                        serviceEndpoint.Name, jsonEx.Message, content.Length > 100 ? content.Substring(0, 100) + "..." : content);
                    
                    return new HealthCheckResult
                    {
                        ServiceEndpoint = serviceEndpoint,
                        IsSuccess = false,
                        ErrorMessage = "Invalid JSON response",
                        StatusCode = response.StatusCode
                    };
                }

                if (healthMetadata is not null)
                {
                    _logger.LogInformation("Health check succeeded for {ServiceName}. Version: {Version}, Status: {Status}.",
                        serviceEndpoint.Name, healthMetadata.Version, healthMetadata.Status);

                    return new HealthCheckResult
                    {
                        ServiceEndpoint = serviceEndpoint,
                        HealthMetadata = healthMetadata,
                        IsSuccess = true,
                        StatusCode = response.StatusCode
                    };
                }

                _logger.LogWarning("Health check for {ServiceName} returned null after deserialization.", serviceEndpoint.Name);
                return new HealthCheckResult
                {
                    ServiceEndpoint = serviceEndpoint,
                    IsSuccess = false,
                    ErrorMessage = "Invalid health metadata format",
                    StatusCode = response.StatusCode
                };
            }

            _logger.LogWarning("Health check for {ServiceName} returned status code {StatusCode}.",
                serviceEndpoint.Name, response.StatusCode);

            return new HealthCheckResult
            {
                ServiceEndpoint = serviceEndpoint,
                IsSuccess = false,
                ErrorMessage = $"HTTP {(int)response.StatusCode}",
                StatusCode = response.StatusCode
            };
        }
        catch (HttpRequestException exception)
        {
            // Extract the root cause for better error messages.
            var errorMessage = exception.InnerException is SocketException socketEx
                ? $"Connection failed: {socketEx.Message}"
                : $"HTTP Error: {exception.Message}";

            // Log DNS/connection errors as warnings, not errors.
            if (exception.InnerException is SocketException)
            {
                _logger.LogWarning("Health check failed for {ServiceName} at {Url}. {ErrorMessage}", 
                    serviceEndpoint.Name, serviceEndpoint.HealthCheckUrl, errorMessage);
            }
            else
            {
                _logger.LogError(exception, "HTTP request failed for {ServiceName}.", serviceEndpoint.Name);
            }

            return new HealthCheckResult
            {
                ServiceEndpoint = serviceEndpoint,
                IsSuccess = false,
                ErrorMessage = errorMessage
            };
        }
        catch (TaskCanceledException)
        {
            var timeoutUsed = serviceEndpoint.TimeoutSeconds ?? _configuration.TimeoutSeconds;
            _logger.LogWarning("Health check for {ServiceName} timed out after {Timeout} seconds.", 
                serviceEndpoint.Name, timeoutUsed);
            
            return new HealthCheckResult
            {
                ServiceEndpoint = serviceEndpoint,
                IsSuccess = false,
                ErrorMessage = "Request timed out"
            };
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unexpected error checking health for {ServiceName}.", serviceEndpoint.Name);
            return new HealthCheckResult
            {
                ServiceEndpoint = serviceEndpoint,
                IsSuccess = false,
                ErrorMessage = $"Error: {exception.Message}"
            };
        }
    }

    public async Task<Dictionary<string, HealthCheckResult>> CheckAllHealthAsync(CancellationToken cancellationToken = default)
    {
        var results = new Dictionary<string, HealthCheckResult>();
        var tasks = _configuration.Services
            .Select(async service =>
            {
                var result = await CheckHealthAsync(service, cancellationToken);
                return (service, result);
            });

        var completedResults = await Task.WhenAll(tasks);

        foreach (var (service, result) in completedResults)
        {
            // Use composite key: name + environment to handle multiple services with same name.
            var key = $"{service.Name}|{service.Environment}";
            results[key] = result;
        }

        return results;
    }
}
