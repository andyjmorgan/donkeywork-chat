// ------------------------------------------------------
// <copyright file="MicrosoftTokenRefreshHandler.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Providers.Models;
using DonkeyWork.Chat.Providers.Provider.Configuration;
using DonkeyWork.Chat.Providers.Provider.Implementation.Microsoft.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DonkeyWork.Chat.Providers.TokenManager.Implementation;

/// <summary>
/// Handles refreshing OAuth tokens for Microsoft.
/// </summary>
public class MicrosoftTokenRefreshHandler : IOAuthTokenRefreshHandler
{
    // Token is considered valid if it has more than this much time left
    private static readonly TimeSpan TokenRefreshWindow = TimeSpan.FromMinutes(5);

    private readonly MicrosoftOAuthConfiguration configuration;
    private readonly IMicrosoftOAuthTokenClient oauthHttpClient;
    private readonly ILogger<MicrosoftTokenRefreshHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="MicrosoftTokenRefreshHandler"/> class.
    /// </summary>
    /// <param name="options">Microsoft OAuth configuration options.</param>
    /// <param name="oauthHttpClient">The OAuth HTTP client.</param>
    /// <param name="logger">Logger instance.</param>
    public MicrosoftTokenRefreshHandler(
        IOptions<MicrosoftOAuthConfiguration> options,
        IMicrosoftOAuthTokenClient oauthHttpClient,
        ILogger<MicrosoftTokenRefreshHandler> logger)
    {
        this.configuration = options.Value;
        this.oauthHttpClient = oauthHttpClient;
        this.logger = logger;
    }

    /// <inheritdoc />
    public async Task<OAuthTokenResult> RefreshTokenAsync(string refreshToken, string userId, CancellationToken cancellationToken = default)
    {
        this.logger.LogInformation("Refreshing Microsoft token for user {UserId}", userId);

        if (string.IsNullOrEmpty(refreshToken))
        {
            throw new ArgumentException("Refresh token cannot be null or empty", nameof(refreshToken));
        }

        try
        {
            // Use the OAuth HTTP client to refresh the token
            var tokenResponse = await this.oauthHttpClient.RefreshTokenAsync(refreshToken, cancellationToken);

            // If no new refresh token was provided, use the existing one
            if (string.IsNullOrEmpty(tokenResponse.RefreshToken))
            {
                tokenResponse.RefreshToken = refreshToken;
            }

            // Convert the response to an OAuthTokenResult
            var result = tokenResponse.ToTokenResult();

            this.logger.LogInformation(
                "Successfully refreshed token for user {UserId}. Token expires at {ExpiresOn}",
                userId,
                result.ExpiresOn);

            return result;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error refreshing Microsoft token for user {UserId}", userId);
            throw;
        }
    }

    /// <inheritdoc />
    public bool IsTokenValid(OAuthTokenResult token, CancellationToken cancellationToken = default)
    {
        // Check if token has not expired and has some time left before it expires
        return token.ExpiresOn > DateTime.UtcNow.Add(TokenRefreshWindow);
    }
}
