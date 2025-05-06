// ------------------------------------------------------
// <copyright file="AgentStart.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.Common.Models.Streaming.Agent;

/// <summary>
/// An agent start message.
/// </summary>
public record AgentStart : BaseAgentItem
{

    /// <summary>
    /// Gets the agent start time.
    /// </summary>
    public DateTime StartTime { get; init; } = DateTime.UtcNow;
}
