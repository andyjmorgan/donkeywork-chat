// ------------------------------------------------------
// <copyright file="GoogleTokenRefreshHandler.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using DonkeyWork.Chat.Providers.Models;
using DonkeyWork.Chat.Providers.Provider.Implementation.Google.Client;
using Microsoft.Extensions.Logging;

namespace DonkeyWork.Chat.Providers.TokenManager.Implementation;

/// <inheritdoc />
public class GoogleTokenRefreshHandler : IOAuthTokenRefreshHandler
{
    private readonly IGoogleOAuthTokenClient tokenClient;
    private readonly ILogger<GoogleTokenRefreshHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GoogleTokenRefreshHandler"/> class.
    /// </summary>
    /// <param name="tokenClient">The token client.</param>
    /// <param name="logger">The logger.</param>
    public GoogleTokenRefreshHandler(
        IGoogleOAuthTokenClient tokenClient,
        ILogger<GoogleTokenRefreshHandler> logger)
    {
        this.tokenClient = tokenClient;
        this.logger = logger;
    }

    /// <inheritdoc />
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1101:Prefix local calls with this", Justification = "Just record things.")]
    public async Task<OAuthTokenResult> RefreshTokenAsync(string refreshToken, string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            this.logger.LogInformation("Refreshing Google token for user {UserId}", userId);
            var response = await this.tokenClient.RefreshTokenAsync(refreshToken, cancellationToken);
            var result = response.ToTokenResult();

            // If no refresh token is returned, keep using the old one
            if (string.IsNullOrEmpty(result.RefreshToken))
            {
                result = result with { RefreshToken = refreshToken };
            }

            this.logger.LogInformation("Successfully refreshed Google token for user {UserId}. New token expires at {ExpiresOn}", userId, result.ExpiresOn);
            return result;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error refreshing Google token for user {UserId}", userId);
            throw new InvalidOperationException($"Failed to refresh Google token: {ex.Message}", ex);
        }
    }

    /// <inheritdoc />
    public bool IsTokenValid(OAuthTokenResult token, CancellationToken cancellationToken = default)
    {
        // Consider the token valid if it expires more than 5 minutes from now
        return token.ExpiresOn.AddMinutes(-5) > DateTime.UtcNow;
    }
}