// ------------------------------------------------------
// <copyright file="ActionPromptContentItem.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Models.Prompt;

namespace DonkeyWork.Persistence.Agent.Repository.Prompt.Models.ActionPrompt;

/// <summary>
/// A content item for an action prompt.
/// </summary>
public record ActionPromptContentItem
{
    /// <summary>
    /// Gets the prompt name.
    /// </summary>
    required public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Gets the prompt description.
    /// </summary>
    required public string Description { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the prompt variables.
    /// </summary>
    public List<PromptVariable> Variables { get; set; } = [];

    /// <summary>
    /// Gets or sets the prompt messages.
    /// </summary>
    public List<PromptMessage> Messages { get; set; } = [];
}