// ------------------------------------------------------
// <copyright file="UserProviderDataKeyType.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.Common.Models.Providers.Tools;

/// <summary>
/// An enumeration of user provider data key types.
/// </summary>
public enum UserProviderDataKeyType
{
    /// <summary>
    /// The provider type is unknown.
    /// </summary>
    Unknown,

    /// <summary>
    /// An access token.
    /// </summary>
    AccessToken,

    /// <summary>
    /// A refresh token.
    /// </summary>
    RefreshToken,
}