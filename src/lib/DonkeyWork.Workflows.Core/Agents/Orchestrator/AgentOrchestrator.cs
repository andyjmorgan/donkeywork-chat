// ------------------------------------------------------
// <copyright file="AgentOrchestrator.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.AiServices.Clients.Models;
using DonkeyWork.Chat.Common.Contracts;
using DonkeyWork.Chat.Common.Models.Agents;
using DonkeyWork.Chat.Common.Models.Chat;
using DonkeyWork.Chat.Common.Models.Streaming.Agent;
using DonkeyWork.Workflows.Core.Agents.Composer;
using DonkeyWork.Workflows.Core.Agents.Execution;
using DonkeyWork.Workflows.Core.Agents.Nodes;
using Microsoft.Extensions.Logging;

namespace DonkeyWork.Workflows.Core.Agents.Orchestrator;

/// <inheritdoc />
public class AgentOrchestrator(
    IAgentComposer agentComposer,
    ILogger<AgentOrchestrator> logger,
    IAgentContext agentContext)
    : IAgentOrchestrator
{
    /// <inheritdoc />
    public async Task ExecuteAsync(
        Guid agentId,
        List<GenericChatMessage> messages,
        CancellationToken cancellationToken = default)
    {
        // Compose the agent.
        logger.LogInformation("Composing agent {AgentId}", agentId);
        var agent = await agentComposer.ComposeAgentAsync(agentId, cancellationToken);

        var agentStart = new AgentStart()
        {
            Id = agentId,
            Name = agentContext.ExecutionDetails.AgentName,
            StartTime = DateTime.UtcNow,
            ExecutionId = agentContext.ExecutionDetails.ExecutionId,
        };

        await agentContext.StreamService.AddStreamItem(agentStart, cancellationToken);
        agentContext.InputDetails = new AgentInputDetails { Headers = [], Messages = messages, };

        await this.ExecuteNodesAsync(agent, cancellationToken);
        var result = agentContext.NodeResults[agent.OfType<OutputNode>().First().Id];
        await agentContext.StreamService.AddStreamItem(AgentEnd.FromAgentStart(agentStart), cancellationToken);
        await agentContext.StreamService.FinalizeStream(cancellationToken);
    }

    private async Task ExecuteNodesAsync(List<IAgentNode> agent, CancellationToken cancellationToken)
    {
        // Get nodes that are not started and are ready to execute
        var readyNodes = agent
            .Where(x => x.Status == AgentNodeStatus.NotStarted &&
                        (x.InputNodes.Count == 0 || x.InputNodes.All(input => input.Status == AgentNodeStatus.Completed)))
            .ToList();

        if (!readyNodes.Any())
        {
            return;
        }

        // Execute all ready nodes
        var tasks = readyNodes.Select(node => node.ExecuteAsync(cancellationToken)).ToList();
        await Task.WhenAll(tasks);

        // Recurse to process the next set of nodes
        await this.ExecuteNodesAsync(agent, cancellationToken);
    }
}
