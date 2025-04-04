// ------------------------------------------------------
// <copyright file="UserOAuthTokenItem.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Providers;

namespace DonkeyWork.Chat.Persistence.Repository.Integration.Models;

/// <summary>
/// A record for the user OAuth token item.
/// </summary>
public record UserOAuthTokenItem : UserIntegrationItem
{
    /// <summary>
    /// Gets the OAuth token metadata.
    /// </summary>
    public Dictionary<UserProviderDataKeyType, string> Metadata { get; init; } = [];

    /// <summary>
    /// Gets or sets the access token.
    /// </summary>
    public DateTimeOffset? Expiration { get; set; }
}