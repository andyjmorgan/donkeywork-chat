// ------------------------------------------------------
// <copyright file="GenericProviderPropertyModel.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.Common.Models.Providers.GenericProvider;

/// <summary>
/// A model representing a property of a generic provider.
/// </summary>
public record GenericProviderPropertyModel
{
    /// <summary>
    /// Gets the property key.
    /// </summary>
    required public string Key { get; init; }

    /// <summary>
    /// Gets the friendly name.
    /// </summary>
    required public string FriendlyName { get; init; }

    /// <summary>
    /// Gets the property type.
    /// </summary>
    public GenericProviderPropertyType Type { get; init; }

    /// <summary>
    /// Gets the value of the property.
    /// </summary>
    public object? Value { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether the property is required.
    /// </summary>
    public bool Required { get; init; }
}