// ------------------------------------------------------
// <copyright file="UserProviderType.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.Common.Providers;

/// <summary>
/// An oauth token provider type.
/// </summary>
public enum UserProviderType
{
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
}