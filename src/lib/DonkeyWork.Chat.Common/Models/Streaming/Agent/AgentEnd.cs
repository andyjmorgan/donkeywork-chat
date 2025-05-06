// ------------------------------------------------------
// <copyright file="AgentStop.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.Common.Models.Streaming.Agent;

/// <summary>
/// An agent stop message.
/// </summary>
public record AgentEnd : BaseAgentItem
{
    /// <summary>
    /// Gets the end time.
    /// </summary>
    public DateTimeOffset EndTime { get; init; }

    /// <summary>
    /// Gets the duration.
    /// </summary>
    public TimeSpan Duration { get; init; }

    /// <summary>
    /// Creates a new <see cref="AgentEnd"/> object from an <see cref="AgentStart"/> object.
    /// </summary>
    /// <param name="agentStart">The agent start message.</param>
    /// <returns>A <see cref="AgentEnd"/>.</returns>
    public static AgentEnd FromAgentStart(AgentStart agentStart)
    {
        return new AgentEnd
        {
            EndTime = DateTimeOffset.UtcNow,
            Duration = DateTimeOffset.UtcNow.Subtract(agentStart.StartTime),
            Id = agentStart.Id,
            Name = agentStart.Name,
            ExecutionId = agentStart.ExecutionId,
        };
    }
}
