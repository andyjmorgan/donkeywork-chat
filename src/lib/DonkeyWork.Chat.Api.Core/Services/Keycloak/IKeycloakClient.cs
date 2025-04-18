// ------------------------------------------------------
// <copyright file="IKeycloakClient.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Text.Json;
using DonkeyWork.Chat.Api.Core.Services.Keycloak.Models;

namespace DonkeyWork.Chat.Api.Core.Services.Keycloak;

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
    /// Logs out by invalidating the refresh token.
    /// </summary>
    /// <param name="refreshToken">The refresh token to invalidate.</param>
    /// <returns>True if logout was successful, otherwise false.</returns>
    Task<bool> LogoutAsync(string refreshToken);

    /// <summary>
    /// Gets the complete Keycloak logout URL that will invalidate the SSO session.
    /// </summary>
    /// <param name="redirectUri">The URI to redirect to after logout.</param>
    /// <returns>The URL to redirect to for complete Keycloak logout.</returns>
    string GetKeycloakLogoutUrl(string redirectUri);

    /// <summary>
    /// Gets a user's information by their ID.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <returns>The user's information.</returns>
    Task<KeycloakUser?> GetUserInfoByIdAsync(Guid userId);
}