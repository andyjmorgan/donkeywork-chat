// ------------------------------------------------------
// <copyright file="ActionExecutionQueueService.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Collections.Concurrent;
using DonkeyWork.Workflows.Core.Actions.Models;

namespace DonkeyWork.Workflows.Core.Actions.Services.ActionConsumer;

/// <summary>
/// Provides a singleton queue service for workflow action execution requests.
/// This service manages the central queue and allows consumers to be created per scope.
/// </summary>
public sealed class ActionExecutionQueueService : IActionExecutionQueueService
{
    /// <summary>
    /// Thread-safe collection for storing action execution requests.
    /// </summary>
    private readonly BlockingCollection<ActionExecutionRequest> queue = new ();

    /// <summary>
    /// Flag indicating whether this instance has been disposed.
    /// </summary>
    private bool disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="ActionExecutionQueueService"/> class.
    /// </summary>
    public ActionExecutionQueueService()
    {
    }

    /// <inheritdoc />
    public void Publish(ActionExecutionRequest request)
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException(nameof(ActionExecutionQueueService));
        }

        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        if (!this.queue.IsAddingCompleted)
        {
            this.queue.Add(request);
        }
    }

    /// <inheritdoc />
    public bool TryTake(out ActionExecutionRequest? request, int timeout, CancellationToken cancellationToken)
    {
        return this.queue.TryTake(out request, timeout, cancellationToken);
    }

    /// <summary>
    /// Releases all resources used by the <see cref="ActionExecutionQueueService"/> instance.
    /// </summary>
    public void Dispose()
    {
        if (this.disposed)
        {
            return;
        }

        this.queue.CompleteAdding();
        this.queue.Dispose();

        this.disposed = true;
    }
}