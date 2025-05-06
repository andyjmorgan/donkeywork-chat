// ------------------------------------------------------
// <copyright file="AgentContext.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Collections.Concurrent;
using DonkeyWork.Chat.Common.Models.Agents.Results;
using DonkeyWork.Chat.Common.Models.Providers.Posture;
using DonkeyWork.Persistence.Agent.Repository.Prompt.Models.SystemPrompt;
using DonkeyWork.Workflows.Core.Agents.Stream;

namespace DonkeyWork.Workflows.Core.Agents.Execution;

/// <summary>
/// Gets or sets the agent execution context.
/// </summary>
public class AgentContext : IAgentContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AgentContext"/> class.
    /// </summary>
    /// <param name="streamService">The stream service.</param>
    public AgentContext(IStreamService streamService)
    {
        this.StreamService = streamService;
    }

    /// <summary>
    /// Gets or sets the agent execution context.
    /// </summary>
    public AgentExecutionDetails ExecutionDetails { get; set; }

    /// <summary>
    /// Gets or sets the agent input details.
    /// </summary>
    public AgentInputDetails InputDetails { get; set; }

    /// <summary>
    /// Gets or sets the user posture.
    /// </summary>
    public ToolProviderPosture UserPosture { get; set; }

    /// <summary>
    /// Gets or sets the system prompts.
    /// </summary>
    public Dictionary<Guid, PromptContentItem> Prompts { get; set; } = [];

    /// <summary>
    /// The agent node results.
    /// </summary>
    public ConcurrentDictionary<Guid, BaseAgentNodeResult> NodeResults { get; set; } = [];

    /// <inheritdoc />
    public IStreamService StreamService { get; set; }

    /// <inheritdoc />
    public void AddNodeResult(Guid id, BaseAgentNodeResult nodeResult)
    {
        this.NodeResults.TryAdd(id, nodeResult);
    }

    /// <inheritdoc />
    public List<BaseAgentNodeResult> GetDependentNodeResults(List<Guid> dependents)
    {
        return this.NodeResults
            .Where(
                x =>
                    dependents.Contains(x.Key))
            .Select(
                x =>
                    x.Value)
            .ToList();
    }
}
