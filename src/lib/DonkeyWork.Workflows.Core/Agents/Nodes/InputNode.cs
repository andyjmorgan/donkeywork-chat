// ------------------------------------------------------
// <copyright file="InputNode.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.AiServices.Clients.Models;
using DonkeyWork.Chat.Common.Models.Agents.Results;
using DonkeyWork.Chat.Common.Models.Chat;
using DonkeyWork.Workflows.Core.Agents.Execution;
using Microsoft.Extensions.Logging;

namespace DonkeyWork.Workflows.Core.Agents.Nodes;

/// <summary>
/// An input node.
/// </summary>
public class InputNode : BaseAgentNode
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InputNode"/> class.
    /// </summary>
    /// <param name="agentContext">The agent context.</param>
    /// <param name="logger">The logger.</param>
    public InputNode(IAgentContext agentContext, ILogger<BaseAgentNode> logger)
        : base(agentContext, logger)
    {
    }

    /// <inheritdoc />
    protected internal override Task<BaseAgentNodeResult> ExecuteNodeAsync(List<BaseAgentNodeResult> inputs, CancellationToken cancellationToken = default)
    {
        this.Logger.LogInformation("Executing input node {NodeId}", this.Id);
        return Task.FromResult<BaseAgentNodeResult>(
            new InputNodeResult(this)
            {
                Message = this.AgentContext.InputDetails.LastMessage ?? new GenericChatMessage
                {
                    Content = string.Empty, Role = GenericMessageRole.User,
                },
            });
    }
}
