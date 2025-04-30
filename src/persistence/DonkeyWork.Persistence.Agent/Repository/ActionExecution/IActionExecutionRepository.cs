// ------------------------------------------------------
// <copyright file="IActionExecutionRepository.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Models.Actions;
using DonkeyWork.Chat.Common.Models.Streaming;
using DonkeyWork.Persistence.Agent.Repository.ActionExecution.Models;
using DonkeyWork.Persistence.Common.Common;

namespace DonkeyWork.Persistence.Agent.Repository.ActionExecution;

/// <summary>
/// A repository for action execution.
/// </summary>
public interface IActionExecutionRepository
{
    /// <summary>
    /// Creates a task for the specified action execution.
    /// </summary>
    /// <param name="executionId">The execution id.</param>
    /// <param name="actionId">The action id.</param>
    /// <param name="actionName">The action name.</param>
    /// <param name="cancellationToken">THe cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task<Guid> CreateTaskAsync(Guid executionId, Guid actionId, string actionName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets the task status for the specified task id.
    /// </summary>
    /// <param name="executionId">The task id.</param>
    /// <param name="cancellationToken">THe cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task SetTaskRunningAsync(Guid executionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets the task status for the specified task id.
    /// </summary>
    /// <param name="executionId">The task id.</param>
    /// <param name="results">The results.</param>
    /// <param name="status">The status.</param>
    /// <param name="cancellationToken">THe cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task SetTaskCompletedStatusAsync(Guid executionId, List<BaseStreamItem> results, ActionExecutionStatus status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the action executions.
    /// </summary>
    /// <param name="pagingParameters">The paging parameters.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task<GetActionExecutionsItem> GetActionExecutionsAsync(PagingParameters pagingParameters, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the action executions by action ID.
    /// </summary>
    /// <param name="actionId">The action ID to filter by.</param>
    /// <param name="pagingParameters">The paging parameters.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task<GetActionExecutionsItem> GetActionExecutionsByActionIdAsync(Guid actionId, PagingParameters pagingParameters, CancellationToken cancellationToken = default);
}