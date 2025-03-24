// ------------------------------------------------------
// <copyright file="IKeycloakClient.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Text.Json;

namespace DonkeyWork.Chat.Api.Services.Keycloak;

/// <summary>
/// Interface for interacting with Keycloak authentication APIs.
/// </summary>
public interface IKeycloakClient
{
    /// <summary>
    /// Exchanges an authorization code for tokens.
    /// </summary>
    /// <param name="code">The authorization code.</param>
    /// <param name="codeVerifier">The PKCE code verifier.</param>
    /// <param name="redirectUri">The redirect URI.</param>
    /// <returns>The token response if successful, or null if failed.</returns>
    Task<JsonElement?> ExchangeCodeForTokensAsync(string code, string codeVerifier, string redirectUri);

    /// <summary>
    /// Refreshes access token using a refresh token.
    /// </summary>
    /// <param name="refreshToken">The refresh token.</param>
    /// <returns>The token response if successful, or null if failed.</returns>
    Task<JsonElement?> RefreshTokensAsync(string refreshToken);

    /// <summary>
    /// Logs out by invalidating the refresh token.
    /// </summary>
    /// <param name="refreshToken">The refresh token to invalidate.</param>
    /// <returns>True if logout was successful, otherwise false.</returns>
    Task<bool> LogoutAsync(string refreshToken);
}