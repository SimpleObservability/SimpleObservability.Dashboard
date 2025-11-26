using Serilog;
using WorldDomination.SimpleObservability;
using WorldDomination.SimpleObservability.Dashboard.Services;
using WorldDomination.SimpleObservability.Dashboard.Configuration;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog.
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext());

// Add optional dashboard settings file.
builder.Configuration.AddDashboardSettingsFile();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Configure JSON serialization to use camelCase.
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

// Configure HttpClient for health checks.
builder.Services.AddHttpClient("HealthCheck")
    .ConfigureHttpClient(client =>
    {
        client.DefaultRequestHeaders.Add("User-Agent", "SimpleObservability/1.0");
    });

// Load dashboard configuration from configuration sources.
var dashboardConfig = DashboardConfigurationLoader.Load(builder.Configuration);

// Use a mutable wrapper to allow in-memory updates.
var configHolder = new ConfigurationHolder { Config = dashboardConfig };
builder.Services.AddSingleton(configHolder);

// Register a factory that always returns the current configuration from the holder.
builder.Services.AddScoped<DashboardConfiguration>(sp => 
{
    var holder = sp.GetRequiredService<ConfigurationHolder>();
    return holder.Config;
});

// Register health check service.
builder.Services.AddScoped<IHealthCheckService, HealthCheckService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Serve static files (dashboard HTML).
app.UseDefaultFiles();
app.UseStaticFiles();

// API endpoint to get all health data.
app.MapGet("/api/health", async (ConfigurationHolder configHolder, IHealthCheckService healthCheckService, CancellationToken cancellationToken) =>
{
    var results = await healthCheckService.CheckAllHealthAsync(cancellationToken);
    
    return Results.Ok(new
    {
        Environments = configHolder.Config.Environments,
        Services = configHolder.Config.Services,
        Results = results,
        RefreshIntervalSeconds = configHolder.Config.RefreshIntervalSeconds,
        Timestamp = DateTimeOffset.UtcNow
    });
})
.WithName("GetHealthStatus");

// API endpoint to get health for a specific service.
app.MapGet("/api/health/{serviceName}", async (string serviceName, ConfigurationHolder configHolder, IHealthCheckService healthCheckService, CancellationToken cancellationToken) =>
{
    var service = configHolder.Config.Services.FirstOrDefault(s => 
        s.Name.Equals(serviceName, StringComparison.OrdinalIgnoreCase));
    
    if (service is null)
    {
        return Results.NotFound(new { Error = $"Service '{serviceName}' not found" });
    }

    var result = await healthCheckService.CheckHealthAsync(service, cancellationToken);
    return Results.Ok(result);
})
.WithName("GetServiceHealth");

// API endpoint to get the current configuration.
app.MapGet("/api/config", (ConfigurationHolder configHolder, ILogger<Program> logger) =>
{
    logger.LogInformation("Returning configuration. EnvironmentOrder: {EnvironmentOrder}", 
        configHolder.Config.EnvironmentOrder != null ? string.Join(", ", configHolder.Config.EnvironmentOrder) : "null");
    
    return Results.Ok(configHolder.Config);
})
.WithName("GetConfiguration");

// API endpoint to update the dashboard configuration.
app.MapPut("/api/config", async (HttpContext context, ConfigurationHolder configHolder, ILogger<Program> logger) =>
{
    // Enable buffering to allow reading the body multiple times.
    context.Request.EnableBuffering();
    
    // Read the raw JSON to see what's actually being sent.
    using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
    var jsonBody = await reader.ReadToEndAsync();
    logger.LogInformation("Raw JSON received: {Json}", jsonBody);
    
    // Reset the stream position so it can be read again for deserialization.
    context.Request.Body.Position = 0;

    // Create JSON options that accept both camelCase and PascalCase.
    var jsonOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    // Manually deserialize to see what we get.
    var updatedConfig = JsonSerializer.Deserialize<DashboardConfiguration>(jsonBody, jsonOptions);

    if (updatedConfig is null)
    {
        return Results.BadRequest(new { Error = "Invalid configuration data" });
    }

    logger.LogInformation("Deserialized configuration. EnvironmentOrder: {EnvironmentOrder}", 
        updatedConfig.EnvironmentOrder != null ? string.Join(", ", updatedConfig.EnvironmentOrder) : "null");

    // Validation.
    if (updatedConfig.Services is null || updatedConfig.Services.Count == 0)
    {
        return Results.BadRequest(new { Error = "Services list cannot be empty" });
    }

    if (updatedConfig.TimeoutSeconds <= 0)
    {
        return Results.BadRequest(new { Error = "TimeoutSeconds must be greater than 0" });
    }

    if (updatedConfig.RefreshIntervalSeconds <= 0)
    {
        return Results.BadRequest(new { Error = "RefreshIntervalSeconds must be greater than 0" });
    }

    // Update the configuration in-memory by creating a new instance.
    configHolder.Config = updatedConfig;

    logger.LogInformation("Configuration updated. Current EnvironmentOrder: {EnvironmentOrder}", 
        configHolder.Config.EnvironmentOrder != null ? string.Join(", ", configHolder.Config.EnvironmentOrder) : "null");

    return Results.Ok(new { Message = "Configuration updated successfully (in-memory only)", Config = configHolder.Config });
})
.WithName("UpdateConfiguration");

