// ------------------------------------------------------
// <copyright file="ActionExecutionPublisherService.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Workflows.Core.Actions.Models;

namespace DonkeyWork.Workflows.Core.Actions.Services.ActionConsumer;

/// <summary>
/// Scoped service for publishing action execution requests to the central queue.
/// </summary>
public sealed class ActionExecutionPublisherService : IActionExecutionPublisherService
{
    /// <summary>
    /// The host service that manages worker tasks and the central queue.
    /// </summary>
    private readonly IActionExecutionHostService hostService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ActionExecutionPublisherService"/> class.
    /// </summary>
    /// <param name="hostService">The host service that manages worker tasks and the central queue.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="hostService"/> is null.</exception>
    public ActionExecutionPublisherService(IActionExecutionHostService hostService)
    {
        this.hostService = hostService ??
                            throw new ArgumentNullException(nameof(hostService));
    }

    /// <inheritdoc />
    public Task PublishAsync(ActionExecutionRequest request, CancellationToken cancellationToken = default)
    {
        this.hostService.Publish(request);
        return Task.CompletedTask;
    }
}
