// ------------------------------------------------------
// <copyright file="GetApiKeysResponseItem.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.Persistence.Repository.ApiKey.Models;

/// <summary>
/// An API key response item.
/// </summary>
public record GetApiKeysResponseItem
{
    /// <summary>
    /// Gets the count of prompts.
    /// </summary>
    public int TotalCount { get; init; }

    /// <summary>
    /// Gets the prompts.
    /// </summary>
    public IEnumerable<ApiKeyInfoItem> Items { get; init; } = [];
}