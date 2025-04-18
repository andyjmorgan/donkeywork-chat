// ------------------------------------------------------
// <copyright file="SessionTokens.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.Api.Core.Services.Keycloak.Models;

/// <summary>
/// Model for storing session tokens.
/// </summary>
public class SessionTokens
{
    /// <summary>
    /// Gets or sets the access token.
    /// </summary>
    public string? AccessToken { get; set; }

    /// <summary>
    /// Gets or sets the refresh token.
    /// </summary>
    public string? RefreshToken { get; set; }

    /// <summary>
    /// Gets or sets the token expiry timestamp.
    /// </summary>
    public DateTime ExpiresAt { get; set; }
}