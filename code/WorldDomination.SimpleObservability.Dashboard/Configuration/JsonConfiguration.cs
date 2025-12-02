using System.Text.Json;
using System.Text.Json.Serialization;

namespace WorldDomination.SimpleObservability.Dashboard.Configuration;

/// <summary>
/// Provides centralized JSON serialization options for the application.
/// </summary>
public static class JsonConfiguration
{
    /// <summary>
    /// Gets the standard JSON serializer options used throughout the application.
    /// Configured for camelCase naming, case-insensitive deserialization, and enum string conversion.
    /// </summary>
    public static JsonSerializerOptions DefaultOptions { get; } = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter() }
    };
}
