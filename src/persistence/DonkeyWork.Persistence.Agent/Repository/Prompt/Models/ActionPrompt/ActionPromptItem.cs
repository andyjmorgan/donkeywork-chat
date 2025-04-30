// ------------------------------------------------------
// <copyright file="ActionPromptItem.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Models.Prompt;

namespace DonkeyWork.Persistence.Agent.Repository.Prompt.Models.ActionPrompt;

/// <summary>
/// An action prompt item.
/// </summary>
public record ActionPromptItem : BasePromptItem
{
    /// <summary>
    /// Gets or sets the prompt variables.
    /// </summary>
    public Dictionary<string, PromptVariable> Variables { get; set; } = [];

    /// <summary>
    /// Gets or sets the prompt messages.
    /// </summary>
    public List<PromptMessage> Messages { get; set; } = [];
}