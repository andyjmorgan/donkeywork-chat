// ------------------------------------------------------
// <copyright file="ToolProviderType.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.Common.Models.Providers.Tools;

/// <summary>
/// An oauth token provider type.
/// </summary>
public enum ToolProviderType
{
    /// <summary>
    /// An unknown provider.
    /// </summary>
    Unknown,

    /// <summary>
    /// A microsoft provider.
    /// </summary>
    Microsoft,

    /// <summary>
    /// A discord provider.
    /// </summary>
    Discord,

    /// <summary>
    /// A google provider.
    /// </summary>
    Google,

    /// <summary>
    /// A swarmpit provider.
    /// </summary>
    Swarmpit,

    /// <summary>
    /// A serp provider.
    /// </summary>
    Serp,

    /// <summary>
    /// A builtin provider.
    /// </summary>
    BuiltIn,
}
