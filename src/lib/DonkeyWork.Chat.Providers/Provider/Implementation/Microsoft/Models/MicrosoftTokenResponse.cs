// ------------------------------------------------------
// <copyright file="MicrosoftTokenResponse.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Text.Json.Serialization;
using DonkeyWork.Chat.Providers.Models;

namespace DonkeyWork.Chat.Providers.Provider.Implementation.Microsoft.Models;

/// <summary>
/// Represents a Microsoft OAuth token response.
/// </summary>
public class MicrosoftTokenResponse
{
    /// <summary>
    /// Gets or sets the token type.
    /// </summary>
    [JsonPropertyName("token_type")]
    public string TokenType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the scope.
    /// </summary>
    [JsonPropertyName("scope")]
    public string Scope { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the number of seconds until the token expires.
    /// </summary>
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    /// <summary>
    /// Gets or sets the extended expiration time in seconds.
    /// </summary>
    [JsonPropertyName("ext_expires_in")]
    public int ExtExpiresIn { get; set; }

    /// <summary>
    /// Gets or sets the access token.
    /// </summary>
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the refresh token.
    /// </summary>
    [JsonPropertyName("refresh_token")]
    public string? RefreshToken { get; set; }

    /// <summary>
    /// Gets or sets the ID token (for OpenID Connect flows).
    /// </summary>
    [JsonPropertyName("id_token")]
    public string? IdToken { get; set; }

    /// <summary>
    /// Gets the parsed scopes from the scope string.
    /// </summary>
    [JsonIgnore]
    public string[] Scopes => this.Scope.Split(' ', StringSplitOptions.RemoveEmptyEntries);

    /// <summary>
    /// Gets the calculated token expiration date/time.
    /// </summary>
    [JsonIgnore]
    public DateTime ExpiresOn => DateTime.UtcNow.AddSeconds(this.ExpiresIn);

    /// <summary>
    /// Converts the response to an OAuthTokenResult.
    /// </summary>
    /// <returns>An OAuthTokenResult populated with data from this response.</returns>
    public OAuthTokenResult ToTokenResult()
    {
        return new OAuthTokenResult
        {
            AccessToken = this.AccessToken,
            RefreshToken = this.RefreshToken ?? string.Empty,
            ExpiresOn = this.ExpiresOn,
            ProviderType = Common.Providers.UserProviderType.Microsoft,
            Scopes = this.Scopes,
        };
    }
}