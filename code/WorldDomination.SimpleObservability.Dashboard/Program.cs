using Serilog;
using WorldDomination.SimpleObservability;
using WorldDomination.SimpleObservability.Dashboard.Services;
using WorldDomination.SimpleObservability.Dashboard.Configuration;
using WorldDomination.SimpleObservability.Dashboard.Features;

// Configure Serilog early for startup logging (instance-based, not static).
using var bootstrapLogger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Configure Serilog.
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

    // Add optional dashboard settings file.
    var environmentName = builder.Environment.IsDevelopment()
        ? builder.Environment.EnvironmentName // Dev: we can use a sample hardcoded default settings file.
        : null; // Not-Development means we don't have any hardcoded default settings file.
    builder.Configuration.AddDashboardSettingsFile(environmentName: environmentName);

    // Add services to the container.
    // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
    builder.Services.AddOpenApi();

    // Configure JSON serialization to use centralized options.
    builder.Services.ConfigureHttpJsonOptions(options =>
    {
        options.SerializerOptions.PropertyNamingPolicy = JsonConfiguration.DefaultOptions.PropertyNamingPolicy;
        options.SerializerOptions.PropertyNameCaseInsensitive = JsonConfiguration.DefaultOptions.PropertyNameCaseInsensitive;
        options.SerializerOptions.DefaultIgnoreCondition = JsonConfiguration.DefaultOptions.DefaultIgnoreCondition;

        foreach (var converter in JsonConfiguration.DefaultOptions.Converters)
        {
            options.SerializerOptions.Converters.Add(converter);
        }
    });

    // Configure HttpClient for health checks.
    builder.Services.AddHttpClient<HealthHttpClient>()
        .ConfigureHttpClient(client =>
        {
            client.DefaultRequestHeaders.Add("User-Agent", "SimpleObservability/1.0");
        });
    builder.Services.AddScoped<IHealthHttpClient, HealthHttpClient>();

    // Load dashboard configuration from configuration sources.
    var dashboardConfig = DashboardConfigurationLoader.Load(builder.Configuration);

    // Use a mutable wrapper to allow in-memory updates.
    var configHolder = new ConfigurationHolder { Config = dashboardConfig };
    builder.Services.AddSingleton(configHolder);

    // Register a factory that always returns the current configuration from the holder.
    builder.Services.AddScoped(serviceProvider =>
    {
        var holder = serviceProvider.GetRequiredService<ConfigurationHolder>();
        return holder.Config;
    });

    // Register health check service.
    builder.Services.AddScoped<IHealthCheckService, HealthCheckService>();

    var app = builder.Build();

    // Add Serilog request logging middleware.
    app.UseSerilogRequestLogging();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    // Serve static files (dashboard HTML).
    app.UseDefaultFiles();
    app.UseStaticFiles();

    // Map all endpoints.
    app.MapAllEndpoints();

    bootstrapLogger.Information("Starting SimpleObservability Dashboard");

    app.Run();
}
catch (Exception exception)
{
    bootstrapLogger.Fatal(exception, "Application terminated unexpectedly");
}
