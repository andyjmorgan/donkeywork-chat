// ------------------------------------------------------
// <copyright file="IOAuthTokenRefreshHandler.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Providers.Models;

namespace DonkeyWork.Chat.Providers.TokenManager;

/// <summary>
/// Interface for OAuth token refresh handling.
/// </summary>
public interface IOAuthTokenRefreshHandler
{
    /// <summary>
    /// Refreshes an OAuth token using the refresh token.
    /// </summary>
    /// <param name="refreshToken">The refresh token to use.</param>
    /// <param name="userId">The user ID associated with the token.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A refreshed token result.</returns>
    Task<OAuthTokenResult> RefreshTokenAsync(string refreshToken, string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Determines if a token is valid or needs to be refreshed.
    /// </summary>
    /// <param name="token">The token to check.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>True if the token is valid, false if it needs refreshing.</returns>
    bool IsTokenValid(OAuthTokenResult token, CancellationToken cancellationToken = default);
}