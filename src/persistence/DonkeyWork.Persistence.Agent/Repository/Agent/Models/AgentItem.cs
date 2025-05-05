// ------------------------------------------------------
// <copyright file="AgentItem.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Models.Agents.Models;

namespace DonkeyWork.Persistence.Agent.Repository.Agent.Models;

/// <summary>
/// Represents an agent item.
/// </summary>
public class AgentItem
{
    /// <summary>
    /// Gets or sets the id.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the execution count.
    /// </summary>
    public int ExecutionCount { get; set; }

    /// <summary>
    /// Gets or sets the tags.
    /// </summary>
    public List<string> Tags { get; set; } = [];

    /// <summary>
    /// Gets or sets the agent node edges navigation property.
    /// </summary>
    public List<AgentNode> Nodes { get; set; } = [];

    /// <summary>
    /// Gets or sets the agent node edges navigation property.
    /// </summary>
    public List<AgentNodeEdge> NodeEdges { get; set; } = [];

    /// <summary>
    /// Gets or sets the created at date.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the updated at date.
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }
}