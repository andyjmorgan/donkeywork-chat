// ------------------------------------------------------
// <copyright file="IActionRepository.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Persistence.Agent.Repository.Action.Models;
using DonkeyWork.Persistence.Agent.Repository.Action.Models.Execution;
using DonkeyWork.Persistence.Common.Common;

namespace DonkeyWork.Persistence.Agent.Repository.Action;

/// <summary>
/// An action repository.
/// </summary>
public interface IActionRepository
{
    /// <summary>
    /// Gets all actions.
    /// </summary>
    /// <param name="pagingParameters">The paging parameters.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A response of <see cref="GetActionsResponseItem"/>.</returns>
    Task<GetActionsResponseItem> GetActionsAsync(PagingParameters pagingParameters, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an action by name.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An <see cref="ActionItem"/> if found, otherwise null.</returns>
    Task<ActionItem?> GetActionByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an action by id.
    /// </summary>
    /// <param name="id">The action id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An <see cref="ActionItem"/> if found, otherwise null.</returns>
    Task<ActionItem?> GetActionByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an action by id for execution.
    /// </summary>
    /// <param name="id">The action id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An <see cref="ActionItem"/> if found, otherwise null.</returns>
    Task<ActionExecutionItem?> GetActionByIdForExecutionAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an action.
    /// </summary>
    /// <param name="id">The action id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>True if deleted, false if not found.</returns>
    Task<bool> DeleteActionAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an action.
    /// </summary>
    /// <param name="id">The action id.</param>
    /// <param name="action">The action data.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>True if updated, false if not found.</returns>
    Task<bool> UpdateActionAsync(Guid id, UpsertActionItem action, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds an action.
    /// </summary>
    /// <param name="action">The action data.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>True if added successfully.</returns>
    Task<bool> AddActionAsync(UpsertActionItem action, CancellationToken cancellationToken = default);
}