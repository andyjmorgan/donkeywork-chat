// ------------------------------------------------------
// <copyright file="DiscordGuild.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Text.Json.Serialization;

namespace DonkeyWork.Chat.Providers.Provider.Implementation.Discord.Models;

/// <summary>
/// Represents a basic Discord guild (server).
/// </summary>
public class DiscordGuild
{
    /// <summary>
    /// Gets or sets the guild ID.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the guild name.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the guild icon hash.
    /// </summary>
    [JsonPropertyName("icon")]
    public string? Icon { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the user is the guild owner.
    /// </summary>
    [JsonPropertyName("owner")]
    public bool IsOwner { get; set; }

    /// <summary>
    /// Gets or sets the user's permissions in the guild.
    /// </summary>
    [JsonPropertyName("permissions")]
    public string Permissions { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the approximate member count.
    /// </summary>
    [JsonPropertyName("approximate_member_count")]
    public int? ApproximateMemberCount { get; set; }
}