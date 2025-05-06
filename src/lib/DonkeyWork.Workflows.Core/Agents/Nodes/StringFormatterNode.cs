// ------------------------------------------------------
// <copyright file="StringFormatterNode.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Models.Agents.Metadata;
using DonkeyWork.Chat.Common.Models.Agents.Results;
using DonkeyWork.Workflows.Core.Agents.Execution;
using Microsoft.Extensions.Logging;
using Scriban;

namespace DonkeyWork.Workflows.Core.Agents.Nodes;

/// <summary>
/// A string formatter node.
/// </summary>
public class StringFormatterNode : BaseAgentNode
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StringFormatterNode"/> class.
    /// </summary>
    /// <param name="agentContext">The agent context.</param>
    /// <param name="logger">The logger.</param>
    public StringFormatterNode(IAgentContext agentContext, ILogger<BaseAgentNode> logger)
        : base(agentContext, logger)
    {
    }

    /// <summary>
    /// Gets or sets the model parameters.
    /// </summary>
    public AgentStringFormatterNodeMetadata Parameters { get; set; }

    /// <inheritdoc />
    protected internal override Task<BaseAgentNodeResult> ExecuteNodeAsync(List<BaseAgentNodeResult> inputs, CancellationToken cancellationToken = default)
    {
        var template = Template.Parse(this.Parameters.Template);
        var variableContext = new AgentVariableContext(inputs, this.AgentContext);

// Render the template with the variableContext and a custom MemberRenamer
        var result = template.Render(variableContext, member => member.Name);
        return Task.FromResult<BaseAgentNodeResult>(new StringFormatterNodeResult(this)
        {
            Inputs = inputs,
            FormattedText = result,
        });
    }
}
