// ------------------------------------------------------
// <copyright file="GenericProviderConfigurationModel.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.Common.Providers.GenericProvider;

/// <summary>
/// A model representing the configuration for a generic provider.
/// </summary>
public record GenericProviderConfigurationModel
{
    /// <summary>
    /// Gets the provider type.
    /// </summary>
    public GenericProviderType ProviderType { get; init; }

    /// <summary>
    /// Gets a value indicating whether the provider is enabled.
    /// </summary>
    public bool IsEnabled { get; init; }

    /// <summary>
    /// Gets the provider properties.
    /// </summary>
    public Dictionary<string, GenericProviderPropertyModel> Properties { get; init; } = [];
}