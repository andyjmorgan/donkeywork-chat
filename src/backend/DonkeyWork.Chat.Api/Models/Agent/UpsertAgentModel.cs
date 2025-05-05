// ------------------------------------------------------
// <copyright file="UpsertAgentModel.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Models.Agents.Models;
using DonkeyWork.Persistence.Agent.Repository.Agent.Models;

namespace DonkeyWork.Chat.Api.Models.Agent;

/// <summary>
/// Model for creating or updating an agent using raw JSON from frontend.
/// </summary>
public class UpsertAgentModel : BaseAgentModel
{
    /// <summary>
    /// Converts the model to an <see cref="UpsertAgentItem"/> object.
    /// </summary>
    /// <returns>A <see cref="UpsertAgentItem"/>.</returns>
    public UpsertAgentItem ToAgentItem()
    {
        return new UpsertAgentItem
        {
            Name = this.Name,
            Description = this.Description,
            Id = this.Id,
            Tags = this.Tags,
            NodeEdges = this.NodeEdges,
            Nodes = this.Nodes.Select(x => new AgentNode()
            {
                Id = x.Id,
                Label = x.Label,
                Position = x.Position,
                NodeType = x.NodeType,
                Metadata = x.Metadata.GetMetadata(),
            }).ToList(),
        };
    }
}
