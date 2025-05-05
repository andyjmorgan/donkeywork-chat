// ------------------------------------------------------
// <copyright file="IAgentRepository.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Persistence.Agent.Repository.Agent.Models;
using DonkeyWork.Persistence.Common.Common;

namespace DonkeyWork.Persistence.Agent.Repository.Agent;

/// <summary>
/// An agent repository.
/// </summary>
public interface IAgentRepository
{
    /// <summary>
    /// Gets all agents.
    /// </summary>
    /// <param name="pagingParameters">The paging parameters.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A response of <see cref="GetAgentsResponseItem"/>.</returns>
    Task<GetAgentsResponseItem> GetAgentsAsync(PagingParameters pagingParameters, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an agent by name.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An <see cref="AgentItem"/> if found, otherwise null.</returns>
    Task<AgentItem?> GetAgentByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an agent by id.
    /// </summary>
    /// <param name="id">The agent id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An <see cref="AgentItem"/> if found, otherwise null.</returns>
    Task<AgentItem?> GetAgentByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an agent by id for execution.
    /// </summary>
    /// <param name="id">The agent id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An <see cref="AgentItem"/> if found, otherwise null.</returns>
    Task<AgentItem?> GetAgentByIdForExecutionAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an agent.
    /// </summary>
    /// <param name="id">The agent id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>True if deleted, false if not found.</returns>
    Task<bool> DeleteAgentAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an agent.
    /// </summary>
    /// <param name="id">The agent id.</param>
    /// <param name="agent">The agent data.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>True if updated, false if not found.</returns>
    Task<bool> UpdateAgentAsync(Guid id, UpsertAgentItem agent, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds an agent.
    /// </summary>
    /// <param name="agent">The agent data.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The id of the newly created agent.</returns>
    Task<Guid> AddAgentAsync(UpsertAgentItem agent, CancellationToken cancellationToken = default);

    /// <summary>
    /// Increments the execution count for an agent.
    /// </summary>
    /// <param name="id">The agent id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>True if incremented, false if not found.</returns>
    Task<bool> IncrementExecutionCountAsync(Guid id, CancellationToken cancellationToken = default);
}
