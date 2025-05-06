// ------------------------------------------------------
// <copyright file="BaseAgentNode.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using DonkeyWork.Chat.Common.Contracts;
using DonkeyWork.Chat.Common.Models.Agents;
using DonkeyWork.Chat.Common.Models.Agents.Models;
using DonkeyWork.Chat.Common.Models.Agents.Results;
using DonkeyWork.Chat.Common.Models.Agents.Results.FlowControl;
using DonkeyWork.Chat.Common.Models.Streaming.Node;
using DonkeyWork.Workflows.Core.Agents.Execution;
using Microsoft.Extensions.Logging;

namespace DonkeyWork.Workflows.Core.Agents.Nodes;

/// <inheritdoc />
[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Reviewed.")]
public abstract class BaseAgentNode : IAgentNode
{
    /// <summary>
    /// Gets the logger.
    /// </summary>
    protected readonly ILogger<BaseAgentNode> Logger;

    /// <summary>
    /// Gets the agent execution context.
    /// </summary>
    protected readonly IAgentContext AgentContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseAgentNode"/> class.
    /// </summary>
    /// <param name="agentContext">The agent execution context.</param>
    /// <param name="logger">The logger.</param>
    public BaseAgentNode(
        IAgentContext agentContext,
        ILogger<BaseAgentNode> logger)
    {
        this.AgentContext = agentContext;
        this.Logger = logger;
    }

    /// <inheritdoc />
    public ConcurrentBag<IAgentNode> InputNodes { get; set; } = [];

    /// <inheritdoc />
    public ConcurrentBag<IAgentNode> OutputNodes { get; set; } = [];

    /// <inheritdoc />
    required public Guid Id { get; set; }

    /// <inheritdoc />
    public AgentNodeStatus Status { get; set; } = AgentNodeStatus.NotStarted;

    /// <inheritdoc />
    required public string Name { get; set; }

    /// <summary>
    /// Gets or sets the agent node type.
    /// </summary>
    public AgentNodeType NodeType { get; set; }

    /// <inheritdoc />
    public async Task<BaseAgentNodeResult> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var nodeStart = new NodeStart()
            {
                NodeType = this.NodeType,
                ExecutionId = this.AgentContext.ExecutionDetails.ExecutionId,
                NodeId = this.Id,
                NodeName = this.Name,
            };
            await this.AgentContext.StreamService.AddStreamItem(nodeStart, cancellationToken);
            this.Status = AgentNodeStatus.InProgress;
            this.Logger.LogInformation("Executing node {NodeName} with Id: {NodeId}", this.Name, this.Id);
            var dependentResults = this.AgentContext
                .GetDependentNodeResults(this.InputNodes.Select(x => x.Id)
                    .ToList());

            if (dependentResults.OfType<ExceptionNodeResult>().Any())
            {
                return new ExceptionNodeResult(this)
                {
                    Message = "An error occurred in a previous node.",
                };
            }

            var result = await this.ExecuteNodeAsync(dependentResults, cancellationToken);
            this.Logger.LogInformation("Executed node {NodeName} with Id: {NodeId}", this.Name, this.Id);
            this.AgentContext.AddNodeResult(this.Id, result);
            await this.AgentContext.StreamService.AddStreamItem(NodeEnd.FromNodeStart(nodeStart, result), cancellationToken);
            return result;
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "Error executing node {NodeName} with Id: {NodeId}", this.Name, this.Id);
            var result = new ExceptionNodeResult(this)
            {
                Message = ex.Message,
                Exception = ex,
            };
            this.AgentContext.AddNodeResult(this.Id, result);
            return result;
        }
        finally
        {
            this.Status = AgentNodeStatus.Completed;
        }
    }

    /// <summary>
    /// Executes the agent node.
    /// </summary>
    /// <param name="inputs">The step inputs.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    protected internal abstract Task<BaseAgentNodeResult> ExecuteNodeAsync(List<BaseAgentNodeResult> inputs, CancellationToken cancellationToken = default);
}
