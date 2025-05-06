// ------------------------------------------------------
// <copyright file="NodeEnd.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Models.Agents.Results;

namespace DonkeyWork.Chat.Common.Models.Streaming.Node;

/// <summary>
/// A node end message.
/// </summary>
public record NodeEnd : BaseNodeItem
{
    /// <summary>
    /// Gets the end time.
    /// </summary>
    public DateTimeOffset EndTime { get; init; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets the duration of the node.
    /// </summary>
    public TimeSpan Duration { get; init; }

    /// <summary>
    /// Gets the result.
    /// </summary>
    required public BaseAgentNodeResult Result { get; init; }

    /// <summary>
    /// Returns a <see cref="NodeEnd"/> object from a <see cref="NodeStart"/> object.
    /// </summary>
    /// <param name="nodeStart">The node start.</param>
    /// <param name="result">The result.</param>
    /// <returns>A <see cref="NodeEnd"/> message.</returns>
    public static NodeEnd FromNodeStart(NodeStart nodeStart, BaseAgentNodeResult result)
    {
        return new NodeEnd
        {
            Duration = DateTimeOffset.UtcNow.Subtract(nodeStart.StartTime),
            NodeId = nodeStart.NodeId,
            NodeName = nodeStart.NodeName,
            NodeType = nodeStart.NodeType,
            ExecutionId = nodeStart.ExecutionId,
            Result = result,
        };
    }
}
