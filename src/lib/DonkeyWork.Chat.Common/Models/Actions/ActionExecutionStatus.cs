// ------------------------------------------------------
// <copyright file="ActionExecutionStatus.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.Common.Models.Actions;

/// <summary>
/// Gets or sets the status of an action execution.
/// </summary>
public enum ActionExecutionStatus
{
    /// <summary>
    /// Action execution is pending.
    /// </summary>
    Pending,

    /// <summary>
    /// Action execution is in progress.
    /// </summary>
    InProgress,

    /// <summary>
    /// Action execution is completed with an error.
    /// </summary>
    Failed,

    /// <summary>
    /// Completed successfully.
    /// </summary>
    Completed,
}