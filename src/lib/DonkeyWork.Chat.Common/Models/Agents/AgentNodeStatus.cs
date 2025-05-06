// ------------------------------------------------------
// <copyright file="AgentNodeStatus.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.Common.Models.Agents;

/// <summary>
/// Gets or sets the agent node status.
/// </summary>
public enum AgentNodeStatus
{
    /// <summary>
    /// Agent is not started.
    /// </summary>
    NotStarted,

    /// <summary>
    /// Agent is in progress
    /// </summary>
    InProgress,

    /// <summary>
    /// Agent is completed.
    /// </summary>
    Completed,
}
