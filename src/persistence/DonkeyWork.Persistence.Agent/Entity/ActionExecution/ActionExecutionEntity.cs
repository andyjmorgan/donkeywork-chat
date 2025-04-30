// ------------------------------------------------------
// <copyright file="ActionExecutionEntity.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.ComponentModel.DataAnnotations.Schema;
using DonkeyWork.Chat.Common.Models.Actions;
using DonkeyWork.Chat.Common.Models.Streaming;
using DonkeyWork.Persistence.Common.Entity.Base;

namespace DonkeyWork.Persistence.Agent.Entity.ActionExecution;

/// <summary>
/// An entity representing an action execution.
/// </summary>
public class ActionExecutionEntity : BaseUserEntity
{
    /// <summary>
    /// Gets or sets the execution ID.
    /// </summary>
    public Guid ExecutionId { get; set; }

    /// <summary>
    /// Gets or sets the action ID.
    /// </summary>
    public Guid ActionId { get; set; }

    /// <summary>
    /// Gets or sets the action name.
    /// </summary>
    public string ActionName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the action status.
    /// </summary>
    public ActionExecutionStatus Status { get; set; } = ActionExecutionStatus.Pending;

    /// <summary>
    /// Gets or sets the result data.
    /// </summary>
    [Column(TypeName = "jsonb")]
    public List<BaseStreamItem> Data { get; set; } = [];

    /// <summary>
    /// Gets or sets the result of the action execution.
    /// </summary>
    public string Result { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the end time of the action execution.
    /// </summary>
    public DateTimeOffset EndTime { get; set; }
}