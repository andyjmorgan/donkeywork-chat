// ------------------------------------------------------
// <copyright file="UpsertActionPromptItem.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Models.Prompt;

namespace DonkeyWork.Persistence.Agent.Repository.Prompt.Models.ActionPrompt;

/// <summary>
/// An item for upserting an action prompt.
/// </summary>
public class UpsertActionPromptItem
{
    /// <summary>
    /// Gets the title.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Gets the description.
    /// </summary>
    public string Description { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the prompt variables.
    /// </summary>
    public Dictionary<string, PromptVariable> Variables { get; set; } = [];

    /// <summary>
    /// Gets or sets the prompt messages.
    /// </summary>
    public List<PromptMessage> Messages { get; set; } = [];
}