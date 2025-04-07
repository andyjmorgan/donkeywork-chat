// ------------------------------------------------------
// <copyright file="IDiscordOAuthTokenClient.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Providers.Provider.Implementation.Discord.Models;

namespace DonkeyWork.Chat.Providers.Provider.Implementation.Discord.Client;

/// <summary>
/// An interface for Discord OAuth token client.
/// </summary>
public interface IDiscordOAuthTokenClient
{
    /// <summary>
    /// Exchanges an authorization code for a token.
    /// </summary>
    /// <param name="code">The authorization code.</param>
    /// <param name="redirectUri">The redirect URI.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The token response.</returns>
    Task<DiscordTokenResponse> ExchangeCodeForTokenAsync(
        string code,
        string redirectUri,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Refreshes an existing token using a refresh token.
    /// </summary>
    /// <param name="refreshToken">The refresh token.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The token response.</returns>
    Task<DiscordTokenResponse> RefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default);
}