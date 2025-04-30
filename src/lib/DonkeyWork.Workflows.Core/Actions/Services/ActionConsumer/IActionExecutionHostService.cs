// ------------------------------------------------------
// <copyright file="IActionExecutionHostService.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Workflows.Core.Actions.Models;

namespace DonkeyWork.Workflows.Core.Actions.Services.ActionConsumer;

/// <summary>
/// An interface for a service that manages the execution of action requests.
/// </summary>
public interface IActionExecutionHostService : IDisposable
{
    /// <summary>
    /// Adds an action execution request to the central processing queue.
    /// </summary>
    /// <param name="request">The action execution request to be processed.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is null.</exception>
    void Publish(ActionExecutionRequest request);
}