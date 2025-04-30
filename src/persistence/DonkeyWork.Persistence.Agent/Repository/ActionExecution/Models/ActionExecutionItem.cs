// ------------------------------------------------------
// <copyright file="ActionExecutionItem.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Models.Actions;

namespace DonkeyWork.Persistence.Agent.Repository.ActionExecution.Models;

/// <summary>
/// An action execution item.
/// </summary>
public class ActionExecutionItem
{
    /// <summary>
    /// Gets or sets the unique identifier for the action execution item.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the execution ID associated with the action execution item.
    /// </summary>
    public Guid ExecutionId { get; set; }

    /// <summary>
    /// Gets or sets the user ID associated with the action execution item.
    /// </summary>
    public Guid ActionId { get; set; }

    /// <summary>
    /// Gets or sets the action name.
    /// </summary>
    public string ActionName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the execution status.
    /// </summary>
    public ActionExecutionStatus ExecutionStatus { get; set; }

    /// <summary>
    /// Gets or sets the created at.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets or sets the end time.
    /// </summary>
    public DateTimeOffset EndTime { get; set; } = DateTimeOffset.UtcNow;
}