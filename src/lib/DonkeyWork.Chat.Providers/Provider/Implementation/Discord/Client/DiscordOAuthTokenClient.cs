// ------------------------------------------------------
// <copyright file="DiscordOAuthTokenClient.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Text.Json;
using DonkeyWork.Chat.Providers.Provider.Configuration;
using DonkeyWork.Chat.Providers.Provider.Implementation.Discord.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DonkeyWork.Chat.Providers.Provider.Implementation.Discord.Client;

/// <inheritdoc />
public class DiscordOAuthTokenClient : IDiscordOAuthTokenClient
{
    private readonly HttpClient httpClient;
    private readonly ILogger<DiscordOAuthTokenClient> logger;
    private readonly DiscordOAuthConfiguration discordOAuthConfiguration;

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscordOAuthTokenClient"/> class.
    /// </summary>
    /// <param name="httpClientFactory">The HTTP client factory.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="configuration">The Discord configuration.</param>
    public DiscordOAuthTokenClient(
        IHttpClientFactory httpClientFactory,
        ILogger<DiscordOAuthTokenClient> logger,
        IOptions<DiscordOAuthConfiguration> configuration)
    {
        this.httpClient = httpClientFactory.CreateClient("OAuthClient");
        this.logger = logger;
        this.discordOAuthConfiguration = configuration.Value;
    }

    /// <inheritdoc />
    public async Task<DiscordTokenResponse> ExchangeCodeForTokenAsync(
        string code,
        string redirectUri,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Prepare the request content
            var requestContent = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["client_id"] = this.discordOAuthConfiguration.ClientId,
                ["client_secret"] = this.discordOAuthConfiguration.ClientSecret,
                ["code"] = code,
                ["redirect_uri"] = redirectUri,
                ["grant_type"] = "authorization_code",
            });

            // Log request details
            this.logger.LogInformation(
                "Sending Discord token request to {TokenUrl} with redirect URI {RedirectUri}",
                this.discordOAuthConfiguration.TokenUrl,
                redirectUri);

            // Make the request
            var response = await this.httpClient.PostAsync(this.discordOAuthConfiguration.TokenUrl, requestContent, cancellationToken);

            // Log response
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            this.logger.LogInformation(
                "Discord token response: Status={StatusCode}, Content={Content}",
                response.StatusCode,
                responseContent);

            response.EnsureSuccessStatusCode();

            // Deserialize the response
            var tokenResponse = JsonSerializer.Deserialize<DiscordTokenResponse>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            });

            if (tokenResponse == null)
            {
                throw new InvalidOperationException("Failed to deserialize token response");
            }

            return tokenResponse;
        }
        catch (HttpRequestException ex)
        {
            this.logger.LogError(ex, "HTTP error exchanging code for token at {TokenUrl}", this.discordOAuthConfiguration.TokenUrl);
            throw new InvalidOperationException($"Failed to exchange code for token: {ex.Message}", ex);
        }
        catch (JsonException ex)
        {
            this.logger.LogError(ex, "JSON parsing error in token response");
            throw new InvalidOperationException($"Failed to parse token response: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Unexpected error exchanging code for token");
            throw new InvalidOperationException($"Unexpected error exchanging code for token: {ex.Message}", ex);
        }
    }

    /// <inheritdoc />
    public async Task<DiscordTokenResponse> RefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Prepare the request content
            var requestContent = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["client_id"] = this.discordOAuthConfiguration.ClientId,
                ["client_secret"] = this.discordOAuthConfiguration.ClientSecret,
                ["refresh_token"] = refreshToken,
                ["grant_type"] = "refresh_token",
            });

            // Log request
            this.logger.LogInformation("Refreshing Discord token");

            // Make the request
            var response = await this.httpClient.PostAsync(this.discordOAuthConfiguration.TokenUrl, requestContent, cancellationToken);

            // Log response
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            this.logger.LogInformation(
                "Discord token refresh response: Status={StatusCode}, Content={Content}",
                response.StatusCode,
                responseContent);

            response.EnsureSuccessStatusCode();

            // Deserialize the response
            var tokenResponse = JsonSerializer.Deserialize<DiscordTokenResponse>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            });

            if (tokenResponse == null)
            {
                throw new InvalidOperationException("Failed to deserialize token response");
            }

            return tokenResponse;
        }
        catch (HttpRequestException ex)
        {
            this.logger.LogError(ex, "HTTP error refreshing token at {TokenUrl}", this.discordOAuthConfiguration.TokenUrl);
            throw new InvalidOperationException($"Failed to refresh token: {ex.Message}", ex);
        }
        catch (JsonException ex)
        {
            this.logger.LogError(ex, "JSON parsing error in refresh token response");
            throw new InvalidOperationException($"Failed to parse refresh token response: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Unexpected error refreshing token");
            throw new InvalidOperationException($"Unexpected error refreshing token: {ex.Message}", ex);
        }
    }
}