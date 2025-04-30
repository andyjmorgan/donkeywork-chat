// ------------------------------------------------------
// <copyright file="UserProviderPosture.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Models.Providers.Tools;

namespace DonkeyWork.Chat.Common.Models.Providers.Posture;

/// <summary>
/// A user provider posture.
/// </summary>
public record UserProviderPosture
{
    /// <summary>
    /// Gets the provider type.
    /// </summary>
    public ToolProviderType ProviderType { get; init; }

    /// <summary>
    /// Gets the scopes.
    /// </summary>
    public List<string> Scopes { get; init; } = [];

    /// <summary>
    /// Gets the users keys.
    /// </summary>
    public Dictionary<UserProviderDataKeyType, string> Keys { get; init; } = [];
}