// ------------------------------------------------------
// <copyright file="GetActionPromptsResponseItem.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Persistence.Agent.Repository.Prompt.Models.ActionPrompt;

/// <summary>
/// A response item for getting action prompts.
/// </summary>
public class GetActionPromptsResponseItem
{
    /// <summary>
    /// Gets the count of prompts.
    /// </summary>
    public int TotalCount { get; init; }

    /// <summary>
    /// Gets the prompts.
    /// </summary>
    public IEnumerable<ActionPromptItem> Prompts { get; init; } = [];
}