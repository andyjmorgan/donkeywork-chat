// ------------------------------------------------------
// <copyright file="GetPromptsResponseItem.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.Persistence.Repository.Prompt.Models;

/// <summary>
/// Gets the prompts response.
/// </summary>
public record GetPromptsResponseItem
{
    /// <summary>
    /// Gets the count of prompts.
    /// </summary>
    public int TotalCount { get; init; }

    /// <summary>
    /// Gets the prompts.
    /// </summary>
    public IEnumerable<PromptItem> Prompts { get; init; } = [];
}