using SimpleObservability.SampleService.WebApi;
using WorldDomination.SimpleObservability;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Configure JSON serialization to use camelCase.
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

// Load health metadata from configuration.
var healthMetadata = new HealthMetadata
{
    ServiceName = builder.Configuration["HealthMetadata:ServiceName"] ?? "Sample Service",
    Version = builder.Configuration["HealthMetadata:Version"] ?? "1.0.0",
    Environment = builder.Configuration["HealthMetadata:Environment"] ?? "Development",
    Description = builder.Configuration["HealthMetadata:Description"] ?? "Sample service for testing Simple Observability",
    HostName = System.Net.Dns.GetHostName()
};

// Track current health status (defaults to Healthy).
var currentStatus = HealthStatus.Healthy;
var currentServiceName = healthMetadata.ServiceName;
var currentVersion = healthMetadata.Version;
var currentEnvironment = healthMetadata.Environment;
var currentDescription = healthMetadata.Description;
var currentHostName = healthMetadata.HostName;
var startTime = DateTimeOffset.UtcNow;

var app = builder.Build();

// Health check endpoint - returns the standard health metadata schema.
app.MapGet("/healthz", () =>
{
    var uptime = DateTimeOffset.UtcNow - startTime;
    
    var response = new HealthMetadata
    {
        ServiceName = currentServiceName,
        Version = currentVersion,
        Environment = currentEnvironment,
        Status = currentStatus,
        Timestamp = DateTimeOffset.UtcNow,
        Uptime = uptime,
        Description = currentDescription,
        HostName = currentHostName,
        AdditionalMetadata = new Dictionary<string, string>
        {
            ["Runtime"] = ".NET 10",
            ["MachineName"] = System.Environment.MachineName,
            ["ProcessorCount"] = System.Environment.ProcessorCount.ToString()
        }
    };

    return Results.Ok(response);
})
.WithName("HealthCheck")
.Produces<HealthMetadata>(StatusCodes.Status200OK);

// Endpoint to update health metadata (for testing).
app.MapPut("/health", (UpdateHealthMetadataRequest request) =>
{
    // Validate status if provided.
    if (request.Status.HasValue && !Enum.IsDefined(typeof(HealthStatus), request.Status.Value))
    {
        return Results.BadRequest(new { Error = "Invalid status. Use 'Healthy', 'Degraded', or 'Unhealthy'." });
    }

    // Update only the properties that were provided.
    if (request.Status.HasValue)
    {
        currentStatus = request.Status.Value;
    }

    if (!string.IsNullOrWhiteSpace(request.ServiceName))
    {
        currentServiceName = request.ServiceName;
    }

    if (!string.IsNullOrWhiteSpace(request.Version))
    {
        currentVersion = request.Version;
    }

    if (request.Environment is not null)
    {
        currentEnvironment = request.Environment;
    }

    if (request.Description is not null)
    {
        currentDescription = request.Description;
    }

    if (request.HostName is not null)
    {
        currentHostName = request.HostName;
    }

    // Build response showing what was updated.
    var updatedFields = new List<string>();
    if (request.Status.HasValue) updatedFields.Add($"Status={currentStatus}");
    if (request.ServiceName is not null) updatedFields.Add($"ServiceName={currentServiceName}");
    if (request.Version is not null) updatedFields.Add($"Version={currentVersion}");
    if (request.Environment is not null) updatedFields.Add($"Environment={currentEnvironment}");
    if (request.Description is not null) updatedFields.Add($"Description={currentDescription}");
    if (request.HostName is not null) updatedFields.Add($"HostName={currentHostName}");
    
    return Results.Ok(new 
    { 
        Message = $"Health metadata updated: {string.Join(", ", updatedFields)}",
        CurrentMetadata = new
        {
            Status = currentStatus,
            ServiceName = currentServiceName,
            Version = currentVersion,
            Environment = currentEnvironment,
            Description = currentDescription,
            HostName = currentHostName
        }
    });
})
.WithName("UpdateHealthMetadata")
.Produces(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest);

// Simple hello world endpoint.
app.MapGet("/", () => 
{
    return Results.Ok(new 
    { 
        Message = "Hello from Sample Service!",
        Service = currentServiceName,
        Version = currentVersion,
        Environment = currentEnvironment,
        CurrentStatus = currentStatus.ToString()
    });
})
.WithName("HelloWorld");

app.Run();
