// ------------------------------------------------------
// <copyright file="MicrosoftTokenRefreshService.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Providers.Provider.Implementation.Microsoft.Client;
using DonkeyWork.Chat.Providers.Services.TokenRefresh.Models;

namespace DonkeyWork.Chat.Providers.Services.TokenRefresh;

/// <inheritdoc cref="ITokenRefreshService"/>
public class MicrosoftTokenRefreshService : ITokenRefreshService
{
    /// <summary>
    /// Gets the token client.
    /// </summary>
    private readonly IMicrosoftOAuthTokenClient tokenClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="MicrosoftTokenRefreshService"/> class.
    /// </summary>
    /// <param name="tokenClient">The token client.</param>
    public MicrosoftTokenRefreshService(IMicrosoftOAuthTokenClient tokenClient)
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