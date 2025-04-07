// ------------------------------------------------------
// <copyright file="GoogleOAuthProvider.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Web;
using DonkeyWork.Chat.Providers.Models;
using DonkeyWork.Chat.Providers.Provider.Configuration;
using DonkeyWork.Chat.Providers.Provider.Implementation.Google.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DonkeyWork.Chat.Providers.Provider.Implementation.Google;

/// <inheritdoc />
public class GoogleOAuthProvider : IOAuthProvider
{
    /// <summary>
    /// The Google OAuth configuration.
    /// </summary>
    private readonly GoogleOAuthConfiguration configuration;

    /// <summary>
    /// The OAuth HTTP client.
    /// </summary>
    private readonly IGoogleOAuthTokenClient oauthHttpClient;

    /// <summary>
    /// The logger.
    /// </summary>
    private readonly ILogger<GoogleOAuthProvider> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GoogleOAuthProvider"/> class.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <param name="oauthHttpClient">The OAuth HTTP client.</param>
    /// <param name="logger">The logger.</param>
    public GoogleOAuthProvider(
        IOptions<GoogleOAuthConfiguration> options,
        IGoogleOAuthTokenClient oauthHttpClient,
        ILogger<GoogleOAuthProvider> logger)
    {
        this.configuration = options.Value;
        this.oauthHttpClient = oauthHttpClient;
        this.logger = logger;
    }

    /// <inheritdoc />
    public Task<string> GetAuthorizationUrl(string redirectUri, string? state = null)
    {
        // Build the query parameters.
        var queryParams = new Dictionary<string, string>
        {
            ["client_id"] = this.configuration.ClientId,
            ["redirect_uri"] = redirectUri,
            ["response_type"] = "code",
            ["scope"] = string.Join(" ", this.configuration.Scopes),
            ["access_type"] = "offline", // Request a refresh token
            ["prompt"] = "consent", // Force the consent screen to show
        };

        // Add the state if required.
        if (state != null)
        {
            queryParams["state"] = state;
        }

        // Build the URI
        var uriBuilder = new UriBuilder(this.configuration.AuthorizeUrl);
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);

        foreach (var param in queryParams)
        {
            query[param.Key] = param.Value;
        }

        uriBuilder.Query = query.ToString();
        return Task.FromResult(uriBuilder.ToString());
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