// ------------------------------------------------------
// <copyright file="AgentNodeBaseMetadata.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Text.Json.Serialization;

namespace DonkeyWork.Persistence.Agent.Entity.Agent.Metadata;

/// <summary>
/// A base agent node metadata entity.
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(AgentModelNodeMetadata), nameof(AgentModelNodeMetadata))]
[JsonDerivedType(typeof(AgentStringFormatterNodeMetadata), nameof(AgentStringFormatterNodeMetadata))]
[JsonDerivedType(typeof(AgentConditionNodeMetadata), nameof(AgentConditionNodeMetadata))]
public abstract class AgentNodeBaseMetadata
{
}
