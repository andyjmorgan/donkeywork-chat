// ------------------------------------------------------
// <copyright file="ActionExecutionController.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Services.UserContext;
using DonkeyWork.Persistence.Agent.Repository.Action;
using DonkeyWork.Persistence.Agent.Repository.ActionExecution;
using DonkeyWork.Persistence.Common.Common;
using DonkeyWork.Workflows.Core.Actions.Models;
using DonkeyWork.Workflows.Core.Actions.Services.ActionConsumer;
using Microsoft.AspNetCore.Mvc;

namespace DonkeyWork.Chat.Api.Controllers;

/// <summary>
/// A controller for executing actions.
/// </summary>
/// <param name="actionExecutionPublisherService">The action publisher service.</param>
/// <param name="actionRepository">The action repository.</param>
/// <param name="actionExecutionRepository">The action execution repository.</param>
/// <param name="userContextProvider">The user context provider.</param>
[ApiController]
[Route("api/[controller]")]
public class ActionExecutionController(
    IActionExecutionPublisherService actionExecutionPublisherService,
    IActionRepository actionRepository,
    IActionExecutionRepository actionExecutionRepository,
    IUserContextProvider userContextProvider)
    : ControllerBase
{
    /// <summary>
    /// Gets a paged list of action executions.
    /// </summary>
    /// <param name="pagingParameters">The paging parameters.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A list of action executions.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActionExecutionsAsync([FromQuery] PagingParameters pagingParameters, CancellationToken cancellationToken)
    {
        var result = await actionExecutionRepository.GetActionExecutionsAsync(pagingParameters, cancellationToken);
        return this.Ok(result);
    }

    /// <summary>
    /// Gets a paged list of action executions for a specific action.
    /// </summary>
    /// <param name="actionId">The action ID to filter by.</param>
    /// <param name="pagingParameters">The paging parameters.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A list of action executions for the specified action.</returns>
    [HttpGet("action/{actionId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetActionExecutionsByActionIdAsync(Guid actionId, [FromQuery] PagingParameters pagingParameters, CancellationToken cancellationToken)
    {
        // Verify the action exists
        var action = await actionRepository.GetActionByIdAsync(actionId, cancellationToken);
        if (action == null)
        {
            return this.NotFound();
        }

        var result = await actionExecutionRepository.GetActionExecutionsByActionIdAsync(actionId, pagingParameters, cancellationToken);
        return this.Ok(result);
    }

    /// <summary>
    /// Adds an action.
    /// </summary>
    /// <param name="request">The action.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ExecuteAction([FromBody] ActionExecutionRequest request, CancellationToken cancellationToken)
    {
        var action = await actionRepository.GetActionByIdAsync(request.Id, cancellationToken);
        if (action == null)
        {
            return this.NotFound();
        }

        await actionExecutionPublisherService.PublishAsync(
            request with
        {
#pragma warning disable SA1101
            UserId = userContextProvider.UserId,
            ActionName = action.Name,
#pragma warning restore SA1101
        }, cancellationToken);
        return this.Accepted(new
        {
            request.ExecutionId,
        });
    }
}
