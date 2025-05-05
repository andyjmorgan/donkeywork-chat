// ------------------------------------------------------
// <copyright file="AgentEntity.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.ComponentModel.DataAnnotations.Schema;
using DonkeyWork.Chat.Common.Models.Agents.Models;
using DonkeyWork.Persistence.Common.Entity.Base;

namespace DonkeyWork.Persistence.Agent.Entity.Agent;

/// <summary>
/// Gets or sets the agent entity.
/// </summary>
public class AgentEntity : BaseUserEntity
{
    /// <summary>
    /// Gets or sets the agent name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the agent description.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the execution count.
    /// </summary>
    public int ExecutionCount { get; set; }

    /// <summary>
    /// Gets or sets the tags.
    /// </summary>
    [Column(TypeName = "jsonb")]
    public List<string> Tags { get; set; } = [];

    /// <summary>
    /// Gets or sets the agent node edges navigation property.
    /// </summary>
    [Column(TypeName = "jsonb")]
    public List<AgentNode> Nodes { get; set; } = [];

    /// <summary>
    /// Gets or sets the agent node edges navigation property.
    /// </summary>
    [Column(TypeName = "jsonb")]
    public List<AgentNodeEdge> NodeEdges { get; set; } = [];
}
