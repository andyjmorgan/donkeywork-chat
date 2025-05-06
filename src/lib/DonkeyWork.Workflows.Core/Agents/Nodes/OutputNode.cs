// ------------------------------------------------------
// <copyright file="OutputNode.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Models.Agents.Results;
using DonkeyWork.Workflows.Core.Agents.Execution;
using Microsoft.Extensions.Logging;

namespace DonkeyWork.Workflows.Core.Agents.Nodes;

/// <summary>
/// An output node.
/// </summary>
public class OutputNode : BaseAgentNode
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OutputNode"/> class.
    /// </summary>
    /// <param name="agentContext">The agent context.</param>
    /// <param name="logger">The logger.</param>
    public OutputNode(IAgentContext agentContext, ILogger<BaseAgentNode> logger)
        : base(agentContext, logger)
    {
    }

    /// <inheritdoc />
    protected internal override Task<BaseAgentNodeResult> ExecuteNodeAsync(List<BaseAgentNodeResult> inputs, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<BaseAgentNodeResult>(
            new OutputNodeResult(this)
            {
                Inputs = inputs,
            });
    }
}
