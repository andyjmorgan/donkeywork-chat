// ------------------------------------------------------
// <copyright file="BaseAccessToken.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.Providers.Services.TokenRefresh.Models;

/// <summary>
/// A base class for access tokens.
/// </summary>
public class BaseAccessToken
{
    /// <summary>
    /// Gets the access token.
    /// </summary>
    required public string AccessToken { get; init; }

    /// <summary>
    /// Gets the refresh token.
    /// </summary>
    required public string RefreshToken { get; init; }

    /// <summary>
    /// Gets the expiry.
    /// </summary>
    required public DateTimeOffset? ExpiresOn { get; init; }

    /// <summary>
    /// Gets the token scopes.
    /// </summary>
    required public string[] Scopes { get; init; }
}