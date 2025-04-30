// ------------------------------------------------------
// <copyright file="ToolProviderAuthorizationType.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.Common.Models.Providers.Tools;

/// <summary>
/// An enumeration of tool provider authorization types.
/// </summary>
public enum ToolProviderAuthorizationType
{
    /// <summary>
    /// A tool provider that uses oauth.
    /// </summary>
    OAuth,

    /// <summary>
    /// A tool provider that uses static credentials or api keys.
    /// </summary>
    Static,
}