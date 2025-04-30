// ------------------------------------------------------
// <copyright file="ActionPromptExecutionItem.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Models.Prompt;

namespace DonkeyWork.Persistence.Agent.Repository.Action.Models.Execution;

/// <summary>
/// An action prompt execution item.
/// </summary>
public class ActionPromptExecutionItem
{
    /// <summary>
    /// Gets or sets the id.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets the prompt name.
    /// </summary>
    required public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the prompt variables.
    /// </summary>
    public Dictionary<string, PromptVariable> Variables { get; set; } = [];

    /// <summary>
    /// Gets or sets the prompt messages.
    /// </summary>
    public List<PromptMessage> Messages { get; set; } = [];
}