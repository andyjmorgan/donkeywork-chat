// ------------------------------------------------------
// <copyright file="IActionOrchestratorService.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Models.Streaming;
using DonkeyWork.Workflows.Core.Actions.Models;

namespace DonkeyWork.Workflows.Core.Actions.Services.ActionOrchestrator;

/// <summary>
/// A service for orchestrating actions.
/// </summary>
public interface IActionOrchestratorService
{
    /// <summary>
    /// Executes an action asynchronously.
    /// </summary>
    /// <param name="actionExecutionRequest">The execution request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task ExecuteActionAsync(
        ActionExecutionRequest actionExecutionRequest,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes an action asynchronously using a stream.
    /// </summary>
    /// <param name="actionExecutionRequest">The execution request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public IAsyncEnumerable<BaseStreamItem> ExecuteActionStreamAsync(
        ActionExecutionRequest actionExecutionRequest,
        CancellationToken cancellationToken = default);
}