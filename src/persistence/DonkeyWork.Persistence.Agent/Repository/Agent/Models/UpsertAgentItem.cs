// ------------------------------------------------------
// <copyright file="UpsertAgentItem.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Models.Agents.Models;

namespace DonkeyWork.Persistence.Agent.Repository.Agent.Models;

/// <summary>
/// Represents data for creating or updating an agent.
/// </summary>
public class UpsertAgentItem
{
    /// <summary>
    /// Gets or sets the ID for the agent.
    /// If provided during creation, this ID will be used instead of generating a new one.
    /// </summary>
    public Guid? Id { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    public string Description { get; set; } = string.Empty;

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
}
