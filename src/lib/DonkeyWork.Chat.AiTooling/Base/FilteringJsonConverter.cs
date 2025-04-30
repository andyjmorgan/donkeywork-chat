// ------------------------------------------------------
// <copyright file="FilteringJsonConverter.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Text.Json;
using System.Text.Json.Serialization;

namespace DonkeyWork.Chat.AiTooling.Base;

/// <summary>
/// A JSON converter that filters out specified properties during serialization.
/// </summary>
public class FilteringJsonConverter : JsonConverter<object>
{
    private readonly string[] excludedProperties;

    /// <summary>
    /// Initializes a new instance of the <see cref="FilteringJsonConverter"/> class.
    /// </summary>
    /// <param name="excludedProperties">The excluded properties.</param>
    public FilteringJsonConverter(params string[] excludedProperties)
    {
        this.excludedProperties = excludedProperties;
    }

    /// <inheritdoc />
    public override bool CanConvert(Type typeToConvert)
    {
        // We'll handle most object types
        return true;
    }

    /// <inheritdoc />
    public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // For deserialization, we'll use the standard approach
        // Create a new options instance without this converter
        var newOptions = new JsonSerializerOptions(options);
        newOptions.Converters.Remove(this);
        return JsonSerializer.Deserialize(ref reader, typeToConvert, newOptions);
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, object? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

        var valueType = value.GetType();

        // Create a new options instance without this converter
        var newOptions = new JsonSerializerOptions(options);
        newOptions.Converters.Remove(this);

        // Handle arrays and collections
        if (value is System.Collections.IEnumerable enumerable && valueType != typeof(string))
        {
            writer.WriteStartArray();

            foreach (var item in enumerable)
            {
                if (item == null)
                {
                    writer.WriteNullValue();
                }
                else
                {
                    // Process each item with the converter removed to avoid recursion
                    JsonSerializer.Serialize(writer, item, newOptions);
                }
            }

            writer.WriteEndArray();
            return;
        }

        // Handle objects
        writer.WriteStartObject();

        var properties = valueType.GetProperties();
        foreach (var property in properties)
        {
            // Skip excluded properties
            if (this.excludedProperties.Contains(property.Name, StringComparer.OrdinalIgnoreCase))
            {
                continue;
            }

            var propertyValue = property.GetValue(value);

            // Skip null values if configured to do so
            if (propertyValue == null && options.DefaultIgnoreCondition == JsonIgnoreCondition.WhenWritingNull)
            {
                continue;
            }

            var propertyName = options.PropertyNamingPolicy?.ConvertName(property.Name) ?? property.Name;
            writer.WritePropertyName(propertyName);

            // Serialize the property value with the converter removed to avoid recursion
            JsonSerializer.Serialize(writer, propertyValue, newOptions);
        }

        writer.WriteEndObject();
    }
}