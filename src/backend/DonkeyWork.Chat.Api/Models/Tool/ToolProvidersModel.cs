// ------------------------------------------------------
// <copyright file="ToolProvidersModel.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Models.Providers.Tools;

namespace DonkeyWork.Chat.Api.Models.Tool;

/// <summary>
/// A model for tool providers.
/// </summary>
public record ToolProvidersModel
{
    /// <summary>
    /// Gets or sets the provider type.
    /// </summary>
    public ToolProviderType ProviderType { get; set; }

    /// <summary>
    /// Gets or sets the authorization type.
    /// </summary>
    public ToolProviderAuthorizationType AuthorizationType { get; set; }

    /// <summary>
    /// Gets the provider name.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Gets the provider description.
    /// </summary>
    public string Description { get; init; } = string.Empty;

    /// <summary>
    /// Gets the provider icon.
    /// </summary>
    public string Icon { get; init; } = string.Empty;

    /// <summary>
    /// Gets a value indicating whether the provider is enabled.
    /// </summary>
    public bool IsConnected { get; init; } = false;

    /// <summary>
    /// Gets the Application definitions.
    /// </summary>
    public Dictionary<ToolProviderApplicationType, ToolProviderApplicationDefinition> Applications { get; init; } = [];
}