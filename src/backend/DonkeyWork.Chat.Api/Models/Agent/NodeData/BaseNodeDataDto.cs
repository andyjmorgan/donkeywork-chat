// ------------------------------------------------------
// <copyright file="BaseNodeDataDto.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Text.Json.Serialization;
using DonkeyWork.Chat.Common.Models.Agents.Metadata;

namespace DonkeyWork.Chat.Api.Models.Agent.NodeData
{
    /// <summary>
    /// Base DTO for all node data types.
    /// </summary>
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "nodeType")]
    [JsonDerivedType(typeof(InputNodeDataDto), typeDiscriminator: "input")]
    [JsonDerivedType(typeof(OutputNodeDataDto), typeDiscriminator: "output")]
    [JsonDerivedType(typeof(ModelNodeDataDto), typeDiscriminator: "model")]
    [JsonDerivedType(typeof(StringFormatterNodeDataDto), typeDiscriminator: "stringFormatter")]
    [JsonDerivedType(typeof(ConditionalNodeDataDto), typeDiscriminator: "conditional")]
    public abstract class BaseNodeDataDto
    {
        /// <summary>
        /// Allows the node to be converted to metadata.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        /// <returns>A <see cref="BaseNodeDataDto"/>.</returns>
        internal static BaseNodeDataDto FromMetadata(AgentNodeBaseMetadata metadata)
        {
            if (metadata is AgentModelNodeMetadata modelMetadata)
            {
                return new ModelNodeDataDto()
                {
                    ModelName = modelMetadata.ModelConfiguration.ModelName,
                    Streaming = modelMetadata.ModelConfiguration.Streaming,
                    AllowedTools = modelMetadata.AllowedTools.Select(x => x.ToString()).ToList(),
                    DynamicTools = modelMetadata.IsDynamicTooling,
                    ProviderType = modelMetadata.ModelConfiguration.ProviderType,
                    SystemPromptIds = modelMetadata.SystemPrompts,
                    ModelParameters = modelMetadata.ModelConfiguration.Metadata,
                };
            }

            if (metadata is AgentInputNodeMetadata _)
            {
                return new InputNodeDataDto();
            }

            if (metadata is AgentOutputNodeMetadata _)
            {
                return new InputNodeDataDto();
            }

            if (metadata is AgentConditionNodeMetadata conditionMetadata)
            {
                return new ConditionalNodeDataDto() { Conditions = conditionMetadata.Conditions, };
            }

            if (metadata is AgentStringFormatterNodeMetadata stringFormatterMetadata)
            {
                return new StringFormatterNodeDataDto() { Template = stringFormatterMetadata.Template, };
            }

            throw new NotImplementedException(metadata.GetType().ToString());
        }

        /// <summary>
        /// Gets the metadata for the node.
        /// </summary>
        /// <returns>A <see cref="AgentNodeBaseMetadata"/>.</returns>
        internal abstract AgentNodeBaseMetadata GetMetadata();
    }
}
