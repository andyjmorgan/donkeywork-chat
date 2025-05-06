// ------------------------------------------------------
// <copyright file="AgentExecutionDetails.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Workflows.Core.Agents.Execution;

/// <summary>
/// An agent execution details.
/// </summary>
public class AgentExecutionDetails
{
    /// <summary>
    /// Gets or sets the agent execution details.
    /// </summary>
    public Guid AgentId { get; set; }

    /// <summary>
    /// Gets or sets the agent name.
    /// </summary>
    public string AgentName { get; set; } = string.Empty;

    /// <summary>
    /// gets or sets the user id.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the execution id.
    /// </summary>
    public Guid ExecutionId { get; set; } = Guid.NewGuid();
}
