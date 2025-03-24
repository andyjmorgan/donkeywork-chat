// ------------------------------------------------------
// <copyright file="PromptItem.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Persistence.Repository.Base;

namespace DonkeyWork.Chat.Persistence.Repository.Prompt.Models;

/// <summary>
/// A prompt item.
/// </summary>
public record PromptItem : BaseItem
{
    /// <summary>
    /// Gets the title.
    /// </summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>
    /// Gets the description.
    /// </summary>
    public string Description { get; init; } = string.Empty;

    /// <summary>
    /// Gets the content.
    /// </summary>
    public string Content { get; init; } = string.Empty;

    /// <summary>
    /// Gets the usage count.
    /// </summary>
    public int UsageCount { get; init; }
}