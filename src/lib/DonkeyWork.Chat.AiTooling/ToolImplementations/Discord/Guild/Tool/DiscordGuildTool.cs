// ------------------------------------------------------
// <copyright file="DiscordGuildTool.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using Discord;
using DonkeyWork.Chat.AiTooling.Attributes;
using DonkeyWork.Chat.AiTooling.ToolImplementations.Discord.Common.Api;
using DonkeyWork.Chat.Common.Providers;

namespace DonkeyWork.Chat.AiTooling.ToolImplementations.Discord.Guild.Tool;

/// <summary>
/// Tool implementation for Discord guild operations.
/// </summary>
[ToolProvider(UserProviderType.Discord)]
public class DiscordGuildTool(IDiscordApiClientFactory discordApiClientFactory)
    : Base.Tool, IDiscordGuildTool
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new ()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    /// <inheritdoc />
    [ToolFunction]
    [Description("Get all guilds (servers) the user is a member of on discord.")]
    [ToolProviderScopes(
        UserProviderScopeHandleType.Any,
        "identify",
        "guilds")]
    public async Task<JsonDocument?> GetUserGuildsAsync(
        [ToolIgnoredParameter] CancellationToken cancellationToken = default)
    {
        var client = await discordApiClientFactory.CreateDiscordClientAsync(cancellationToken);

        // Get the guilds the user is a member of
        var guilds = await client.GetGuildSummariesAsync(new RequestOptions
        {
            CancelToken = cancellationToken,
        }).FlattenAsync();

        // Format the guild data
        return JsonDocument.Parse(JsonSerializer.Serialize(guilds, JsonSerializerOptions));
    }

    /// <inheritdoc />
    [ToolFunction]
    [Description("Get information about the current user in respect of their discord identity.")]
    [ToolProviderScopes(
        UserProviderScopeHandleType.Any,
        "identify")]
    public async Task<JsonDocument?> GetCurrentUserAsync(
        [ToolIgnoredParameter] CancellationToken cancellationToken = default)
    {
        var client = await discordApiClientFactory.CreateDiscordClientAsync(cancellationToken);

        // Get the current user info
        var user = client.CurrentUser;

        // Format the user data
        var userData = new
        {
            Id = user.Id.ToString(),
            user.Username,
            user.GlobalName,
            user.Discriminator,
            user.AvatarId,
            user.IsBot,
            user.IsMfaEnabled,
        };

        return JsonDocument.Parse(JsonSerializer.Serialize(userData, JsonSerializerOptions));
    }
}