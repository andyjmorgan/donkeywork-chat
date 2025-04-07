// ------------------------------------------------------
// <copyright file="IDiscordApiClientFactory.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using Discord.Rest;

namespace DonkeyWork.Chat.AiTooling.ToolImplementations.Discord.Common.Api;

/// <summary>
/// Interface for creating Discord API clients.
/// </summary>
public interface IDiscordApiClientFactory
{
    /// <summary>
    /// Creates a Discord REST client for user API calls.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A configured Discord rest client.</returns>
    Task<DiscordRestClient> CreateDiscordClientAsync(CancellationToken cancellationToken = default);
}