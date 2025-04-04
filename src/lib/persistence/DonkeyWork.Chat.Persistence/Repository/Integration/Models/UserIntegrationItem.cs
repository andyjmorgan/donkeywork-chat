// ------------------------------------------------------
// <copyright file="UserIntegrationItem.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Providers;

namespace DonkeyWork.Chat.Persistence.Repository.Integration.Models;

/// <summary>
/// Gets the OAuth token metadata.
/// </summary>
public record UserIntegrationItem
{
    /// <summary>
    /// Gets the provider.
    /// </summary>
    public UserProviderType Provider { get; init; }

    /// <summary>
    /// Gets the scopes.
    /// </summary>
    public List<string> Scopes { get; init; } = [];
}