// ------------------------------------------------------
// <copyright file="IDiscordGuildTool.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Text.Json;

namespace DonkeyWork.Chat.AiTooling.ToolImplementations.Discord.Guild.Tool;

/// <summary>
/// Interface for Discord guild operations.
/// </summary>
public interface IDiscordGuildTool
{
    /// <summary>
    /// Gets all guilds (servers) the user is a member of in discord.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A JSON document containing guild information.</returns>
    Task<JsonDocument?> GetUserDiscordGuildsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets current users discord information.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A JSON document containing user information.</returns>
    Task<JsonDocument?> GetCurrentDiscordUserAsync(CancellationToken cancellationToken = default);
}