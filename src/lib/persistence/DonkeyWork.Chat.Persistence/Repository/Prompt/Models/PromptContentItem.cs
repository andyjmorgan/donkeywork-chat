// ------------------------------------------------------
// <copyright file="PromptContentItem.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.Persistence.Repository.Prompt.Models;

/// <summary>
/// The contents of a prompt.
/// </summary>
public record PromptContentItem
{
    /// <summary>
    /// Gets the prompt content.
    /// </summary>
    public string Content { get; init; } = string.Empty;
}