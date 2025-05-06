// ------------------------------------------------------
// <copyright file="BaseAgentNodeResult.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Text.Json.Serialization;
using DonkeyWork.Chat.Common.Contracts;
using DonkeyWork.Chat.Common.Models.Agents.Models;
using DonkeyWork.Chat.Common.Models.Agents.Results.FlowControl;

namespace DonkeyWork.Chat.Common.Models.Agents.Results;

/// <summary>
/// An agent string result.
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "ResultType")]
[JsonDerivedType(typeof(InputNodeResult), typeDiscriminator: nameof(InputNodeResult))]
[JsonDerivedType(typeof(ModelNodeResult), typeDiscriminator: nameof(ModelNodeResult))]
[JsonDerivedType(typeof(OutputNodeResult), typeDiscriminator: nameof(OutputNodeResult))]
[JsonDerivedType(typeof(StringFormatterNodeResult), typeDiscriminator: nameof(StringFormatterNodeResult))]
[JsonDerivedType(typeof(ConditionalNodeResult), typeDiscriminator: nameof(ConditionalNodeResult))]
[JsonDerivedType(typeof(ExceptionNodeResult), typeDiscriminator: nameof(ExceptionNodeResult))]
public abstract class BaseAgentNodeResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseAgentNodeResult"/> class.
    /// </summary>
    /// <param name="agentNode">The agent node.</param>
    protected BaseAgentNodeResult(IAgentNode agentNode)
    {
        this.Id = agentNode.Id;
        this.Name = agentNode.Name;
        this.NodeType = agentNode.NodeType;
    }

    /// <summary>
    /// Gets or sets the node id.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the node name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the node type.
    /// </summary>
    public AgentNodeType NodeType { get; set; }

    /// <summary>
    /// Gets a string value for the result.
    /// </summary>
    /// <returns>A string representation of the result.</returns>
    public abstract string Text();
}
