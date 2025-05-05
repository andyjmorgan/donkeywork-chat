// ------------------------------------------------------
// <copyright file="AgentNodeDto.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Text.Json.Serialization;
using DonkeyWork.Chat.Api.Models.Agent.NodeData;
using DonkeyWork.Chat.Common.Models.Agents.Models;

namespace DonkeyWork.Chat.Api.Models.Agent;

/// <summary>
/// Agent node DTO for use with the domain model.
/// </summary>
/// <remarks>
/// This is NOT used for the raw frontend data, which now uses object type in UpsertAgentModel.
/// </remarks>
public class AgentNodeDto
{
    /// <summary>
    /// Gets or sets the node id.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the agent node label.
    /// </summary>
    [JsonPropertyName("label")]
    required public string Label { get; set; }

    /// <summary>
    /// Gets or sets the agent node type.
    /// </summary>
    public AgentNodeType NodeType { get; set; }

    /// <summary>
    /// Gets or sets the agent node position.
    /// </summary>
    required public AgentNodePosition Position { get; set; }

    /// <summary>
    /// Gets or sets the agent node metadata.
    /// </summary>
    [JsonPropertyName("metadata")]
    required public BaseNodeDataDto Metadata { get; set; }
}
