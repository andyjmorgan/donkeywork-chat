// ------------------------------------------------------
// <copyright file="UpsertPromptItem.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.Persistence.Repository.Prompt.Models;

/// <summary>
/// Represents an item to upsert a prompt.
/// </summary>
public record UpsertPromptItem
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
}