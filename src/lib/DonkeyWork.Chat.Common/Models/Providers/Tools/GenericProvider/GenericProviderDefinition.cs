// ------------------------------------------------------
// <copyright file="GenericProviderDefinition.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.Common.Models.Providers.Tools.GenericProvider;

/// <summary>
/// A class representing a generic provider definition.
/// </summary>
public class GenericProviderDefinition
{
    /// <summary>
    /// Gets the provider display name.
    /// </summary>
    required public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Gets the provider type.
    /// </summary>
    required public ToolProviderType Type { get; init; }

    /// <summary>
    /// Gets the provider description.
    /// </summary>
    required public string Description { get; init; }

    /// <summary>
    /// Gets a value indicating whether the provider is connected.
    /// </summary>
    required public bool IsConnected { get; init; }

    /// <summary>
    /// Gets a value indicating whether the provider is enabled.
    /// </summary>
    required public bool IsEnabled { get; init; }

    /// <summary>
    /// Gets the provider image.
    /// </summary>
    required public string Image { get; init; }

    /// <summary>
    /// Gets the tags.
    /// </summary>
    public List<string> Tags { get; init; } = [];

    /// <summary>
    /// Gets the provider capabilities.
    /// </summary>
    public Dictionary<string, string> Capabilities { get; init; } = [];
}