// ------------------------------------------------------
// <copyright file="DiscordChannel.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Text.Json.Serialization;

namespace DonkeyWork.Chat.Providers.Provider.Implementation.Discord.Models;

/// <summary>
/// Represents a Discord channel.
/// </summary>
public class DiscordChannel
{
    /// <summary>
    /// Gets or sets the channel ID.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the channel type.
    /// </summary>
    [JsonPropertyName("type")]
    public int Type { get; set; }

    /// <summary>
    /// Gets or sets the channel name.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the channel topic.
    /// </summary>
    [JsonPropertyName("topic")]
    public string? Topic { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the channel is NSFW.
    /// </summary>
    [JsonPropertyName("nsfw")]
    public bool IsNsfw { get; set; }

    /// <summary>
    /// Gets or sets the ID of the parent channel/category.
    /// </summary>
    [JsonPropertyName("parent_id")]
    public string? ParentId { get; set; }

    /// <summary>
    /// Gets or sets the position of the channel.
    /// </summary>
    [JsonPropertyName("position")]
    public int Position { get; set; }
}