// ------------------------------------------------------
// <copyright file="MicrosoftTokenRefreshService.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Providers.Provider.Implementation.Discord.Client;
using DonkeyWork.Chat.Providers.Services.TokenRefresh.Models;

namespace DonkeyWork.Chat.Providers.Services.TokenRefresh;

/// <inheritdoc cref="ITokenRefreshService"/>
public class DiscordTokenRefreshService : ITokenRefreshService
{
    /// <summary>
    /// Gets the token client.
    /// </summary>
    private readonly IDiscordOAuthTokenClient tokenClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscordTokenRefreshService"/> class.
    /// </summary>
    /// <param name="tokenClient">The token client.</param>
    public DiscordTokenRefreshService(IDiscordOAuthTokenClient tokenClient)
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