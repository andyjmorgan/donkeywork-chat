// ------------------------------------------------------
// <copyright file="IAgentExecutionContext.cs" company="DonkeyWork.Dev">
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
/// An agent execution context.
/// </summary>
public interface IAgentContext
{
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
    /// The system prompts.
    /// </summary>
    public Dictionary<Guid, PromptContentItem> Prompts { get; set; }

    /// <summary>
    /// Gets or sets the agent node results.
    /// </summary>
    public ConcurrentDictionary<Guid, BaseAgentNodeResult> NodeResults { get; set; }

    /// <summary>
    /// Adds a node result to the agent execution context.
    /// </summary>
    /// <param name="id">The node id.</param>
    /// <param name="nodeResult">THe node result.</param>
    public void AddNodeResult(Guid id, BaseAgentNodeResult nodeResult);

    /// <summary>
    /// Gets the results for the given dependents.
    /// </summary>
    /// <param name="dependents">The dependents.</param>
    /// <returns>A <see cref="List{T}"/> of <see cref="BaseAgentNodeResult"/>.</returns>
    public List<BaseAgentNodeResult> GetDependentNodeResults(List<Guid> dependents);

    /// <summary>
    /// Gets or sets the stream service.
    /// </summary>
    public IStreamService StreamService { get; set; }
}
