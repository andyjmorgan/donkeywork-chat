// ------------------------------------------------------
// <copyright file="GetActionExecutionsItem.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Persistence.Agent.Repository.ActionExecution.Models;

/// <summary>
/// Gets the action executions response item.
/// </summary>
public class GetActionExecutionsItem
{
    /// <summary>
    /// Gets or sets the total count.
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Gets or sets the actions.
    /// </summary>
    public List<ActionExecutionItem> Actions { get; set; } = [];
}