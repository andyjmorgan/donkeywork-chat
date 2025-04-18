// ------------------------------------------------------
// <copyright file="MicrosoftGraphSerializerOptions.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Text.Json;
using System.Text.Json.Serialization;

namespace DonkeyWork.Chat.AiTooling.ToolImplementations.MicrosoftGraph.Common;

/// <summary>
/// A class that contains the serialization options for Microsoft Graph.
/// </summary>
public static class MicrosoftGraphSerializationOptions
{
    /// <summary>
    /// A list of property names to exclude from Microsoft Graph responses.
    /// </summary>
    private static readonly string[] ExcludedProperties = new[]
    {
        "additionalData",
        "backingStore",
        "odataType",
    };

    /// <summary>
    /// A static instance of <see cref="JsonSerializerOptions"/> for Microsoft Graph.
    /// </summary>
    public static readonly JsonSerializerOptions MicrosoftGraphJsonSerializerOptions = new ()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new MicrosoftGraphJsonConverter() },
    };

    /// <summary>
    /// Determines if a property should be excluded from serialization.
    /// </summary>
    /// <param name="propertyName">The property name.</param>
    /// <returns>True if the property should be excluded, false otherwise.</returns>
    public static bool ShouldExcludeProperty(string propertyName)
    {
        return Array.Exists(ExcludedProperties, p => 
            string.Equals(p, propertyName, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// JSON converter for Microsoft Graph objects that filters out unwanted properties.
    /// </summary>
    private class MicrosoftGraphJsonConverter : JsonConverterFactory
    {
        /// <inheritdoc/>
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert.IsClass && !typeToConvert.IsPrimitive && typeToConvert != typeof(string);
        }

        /// <inheritdoc/>
        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            var converterType = typeof(MicrosoftGraphObjectConverter<>).MakeGenericType(typeToConvert);
            return (JsonConverter)Activator.CreateInstance(converterType)!;
        }

        /// <summary>
        /// Generic converter for Microsoft Graph objects.
        /// </summary>
        /// <typeparam name="T">The type to convert.</typeparam>
        private class MicrosoftGraphObjectConverter<T> : JsonConverter<T>
        {
            /// <inheritdoc/>
            public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                // For deserialization, use the default behavior
                var newOptions = new JsonSerializerOptions(options);
                newOptions.Converters.Remove(this);
                return JsonSerializer.Deserialize<T>(ref reader, newOptions)!;
            }

            /// <inheritdoc/>
            public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
            {
                writer.WriteStartObject();

                // Get all properties of the object
                var properties = value!.GetType().GetProperties();

                // Write only non-excluded properties
                foreach (var property in properties)
                {
                    if (!ShouldExcludeProperty(property.Name))
                    {
                        var propertyValue = property.GetValue(value);
                        if (propertyValue != null || options.DefaultIgnoreCondition != JsonIgnoreCondition.WhenWritingNull)
                        {
                            var propertyName = options.PropertyNamingPolicy?.ConvertName(property.Name) ?? property.Name;
                            writer.WritePropertyName(propertyName);

                            var newOptions = new JsonSerializerOptions(options);
                            newOptions.Converters.Remove(this);

                            JsonSerializer.Serialize(writer, propertyValue, newOptions);
                        }
                    }
                }

                writer.WriteEndObject();
            }
        }
    }
}