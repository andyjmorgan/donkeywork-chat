// ------------------------------------------------------
// <copyright file="GoogleOAuthTokenClient.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Net.Http.Json;
using System.Text.Json;
using DonkeyWork.Chat.Providers.Provider.Configuration;
using DonkeyWork.Chat.Providers.Provider.Implementation.Google.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DonkeyWork.Chat.Providers.Provider.Implementation.Google.Client;

/// <inheritdoc />
public class GoogleOAuthTokenClient : IGoogleOAuthTokenClient
{
    private readonly HttpClient httpClient;
    private readonly ILogger<GoogleOAuthTokenClient> logger;
    private readonly GoogleOAuthConfiguration googleOAuthConfiguration;

    /// <summary>
    /// Initializes a new instance of the <see cref="GoogleOAuthTokenClient"/> class.
    /// </summary>
    /// <param name="httpClientFactory">The HTTP client factory.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="configuration">The Google configuration.</param>
    public GoogleOAuthTokenClient(
        IHttpClientFactory httpClientFactory,
        ILogger<GoogleOAuthTokenClient> logger,
        IOptions<GoogleOAuthConfiguration> configuration)
    {
        this.httpClient = httpClientFactory.CreateClient("OAuthClient");
        this.logger = logger;
        this.googleOAuthConfiguration = configuration.Value;
    }

    /// <inheritdoc />
    public async Task<GoogleTokenResponse> ExchangeCodeForTokenAsync(
        string code,
        string redirectUri,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Prepare the request content
            var requestContent = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["client_id"] = this.googleOAuthConfiguration.ClientId,
                ["client_secret"] = this.googleOAuthConfiguration.ClientSecret,
                ["code"] = code,
                ["redirect_uri"] = redirectUri,
                ["grant_type"] = "authorization_code",
            });

            // Make the request
            var response = await this.httpClient.PostAsync(this.googleOAuthConfiguration.TokenUrl, requestContent, cancellationToken);
            response.EnsureSuccessStatusCode();

            // Deserialize the response
            var tokenResponse = await response.Content.ReadFromJsonAsync<GoogleTokenResponse>(cancellationToken: cancellationToken);
            if (tokenResponse == null)
            {
                throw new InvalidOperationException("Failed to deserialize token response");
            }

            return tokenResponse;
        }
        catch (HttpRequestException ex)
        {
            this.logger.LogError(ex, "HTTP error exchanging code for token at {TokenUrl}", this.googleOAuthConfiguration.TokenUrl);
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
    public async Task<GoogleTokenResponse> RefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Prepare the request content
            var requestContent = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["client_id"] = this.googleOAuthConfiguration.ClientId,
                ["client_secret"] = this.googleOAuthConfiguration.ClientSecret,
                ["refresh_token"] = refreshToken,
                ["grant_type"] = "refresh_token",
            });

            // Make the request
            var response = await this.httpClient.PostAsync(this.googleOAuthConfiguration.TokenUrl, requestContent, cancellationToken);
            response.EnsureSuccessStatusCode();

            // Deserialize the response
            var tokenResponse = await response.Content.ReadFromJsonAsync<GoogleTokenResponse>(cancellationToken: cancellationToken);
            if (tokenResponse == null)
            {
                throw new InvalidOperationException("Failed to deserialize token response");
            }

            return tokenResponse;
        }
        catch (HttpRequestException ex)
        {
            this.logger.LogError(ex, "HTTP error refreshing token at {TokenUrl}", this.googleOAuthConfiguration.TokenUrl);
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