// ------------------------------------------------------
// <copyright file="IMicrosoftOAuthTokenClient.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Providers.Provider.Implementation.Microsoft.Models;

namespace DonkeyWork.Chat.Providers.Provider.Implementation.Microsoft.Client;

/// <summary>
/// An interface for Microsoft OAuth token client.
/// </summary>
public interface IMicrosoftOAuthTokenClient
{
    /// <summary>
    /// Exchanges an authorization code for a token.
    /// </summary>
    /// <param name="code">The authorization code.</param>
    /// <param name="redirectUri">The redirect URI.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The token response.</returns>
    Task<MicrosoftTokenResponse> ExchangeCodeForTokenAsync(
        string code,
        string redirectUri,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Refreshes an existing token using a refresh token.
    /// </summary>
    /// <param name="refreshToken">The refresh token.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The token response.</returns>
    Task<MicrosoftTokenResponse> RefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default);
}