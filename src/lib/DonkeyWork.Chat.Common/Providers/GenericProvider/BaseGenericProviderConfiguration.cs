// ------------------------------------------------------
// <copyright file="BaseGenericProviderConfiguration.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;
using DonkeyWork.Chat.Common.Providers.GenericProvider.Implementations;

namespace DonkeyWork.Chat.Common.Providers.GenericProvider;

/// <summary>
/// A base class for generic provider configurations.
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(SwarmpitConfiguration), typeDiscriminator: nameof(SwarmpitConfiguration))]
public abstract class BaseGenericProviderConfiguration
{
    /// <summary>
    /// A dictionary of all generic provider configurations.
    /// </summary>
    private static Dictionary<GenericProviderType, BaseGenericProviderConfiguration> configurationByProviderTypeMap =
        new Dictionary<GenericProviderType, BaseGenericProviderConfiguration>()
        {
            { GenericProviderType.Swarmpit, new SwarmpitConfiguration() },
        };

    /// <summary>
    /// Gets or sets a value indicating whether the configuration is enabled.
    /// </summary>
    public bool Enabled { get; set; } = true;

    private static JsonSerializerOptions JsonSerializerOptions { get; } = new ()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.Never,
    };

    /// <summary>
    /// Deserializes a JSON string to a configuration object.
    /// </summary>
    /// <param name="json">The json string.</param>
    /// <returns>A concrete implementation of the class.</returns>
    public static BaseGenericProviderConfiguration? FromJson(string json)
    {
        return JsonSerializer.Deserialize<BaseGenericProviderConfiguration>(json, JsonSerializerOptions);
    }

    /// <summary>
    /// Gets the configuration by provider type.
    /// </summary>
    /// <param name="providerType">The provider type.</param>
    /// <returns>A <see cref="BaseGenericProviderConfiguration"/>.</returns>
    public static BaseGenericProviderConfiguration GetConfigurationByProviderType(GenericProviderType providerType)
    {
        return configurationByProviderTypeMap[providerType];
    }

    /// <summary>
    /// All generic provider configurations must implement this method to convert their properties to a dictionary.
    /// </summary>
    /// <returns>A Dictionary of configuration.</returns>
    public abstract GenericProviderConfigurationModel ToGenericProviderConfiguration();

    /// <summary>
    /// All generic provider configurations must implement this method to convert their properties to a dictionary.
    /// </summary>
    /// <param name="configurationModel">The configuration model.</param>
    /// <returns>A concrete implementation from the configuration.</returns>
    public abstract BaseGenericProviderConfiguration FromGenericProviderConfiguration(
        GenericProviderConfigurationModel configurationModel);

    /// <summary>
    /// Serializes the configuration to a JSON string.
    /// </summary>
    /// <returns>A json string.</returns>
    public string ToJson()
    {
        return JsonSerializer.Serialize(this, JsonSerializerOptions);
    }

    /// <summary>
    /// All generic provider configurations must implement this method to convert their properties to a dictionary.
    /// </summary>
    /// <param name="providerType">The provider type.</param>
    /// <returns>A <see cref="GenericProviderPropertyModel"/>.</returns>
    protected GenericProviderConfigurationModel CreateGenericProviderConfiguration(GenericProviderType providerType)
    {
        var properties = this.GetType()
            .GetProperties()
            .Where(prop => prop.DeclaringType == this.GetType())
            .ToDictionary(
                prop => prop.Name,
                prop =>
                {
                    var descriptionAttribute = prop.GetCustomAttributes(typeof(DescriptionAttribute), false)
                        .FirstOrDefault() as DescriptionAttribute;
                    var requiredAttribute = prop.GetCustomAttributes(typeof(RequiredAttribute), false)
                        .FirstOrDefault() as RequiredAttribute;
                    var passwordAttribute = prop.GetCustomAttributes(typeof(PasswordPropertyTextAttribute), false)
                        .FirstOrDefault() as PasswordPropertyTextAttribute;

                    return new GenericProviderPropertyModel()
                    {
                        Key = prop.Name,
                        FriendlyName = descriptionAttribute?.Description ?? prop.Name,
                        Type = prop.PropertyType == typeof(string)
                            ? (passwordAttribute != null ? GenericProviderPropertyType.Secret : GenericProviderPropertyType.String)
                            : GenericProviderPropertyType.Secret,
                        Value = prop.GetValue(this)?.ToString(),
                        Required = requiredAttribute != null,
                    };
                });

        return new GenericProviderConfigurationModel()
        {
            IsEnabled = this.Enabled,
            ProviderType = providerType,
            Properties = properties,
        };
    }
}