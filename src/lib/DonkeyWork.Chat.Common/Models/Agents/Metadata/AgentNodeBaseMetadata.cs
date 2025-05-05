// ------------------------------------------------------
// <copyright file="AgentNodeBaseMetadata.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Text.Json.Serialization;

namespace DonkeyWork.Chat.Common.Models.Agents.Metadata;

/// <summary>
/// A base agent node metadata entity.
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(AgentModelNodeMetadata), nameof(AgentModelNodeMetadata))]
[JsonDerivedType(typeof(AgentStringFormatterNodeMetadata), nameof(AgentStringFormatterNodeMetadata))]
[JsonDerivedType(typeof(AgentConditionNodeMetadata), nameof(AgentConditionNodeMetadata))]
[JsonDerivedType(typeof(AgentInputNodeMetadata), nameof(AgentInputNodeMetadata))]
[JsonDerivedType(typeof(AgentOutputNodeMetadata), nameof(AgentOutputNodeMetadata))]
public abstract class AgentNodeBaseMetadata
{
}
