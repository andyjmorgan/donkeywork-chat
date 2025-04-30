// ------------------------------------------------------
// <copyright file="IActionExecutionService.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Models.Streaming;
using DonkeyWork.Persistence.Agent.Repository.Action.Models.Execution;

namespace DonkeyWork.Workflows.Core.Actions.Services.ActionExecution;

/// <summary>
/// An action execution service.
/// </summary>
public interface IActionExecutionService
{
    /// <summary>
    /// Executes an action asynchronously.
    /// </summary>
    /// <param name="actionItem">The action item.</param>
    /// <param name="userInput">The user input.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public IAsyncEnumerable<BaseStreamItem> ExecuteActionAsync(
        ActionExecutionItem actionItem,
        string userInput,
        CancellationToken cancellationToken = default);
}
