// ------------------------------------------------------
// <copyright file="AgentNode.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Models.Agents.Metadata;

namespace DonkeyWork.Chat.Common.Models.Agents.Models;

/// <summary>
/// Agent node entity.
/// </summary>
public class AgentNode
{
    /// <summary>
    /// Gets or sets the node id.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the agent node label.
    /// </summary>
    required public string Label { get; set; }

    /// <summary>
    /// Gets or sets the agent node type.
    /// </summary>
    public AgentNodeType NodeType { get; set; }

    /// <summary>
    /// Gets or sets the agent node position.
    /// </summary>
    public AgentNodePosition Position { get; set; }

    /// <summary>
    /// Gets or sets the agent node position.
    /// </summary>
    required public AgentNodeBaseMetadata Metadata { get; set; }
}
