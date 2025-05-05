// ------------------------------------------------------
// <copyright file="BaseAgentModel.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Text.Json.Serialization;
using DonkeyWork.Chat.Common.Models.Agents.Models;

namespace DonkeyWork.Chat.Api.Models.Agent;

/// <summary>
/// A base agent model.
/// </summary>
public abstract class BaseAgentModel
{
    /// <summary>
    /// Gets or sets the ID for the agent.
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
    /// Gets or sets the tags.
    /// </summary>
    public List<string> Tags { get; set; } = [];

    /// <summary>
    /// Gets or sets the raw nodes from frontend.
    /// </summary>
    [JsonPropertyName("nodes")]
    public List<AgentNodeDto> Nodes { get; set; } = [];

    /// <summary>
    /// Gets or sets the raw node edges from frontend.
    /// </summary>
    [JsonPropertyName("nodeEdges")]
    public List<AgentNodeEdge> NodeEdges { get; set; } = [];
}
