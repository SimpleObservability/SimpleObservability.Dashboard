using System.Text.Json;
using System.Text.Json.Serialization;
using WorldDomination.SimpleObservability;

namespace SimpleObservability.SampleService.WebApi;

/// <summary>
/// Custom JSON converter for HealthStatus enum that provides user-friendly error messages.
/// </summary>
public class HealthStatusJsonConverter : JsonConverter<HealthStatus?>
{
    public override HealthStatus? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Handle null values.
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException("Invalid value for 'Status' property. Acceptable values are 'Healthy', 'Degraded', or 'Unhealthy'.");
        }

        var value = reader.GetString();

        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        // Try to parse the enum value (case-insensitive).
        if (Enum.TryParse<HealthStatus>(value, ignoreCase: true, out var result))
        {
            return result;
        }

        // Provide a helpful error message with valid options.
        throw new JsonException($"Invalid value for 'Status' property: '{value}'. Acceptable values are 'Healthy', 'Degraded', or 'Unhealthy'.");
    }

    public override void Write(Utf8JsonWriter writer, HealthStatus? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
        {
            writer.WriteStringValue(value.Value.ToString());
        }
        else
        {
            writer.WriteNullValue();
        }
    }
}
