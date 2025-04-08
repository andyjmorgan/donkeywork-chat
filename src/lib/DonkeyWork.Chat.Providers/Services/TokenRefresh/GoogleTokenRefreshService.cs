// ------------------------------------------------------
// <copyright file="GoogleTokenRefreshService.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Providers.Provider.Implementation.Google.Client;
using DonkeyWork.Chat.Providers.Services.TokenRefresh.Models;

namespace DonkeyWork.Chat.Providers.Services.TokenRefresh;

/// <inheritdoc cref="ITokenRefreshService"/>
public class GoogleTokenRefreshService : ITokenRefreshService
{
    /// <summary>
    /// Gets the token client.
    /// </summary>
    private readonly IGoogleOAuthTokenClient tokenClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="GoogleTokenRefreshService"/> class.
    /// </summary>
    /// <param name="tokenClient">The token client.</param>
    public GoogleTokenRefreshService(IGoogleOAuthTokenClient tokenClient)
    {
        this.tokenClient = tokenClient;
    }

    /// <inheritdoc />
    public async Task<BaseAccessToken> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var newToken = await this.tokenClient.RefreshTokenAsync(refreshToken, cancellationToken);
        return new BaseAccessToken()
        {
            AccessToken = newToken.AccessToken,
            RefreshToken = newToken.RefreshToken ?? string.Empty,
            Scopes = newToken.Scopes,
            ExpiresOn = newToken.ExpiresOn,
        };
    }
}