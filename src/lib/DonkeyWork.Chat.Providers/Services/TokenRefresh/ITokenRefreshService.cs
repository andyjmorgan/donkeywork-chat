// ------------------------------------------------------
// <copyright file="ITokenRefreshService.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Providers.Services.TokenRefresh.Models;

namespace DonkeyWork.Chat.Providers.Services.TokenRefresh;

/// <summary>
/// An interface for a token refresh service.
/// </summary>
public interface ITokenRefreshService
{
    /// <summary>
    /// Refreshes a provider access token.
    /// </summary>
    /// <param name="refreshToken">The refresh token.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task<BaseAccessToken> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
}