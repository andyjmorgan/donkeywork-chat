// ------------------------------------------------------
// <copyright file="IActionExecutionPublisherService.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Workflows.Core.Actions.Models;

namespace DonkeyWork.Workflows.Core.Actions.Services.ActionConsumer;

/// <summary>
/// An interface for a service that publishes action execution requests to a central queue.
/// </summary>
public interface IActionExecutionPublisherService
{
    /// <summary>
    /// Publishes an action execution request to the central queue for processing.
    /// </summary>
    /// <param name="request">The action execution request to publish.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is null.</exception>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task PublishAsync(ActionExecutionRequest request, CancellationToken cancellationToken = default);
}