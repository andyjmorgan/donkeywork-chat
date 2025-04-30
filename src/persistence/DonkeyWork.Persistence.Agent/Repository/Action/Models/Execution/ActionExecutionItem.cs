// ------------------------------------------------------
// <copyright file="ActionExecutionItem.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Models.Actions;
using DonkeyWork.Chat.Common.Models.Providers.Tools;

namespace DonkeyWork.Persistence.Agent.Repository.Action.Models.Execution;

/// <summary>
/// Gets or sets the action execution item.
/// </summary>
public class ActionExecutionItem
{
    /// <summary>
    /// Gets or sets the action id.
    /// </summary>
    public Guid ActionId { get; set; }

    /// <summary>
    /// Gets or sets the execution id.
    /// </summary>
    public Guid ExecutionId { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the system prompts.
    /// </summary>
    public List<SystemPromptExecutionItem> SystemPrompts { get; set; } = [];

    /// <summary>
    /// Gets or sets the action prompts.
    /// </summary>
    public List<ActionPromptExecutionItem> ActionItems { get; set; } = [];

    /// <summary>
    /// Gets or sets the allowed tools.
    /// </summary>
    public List<ToolProviderApplicationType> AllowedTools { get; set; } = [];

    /// <summary>
    /// Gets or sets the action model configuration.
    /// </summary>
    required public ActionModelConfiguration ActionModelConfiguration { get; set; }
}