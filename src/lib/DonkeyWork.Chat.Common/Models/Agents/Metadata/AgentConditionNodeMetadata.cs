// ------------------------------------------------------
// <copyright file="AgentConditionNodeMetadata.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Persistence.Agent.Entity.Agent.Models;

namespace DonkeyWork.Persistence.Agent.Entity.Agent.Metadata;

/// <summary>
/// Gets or sets the agent condition node metadata.
/// </summary>
public class AgentConditionNodeMetadata : AgentNodeBaseMetadata
{
    /// <summary>
    /// Gets or sets the condition node id.
    /// </summary>
    public List<ConditionItem> Conditions { get; set; } = [];
}
