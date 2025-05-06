// ------------------------------------------------------
// <copyright file="BaseNodeResult.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Models.Agents.Models;

namespace DonkeyWork.Chat.Common.Models.Streaming.Node;

/// <summary>
/// A base node item.
/// </summary>
public abstract record BaseNodeItem : BaseStreamItem
{
    /// <summary>
    /// Gets the node id.
    /// </summary>
    public Guid NodeId { get; init; }

    /// <summary>
    /// Gets the node name.
    /// </summary>
    public string NodeName { get; init; } = string.Empty;

    /// <summary>
    /// Gets the node type.
    /// </summary>
    public AgentNodeType NodeType { get; init; }
}
