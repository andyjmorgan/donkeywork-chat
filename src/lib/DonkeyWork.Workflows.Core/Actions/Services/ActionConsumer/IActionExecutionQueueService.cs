// ------------------------------------------------------
// <copyright file="IActionExecutionQueueService.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Workflows.Core.Actions.Models;

namespace DonkeyWork.Workflows.Core.Actions.Services.ActionConsumer;

/// <summary>
/// An interface for a service that manages a queue of action execution requests.
/// </summary>
public interface IActionExecutionQueueService : IDisposable
{
    /// <summary>
    /// Adds an action execution request to the processing queue.
    /// </summary>
    /// <param name="request">The action execution request to be processed.</param>
    /// <exception cref="ObjectDisposedException">Thrown when trying to publish to a disposed service.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is null.</exception>
    public void Publish(ActionExecutionRequest request);

    /// <summary>
    /// Attempts to take an item from the queue.
    /// </summary>
    /// <param name="request">When this method returns, contains the execution request, if available; otherwise the default value.</param>
    /// <param name="timeout">The time to wait for an available item, or -1 for infinite wait.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>true if an item was available; otherwise, false.</returns>
    public bool TryTake(out ActionExecutionRequest? request, int timeout, CancellationToken cancellationToken);
}