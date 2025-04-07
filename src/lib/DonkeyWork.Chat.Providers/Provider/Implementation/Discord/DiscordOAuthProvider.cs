// ------------------------------------------------------
// <copyright file="DiscordOAuthProvider.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Providers.Models;
using DonkeyWork.Chat.Providers.Provider.Configuration;
using DonkeyWork.Chat.Providers.Provider.Implementation.Discord.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DonkeyWork.Chat.Providers.Provider.Implementation.Discord;

/// <inheritdoc />
public class DiscordOAuthProvider : IOAuthProvider
{
    /// <summary>
    /// The Discord OAuth configuration.
    /// </summary>
    private readonly DiscordOAuthConfiguration configuration;

    /// <summary>
    /// The OAuth HTTP client.
    /// </summary>
    private readonly IDiscordOAuthTokenClient oauthHttpClient;

    /// <summary>
    /// The logger.
    /// </summary>
    private readonly ILogger<DiscordOAuthProvider> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscordOAuthProvider"/> class.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <param name="oauthHttpClient">The OAuth HTTP client.</param>
    /// <param name="logger">The logger.</param>
    public DiscordOAuthProvider(
        IOptions<DiscordOAuthConfiguration> options,
        IDiscordOAuthTokenClient oauthHttpClient,
        ILogger<DiscordOAuthProvider> logger)
    {
        this.configuration = options.Value;
        this.oauthHttpClient = oauthHttpClient;
        this.logger = logger;
    }

    /// <inheritdoc />
    public Task<string> GetAuthorizationUrl(string redirectUri, string? state = null)
    {
        // Get all configured scopes from the configuration
        string scopesJoined = string.Join(" ", this.configuration.Scopes);

        // Properly escape all URL components
        string escapedClientId = Uri.EscapeDataString(this.configuration.ClientId);
        string escapedRedirectUri = Uri.EscapeDataString(redirectUri);
        string escapedScopes = Uri.EscapeDataString(scopesJoined);

        // Build the URL with proper URL encoding
        string authUrl = $"{this.configuration.AuthorizeUrl}?" +
                        $"client_id={escapedClientId}&" +
                        $"redirect_uri={escapedRedirectUri}&" +
                        $"response_type=code&" +
                        $"scope={escapedScopes}";

        if (!string.IsNullOrEmpty(state))
        {
            authUrl += $"&state={Uri.EscapeDataString(state)}";
        }

        this.logger.LogInformation("Discord auth URL: {AuthUrl}", authUrl);
        this.logger.LogInformation("Discord scopes requested: {Scopes}", scopesJoined);
        return Task.FromResult(authUrl);
    }

    /// <inheritdoc />
    public async Task<OAuthTokenResult> ExchangeCodeForToken(string code, string redirectUri)
    {
        try
        {
            this.logger.LogInformation("Exchanging code for token with redirect URI {RedirectUri}", redirectUri);
            var tokenResponse = await this.oauthHttpClient.ExchangeCodeForTokenAsync(
                code,
                redirectUri);

            var result = tokenResponse.ToTokenResult();
            this.logger.LogInformation("Successfully exchanged code for token. Token expires at {ExpiresOn}", result.ExpiresOn);
            return result;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error exchanging authorization code for token");
            throw new InvalidOperationException($"Failed to exchange authorization code for token: {ex.Message}", ex);
        }
    }
}