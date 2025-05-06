// ------------------------------------------------------
// <copyright file="AgentComposer.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Contracts;
using DonkeyWork.Chat.Common.Models.Agents.Metadata;
using DonkeyWork.Chat.Common.Models.Agents.Models;
using DonkeyWork.Chat.Common.Services.UserContext;
using DonkeyWork.Persistence.Agent.Repository.Agent;
using DonkeyWork.Persistence.Agent.Repository.Agent.Models;
using DonkeyWork.Persistence.Agent.Repository.Prompt;
using DonkeyWork.Workflows.Core.Agents.Execution;
using DonkeyWork.Workflows.Core.Agents.Nodes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DonkeyWork.Workflows.Core.Agents.Composer;

/// <summary>
/// Composes an agent from the entity.
/// </summary>
public class AgentComposer(
    ILogger<AgentComposer> logger,
    IAgentRepository agentRepository,
    IServiceProvider serviceProvider,
    IPromptRepository promptRepository,
    IAgentContext agentContext,
    IUserContextProvider userContextProvider,
    IUserPostureService userPostureService)
    : IAgentComposer
{
    /// <inheritdoc />
    public async Task<List<IAgentNode>> ComposeAgentAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var agent = await agentRepository.GetAgentByIdAsync(id, cancellationToken);
        ArgumentNullException.ThrowIfNull(agent);
        var nodes = new List<IAgentNode>();
        foreach (var node in agent.Nodes)
        {
            switch (node.NodeType)
            {
                case AgentNodeType.Input:
                    var inputNode = this.CreateNodeType<InputNode>(node);
                    nodes.Add(inputNode);
                    break;

                case AgentNodeType.Model:
                    var modelNode = this.CreateNodeType<ModelNode>(node);
                    modelNode.Parameters = (AgentModelNodeMetadata)node.Metadata;
                    nodes.Add(modelNode);
                    break;

                case AgentNodeType.StringFormatter:
                    var stringFormatterNode = this.CreateNodeType<StringFormatterNode>(node);
                    stringFormatterNode.Parameters = (AgentStringFormatterNodeMetadata)node.Metadata;
                    nodes.Add(stringFormatterNode);
                    break;

                case AgentNodeType.Output:
                    var outputNode = this.CreateNodeType<OutputNode>(node);
                    nodes.Add(outputNode);
                    break;

                default:
                    throw new NotImplementedException("Node type not implemented");
            }
        }

        this.LinkNodes(nodes, agent);
        await this.HydrateContextAsync(agent, nodes, cancellationToken);

        return nodes;
    }

    private void LinkNodes(List<IAgentNode> nodes, AgentItem agent)
    {
        var nodeDictionary = nodes.ToDictionary(node => node.Id);
        foreach (var node in nodes.OfType<BaseAgentNode>())
        {
            var nodeDependsOn = agent.NodeEdges.Where(x => x.TargetNodeId == node.Id);
            var nodeDependents = agent.NodeEdges.Where(x => x.SourceNodeId == node.Id);

            foreach (var dependsOn in nodeDependsOn)
            {
                if (nodeDictionary.TryGetValue(dependsOn.SourceNodeId, out var sourceNode))
                {
                    node.InputNodes.Add(sourceNode);
                }
            }

            foreach (var dependent in nodeDependents)
            {
                if (nodeDictionary.TryGetValue(dependent.TargetNodeId, out var targetNode))
                {
                    node.OutputNodes.Add(targetNode);
                }
            }
        }
    }

    private async Task HydrateContextAsync(AgentItem agent, List<IAgentNode> nodes, CancellationToken cancellationToken)
    {
        agentContext.ExecutionDetails = new AgentExecutionDetails()
        {
            AgentId = agent.Id,
            AgentName = agent.Name,
            UserId = userContextProvider.UserId,
        };

        agentContext.UserPosture = await userPostureService.GetUserPosturesAsync(cancellationToken);

        var prompts = nodes.OfType<ModelNode>()
            .Select(x => x.Parameters)
            .SelectMany(x => x.SystemPrompts)
            .Distinct();

        foreach (var prompt in prompts)
        {
            var promptContent = await promptRepository.GetPromptContentAsync(prompt, cancellationToken);
            if (promptContent is null)
            {
                logger.LogError("Prompt content not found for prompt {PromptId}", prompt);
                continue;
            }

            agentContext.Prompts[prompt] = promptContent;
        }
    }

    private T CreateNodeType<T>(AgentNode node)
        where T : IAgentNode
    {
        var agentStep = ActivatorUtilities.CreateInstance<T>(serviceProvider);
        agentStep.Id = node.Id;
        agentStep.Name = node.Label;
        agentStep.NodeType = node.NodeType;
        return agentStep;
    }
}
