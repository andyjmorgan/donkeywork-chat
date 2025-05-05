// ------------------------------------------------------
// <copyright file="GetAgentModel.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Api.Models.Agent.NodeData;
using DonkeyWork.Persistence.Agent.Repository.Agent.Models;

namespace DonkeyWork.Chat.Api.Models.Agent;

/// <summary>
/// Model for creating or updating an agent using raw JSON from frontend.
/// </summary>
public class GetAgentModel : BaseAgentModel
{
    /// <summary>
    /// Converts the model to an <see cref="UpsertAgentItem"/> object.
    /// </summary>
    /// <param name="agentItem">The agent item.</param>
    /// <returns>A <see cref="GetAgentModel"/>.</returns>
    public static GetAgentModel FromAgentItem(AgentItem agentItem)
    {
        return new GetAgentModel()
        {
            Description = agentItem.Description,
            Id = agentItem.Id,
            Name = agentItem.Name,
            Tags = agentItem.Tags,
            NodeEdges = agentItem.NodeEdges,
            Nodes = agentItem.Nodes.Select(x => new AgentNodeDto
            {
                Id = x.Id,
                Label = x.Label,
                Metadata = BaseNodeDataDto.FromMetadata(x.Metadata),
                NodeType = x.NodeType,
                Position = x.Position,
            }).ToList(),
        };
    }
}
