// ------------------------------------------------------
// <copyright file="OAuthTokenResult.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Models.Providers.Tools;

namespace DonkeyWork.Chat.Providers.Models;

/// <summary>
/// An OAuth token result.
/// </summary>
public record OAuthTokenResult
{
    /// <summary>
    /// Gets the provider type.
    /// </summary>
    public ToolProviderType ProviderType { get; init; }

    /// <summary>
    /// Gets the access token.
    /// </summary>
    public string AccessToken { get; init; } = string.Empty;

    /// <summary>
    /// Gets the refresh token.
    /// </summary>
    public string RefreshToken { get; init; } = string.Empty;

    /// <summary>
    /// Gets the token expiry.
    /// </summary>
    public DateTimeOffset ExpiresOn { get; init; }

    /// <summary>
    /// Gets the token scope metadata.
    /// </summary>
    public string[] Scopes { get; init; } = [];
}