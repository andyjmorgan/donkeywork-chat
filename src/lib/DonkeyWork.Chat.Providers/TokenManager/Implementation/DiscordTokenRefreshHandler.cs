// ------------------------------------------------------
// <copyright file="DiscordTokenRefreshHandler.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Providers.Models;
using DonkeyWork.Chat.Providers.Provider.Implementation.Discord.Client;
using Microsoft.Extensions.Logging;

namespace DonkeyWork.Chat.Providers.TokenManager.Implementation;

/// <inheritdoc />
public class DiscordTokenRefreshHandler : IOAuthTokenRefreshHandler
{
    private readonly IDiscordOAuthTokenClient tokenClient;
    private readonly ILogger<DiscordTokenRefreshHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscordTokenRefreshHandler"/> class.
    /// </summary>
    /// <param name="tokenClient">The token client.</param>
    /// <param name="logger">The logger.</param>
    public DiscordTokenRefreshHandler(
        IDiscordOAuthTokenClient tokenClient,
        ILogger<DiscordTokenRefreshHandler> logger)
    {
        this.tokenClient = tokenClient;
        this.logger = logger;
    }

    /// <inheritdoc />
    public async Task<OAuthTokenResult> RefreshTokenAsync(string refreshToken, string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            this.logger.LogInformation("Refreshing Discord token for user {UserId}", userId);
            var response = await this.tokenClient.RefreshTokenAsync(refreshToken, cancellationToken);
            var result = response.ToTokenResult();

            // If no refresh token is returned, keep using the old one
            if (string.IsNullOrEmpty(result.RefreshToken))
            {
                result = result with { RefreshToken = refreshToken };
            }

            this.logger.LogInformation("Successfully refreshed Discord token for user {UserId}. New token expires at {ExpiresOn}", userId, result.ExpiresOn);
            return result;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error refreshing Discord token for user {UserId}", userId);
            throw new InvalidOperationException($"Failed to refresh Discord token: {ex.Message}", ex);
        }
    }

    /// <inheritdoc />
    public bool IsTokenValid(OAuthTokenResult token, CancellationToken cancellationToken = default)
    {
        // Consider the token valid if it expires more than 5 minutes from now
        return token.ExpiresOn.AddMinutes(-5) > DateTime.UtcNow;
    }
}