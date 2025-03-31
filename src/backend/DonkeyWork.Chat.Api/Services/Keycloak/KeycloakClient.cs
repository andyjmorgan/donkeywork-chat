// ------------------------------------------------------
// <copyright file="KeycloakClient.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Text.Json;
using DonkeyWork.Chat.Api.Configuration;
using Microsoft.Extensions.Options;

namespace DonkeyWork.Chat.Api.Services.Keycloak;

/// <summary>
/// Implementation of IKeycloakClient that interacts with Keycloak APIs.
/// </summary>
public class KeycloakClient : IKeycloakClient
{
    private readonly IHttpClientFactory httpClientFactory;
    private readonly KeycloakConfiguration keycloakConfig;
    private readonly ILogger<KeycloakClient> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="KeycloakClient"/> class.
    /// </summary>
    /// <param name="httpClientFactory">The HTTP client factory.</param>
    /// <param name="keycloakOptions">The Keycloak configuration options.</param>
    /// <param name="logger">The logger.</param>
    public KeycloakClient(
        IHttpClientFactory httpClientFactory,
        IOptions<KeycloakConfiguration> keycloakOptions,
        ILogger<KeycloakClient> logger)
    {
        this.httpClientFactory = httpClientFactory;
        this.keycloakConfig = keycloakOptions.Value;
        this.logger = logger;
    }

    /// <inheritdoc/>
    public string GetKeycloakLogoutUrl(string redirectUri)
    {
        // Ensure the redirect URI is properly encoded
        var encodedRedirectUri = Uri.EscapeDataString(redirectUri);

        // Build the complete logout URL that will end the SSO session
        return $"{this.keycloakConfig.ValidIssuer}/protocol/openid-connect/logout" +
               $"?client_id={this.keycloakConfig.ClientId}" +
               $"&post_logout_redirect_uri={encodedRedirectUri}";
    }

    /// <inheritdoc/>
    public async Task<JsonElement?> ExchangeCodeForTokensAsync(string code, string codeVerifier, string redirectUri)
    {
        try
        {
            var client = this.httpClientFactory.CreateClient();
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "authorization_code",
                ["client_id"] = this.keycloakConfig.ClientId,
                ["client_secret"] = this.keycloakConfig.ClientSecret,
                ["code"] = code,
                ["code_verifier"] = codeVerifier,
                ["redirect_uri"] = redirectUri,
            });

            var response = await client.PostAsync(
                $"{this.keycloakConfig.BackChannelAddress}/protocol/openid-connect/token",
                content);

            if (!response.IsSuccessStatusCode)
            {
                var errorText = await response.Content.ReadAsStringAsync();
                this.logger.LogError("Token exchange failed: {Error}", errorText);
                return null;
            }

            return JsonSerializer.Deserialize<JsonElement>(
                await response.Content.ReadAsStringAsync());
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Failed to exchange code for tokens");
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> LogoutAsync(string refreshToken)
    {
        try
        {
            var client = this.httpClientFactory.CreateClient();
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["client_id"] = this.keycloakConfig.ClientId,
                ["client_secret"] = this.keycloakConfig.ClientSecret,
                ["refresh_token"] = refreshToken,
            });

            var response = await client.PostAsync(
                $"{this.keycloakConfig.BackChannelAddress}/protocol/openid-connect/logout",
                content);

            if (!response.IsSuccessStatusCode)
            {
                var errorText = await response.Content.ReadAsStringAsync();
                this.logger.LogError("Logout failed: {Error}", errorText);
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Failed to logout");
            return false;
        }
    }
}