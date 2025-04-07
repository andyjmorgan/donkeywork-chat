// ------------------------------------------------------
// <copyright file="DiscordGuildDetail.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Text.Json.Serialization;

namespace DonkeyWork.Chat.Providers.Provider.Implementation.Discord.Models;

/// <summary>
/// Represents detailed information about a Discord guild.
/// </summary>
public class DiscordGuildDetail : DiscordGuild
{
    /// <summary>
    /// Gets or sets the guild description.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the guild features.
    /// </summary>
    [JsonPropertyName("features")]
    public List<string> Features { get; set; } = new();

    /// <summary>
    /// Gets or sets the guild verification level.
    /// </summary>
    [JsonPropertyName("verification_level")]
    public int VerificationLevel { get; set; }

    /// <summary>
    /// Gets or sets the default message notification level.
    /// </summary>
    [JsonPropertyName("default_message_notifications")]
    public int DefaultMessageNotifications { get; set; }

    /// <summary>
    /// Gets or sets the member count.
    /// </summary>
    [JsonPropertyName("member_count")]
    public int MemberCount { get; set; }

    /// <summary>
    /// Gets or sets the list of channels.
    /// </summary>
    [JsonPropertyName("channels")]
    public List<DiscordChannel> Channels { get; set; } = new();
}