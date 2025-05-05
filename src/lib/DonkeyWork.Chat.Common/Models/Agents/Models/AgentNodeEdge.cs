// ------------------------------------------------------
// <copyright file="AgentNodeEdge.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.Common.Models.Agents.Models;

/// <summary>
/// An agent node edge entity.
/// </summary>
public class AgentNodeEdge
{
    /// <summary>
    /// Gets or sets the edge id.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the source node id.
    /// </summary>
    public Guid SourceNodeId { get; set; }

    /// <summary>
    /// Gets or sets the target node id.
    /// </summary>
    public Guid TargetNodeId { get; set; }

    /// <summary>
    /// Gets or sets the source node handle label.
    /// </summary>
    public string? SourceNodeHandle { get; set; }

    /// <summary>
    /// Gets or sets the target node handle label.
    /// </summary>
    public string? TargetNodeHandle { get; set; }
}
