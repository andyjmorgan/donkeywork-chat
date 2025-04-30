// ------------------------------------------------------
// <copyright file="KeycloakClient.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Net.Http.Json;
using System.Text.Json;
using DonkeyWork.Chat.Api.Core.Configuration;
using DonkeyWork.Chat.Api.Core.Services.Keycloak.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DonkeyWork.Chat.Api.Core.Services.Keycloak;

/// <summary>
/// Implementation of IKeycloakClient that interacts with Keycloak APIs.
/// </summary>
public class KeycloakClient : IKeycloakClient
{
    private readonly KeycloakConfiguration keycloakConfig;
    private readonly ILogger<KeycloakClient> logger;
    private readonly HttpClient httpClient;

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
        this.httpClient = httpClientFactory.CreateClient(nameof(KeycloakClient));
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
    public async Task<KeycloakUser?> GetUserInfoByIdAsync(Guid userId)
    {
        try
        {
            var accessToken = await this.GetAdminTokenAsync();

            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"{this.keycloakConfig.BackChannelAdminAddress}/users/{userId}");

            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await this.httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                this.logger.LogError("Failed to get user info for {UserId}", userId);
                return null;
            }

            return await response.Content.ReadFromJsonAsync<KeycloakUser>();
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Exception in GetUserInfoByIdAsync");
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task<JsonElement?> ExchangeCodeForTokensAsync(string code, string codeVerifier, string redirectUri)
    {
        try
        {
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "authorization_code",
                ["client_id"] = this.keycloakConfig.ClientId,
                ["client_secret"] = this.keycloakConfig.ClientSecret,
                ["code"] = code,
                ["code_verifier"] = codeVerifier,
                ["redirect_uri"] = redirectUri,
            });

            var response = await this.httpClient.PostAsync(
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
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["client_id"] = this.keycloakConfig.ClientId,
                ["client_secret"] = this.keycloakConfig.ClientSecret,
                ["refresh_token"] = refreshToken,
            });

            var response = await this.httpClient.PostAsync(
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

    private async Task<string?> GetAdminTokenAsync()
    {
        var tokenResponse = await this.httpClient.PostAsync(
            $"{this.keycloakConfig.BackChannelAddress}/protocol/openid-connect/token",
            new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials",
                ["client_id"] = this.keycloakConfig.ClientId,
                ["client_secret"] = this.keycloakConfig.ClientSecret,
            }));

        if (!tokenResponse.IsSuccessStatusCode)
        {
            this.logger.LogError("Failed to acquire admin token");
            return null;
        }

        var tokenJson = JsonSerializer.Deserialize<JsonElement>(await tokenResponse.Content.ReadAsStringAsync());
        var accessToken = tokenJson.GetProperty("access_token").GetString();
        return accessToken;
    }
}