// API endpoint to add a new service.
app.MapPost("/api/config/services", (ServiceEndpoint newService, ConfigurationHolder configHolder) =>
{
    // Validation.
    if (string.IsNullOrWhiteSpace(newService.Name))
    {
        return Results.BadRequest(new { Error = "Service name is required" });
    }

    if (string.IsNullOrWhiteSpace(newService.Environment))
    {
        return Results.BadRequest(new { Error = "Environment is required" });
    }

    if (string.IsNullOrWhiteSpace(newService.HealthCheckUrl))
    {
        return Results.BadRequest(new { Error = "Health check URL is required" });
    }

    // Check for duplicates (same name + environment).
    var exists = configHolder.Config.Services.Any(s => 
        s.Name.Equals(newService.Name, StringComparison.OrdinalIgnoreCase) && 
        s.Environment.Equals(newService.Environment, StringComparison.OrdinalIgnoreCase));

    if (exists)
    {
        return Results.Conflict(new { Error = $"Service '{newService.Name}' already exists in environment '{newService.Environment}'" });
    }

    // Create new configuration with added service.
    var updatedServices = new List<ServiceEndpoint>(configHolder.Config.Services) { newService };
    configHolder.Config = configHolder.Config with { Services = updatedServices };

    return Results.Created($"/api/config/services/{newService.Name}", new { Message = "Service added successfully", Service = newService });
})
.WithName("AddService");

// API endpoint to update a service.
app.MapPut("/api/config/services/{serviceName}/{environment}", (string serviceName, string environment, ServiceEndpoint updatedService, ConfigurationHolder configHolder) =>
{
    var service = configHolder.Config.Services.FirstOrDefault(s => 
        s.Name.Equals(serviceName, StringComparison.OrdinalIgnoreCase) && 
        s.Environment.Equals(environment, StringComparison.OrdinalIgnoreCase));

    if (service is null)
    {
        return Results.NotFound(new { Error = $"Service '{serviceName}' in environment '{environment}' not found" });
    }

    // Create new configuration with updated service.
    var updatedServices = configHolder.Config.Services
        .Where(s => !(s.Name.Equals(serviceName, StringComparison.OrdinalIgnoreCase) && 
                      s.Environment.Equals(environment, StringComparison.OrdinalIgnoreCase)))
        .Append(updatedService)
        .ToList();
    
    configHolder.Config = configHolder.Config with { Services = updatedServices };

    return Results.Ok(new { Message = "Service updated successfully", Service = updatedService });
})
.WithName("UpdateService");

// API endpoint to delete a service.
app.MapDelete("/api/config/services/{serviceName}/{environment}", (string serviceName, string environment, ConfigurationHolder configHolder) =>
{
    var service = configHolder.Config.Services.FirstOrDefault(s => 
        s.Name.Equals(serviceName, StringComparison.OrdinalIgnoreCase) && 
        s.Environment.Equals(environment, StringComparison.OrdinalIgnoreCase));

    if (service is null)
    {
        return Results.NotFound(new { Error = $"Service '{serviceName}' in environment '{environment}' not found" });
    }

    // Create new configuration without the deleted service.
    var updatedServices = configHolder.Config.Services
        .Where(s => !(s.Name.Equals(serviceName, StringComparison.OrdinalIgnoreCase) && 
                      s.Environment.Equals(environment, StringComparison.OrdinalIgnoreCase)))
        .ToList();
    
    configHolder.Config = configHolder.Config with { Services = updatedServices };

    return Results.Ok(new { Message = "Service deleted successfully" });
})
.WithName("DeleteService");

app.Run();
