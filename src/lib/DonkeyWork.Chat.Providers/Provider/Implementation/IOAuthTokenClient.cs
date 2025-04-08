// ------------------------------------------------------
// <copyright file="IOAuthTokenClient.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.Providers.Provider.Implementation;

/// <summary>
/// A generic interface for OAuth token clients.
/// </summary>
/// <typeparam name="TTokenResponse">The token response type specific to the provider.</typeparam>
public interface IOAuthTokenClient<TTokenResponse>
    where TTokenResponse : class
{
    /// <summary>
    /// Exchanges an authorization code for a token.
    /// </summary>
    /// <param name="code">The authorization code.</param>
    /// <param name="redirectUri">The redirect URI.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The token response.</returns>
    Task<TTokenResponse> ExchangeCodeForTokenAsync(
        string code,
        string redirectUri,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Refreshes an existing token using a refresh token.
    /// </summary>
    /// <param name="refreshToken">The refresh token.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The token response.</returns>
    Task<TTokenResponse> RefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default);
}