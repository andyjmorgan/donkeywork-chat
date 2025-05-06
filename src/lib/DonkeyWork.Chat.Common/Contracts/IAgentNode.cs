// ------------------------------------------------------
// <copyright file="IAgentNode.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Collections.Concurrent;
using DonkeyWork.Chat.Common.Models.Agents;
using DonkeyWork.Chat.Common.Models.Agents.Models;
using DonkeyWork.Chat.Common.Models.Agents.Results;

namespace DonkeyWork.Chat.Common.Contracts;

/// <summary>
/// An agent node.
/// </summary>
public interface IAgentNode
{
    /// <summary>
    /// Gets or sets the agent node id.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the node name.
    /// </summary>
    public ConcurrentBag<IAgentNode> InputNodes { get; set; }

    /// <summary>
    /// Gets or sets the output nodes.
    /// </summary>
    public ConcurrentBag<IAgentNode> OutputNodes { get; set; }

    /// <summary>
    /// Gets or sets the agent node status.
    /// </summary>
    public AgentNodeStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the agent node name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the agent node id.
    /// </summary>
    public AgentNodeType NodeType { get; set; }

    /// <summary>
    /// Executes the agent node.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task<BaseAgentNodeResult> ExecuteAsync(CancellationToken cancellationToken = default);
}
