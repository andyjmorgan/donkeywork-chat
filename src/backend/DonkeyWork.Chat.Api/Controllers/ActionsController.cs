// ------------------------------------------------------
// <copyright file="ActionsController.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using AutoMapper;
using DonkeyWork.Chat.Api.Models.Action;
using DonkeyWork.Persistence.Agent.Repository.Action;
using DonkeyWork.Persistence.Agent.Repository.Action.Models;
using DonkeyWork.Persistence.Common.Common;
using Microsoft.AspNetCore.Mvc;

namespace DonkeyWork.Chat.Api.Controllers;

/// <summary>
/// Handles actions.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ActionsController(
    IActionRepository actionRepository,
    IMapper mapper)
    : ControllerBase
{
    /// <summary>
    /// Gets a paged list of actions.
    /// </summary>
    /// <param name="pagingParameters">The paging parameters.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="GetActionsModel"/>.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetActionsModel))]
    public async Task<IActionResult> GetActionsAsync([FromQuery] PagingParameters pagingParameters, CancellationToken cancellationToken)
    {
        var result = await actionRepository.GetActionsAsync(pagingParameters, cancellationToken);
        return this.Ok(mapper.Map<GetActionsModel>(result));
    }

    /// <summary>
    /// Gets an action by id.
    /// </summary>
    /// <param name="id">The action id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="GetActionsItemModel"/>.</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetActionsItemModel))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetActionByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var action = await actionRepository.GetActionByIdAsync(id, cancellationToken);
        if (action == null)
        {
            return this.NotFound();
        }

        return this.Ok(mapper.Map<GetActionsItemModel>(action));
    }

    /// <summary>
    /// Gets an action by name.
    /// </summary>
    /// <param name="name">The action name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="GetActionsItemModel"/>.</returns>
    [HttpGet("name/{name}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetActionsItemModel))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetActionByNameAsync(string name, CancellationToken cancellationToken)
    {
        var action = await actionRepository.GetActionByNameAsync(name, cancellationToken);
        if (action == null)
        {
            return this.NotFound();
        }

        return this.Ok(mapper.Map<GetActionsItemModel>(action));
    }

    /// <summary>
    /// Deletes an action.
    /// </summary>
    /// <param name="id">The action id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteActionAsync(Guid id, CancellationToken cancellationToken)
    {
        return await actionRepository.DeleteActionAsync(id, cancellationToken)
            ? this.NoContent()
            : this.NotFound();
    }

    /// <summary>
    /// Updates an action.
    /// </summary>
    /// <param name="id">The action id.</param>
    /// <param name="action">The action.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [HttpPatch("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateActionAsync(Guid id, [FromBody] UpsertActionModel action, CancellationToken cancellationToken)
    {
        var actionItem = mapper.Map<UpsertActionItem>(action);

        return await actionRepository.UpdateActionAsync(id, actionItem, cancellationToken)
            ? this.NoContent()
            : this.NotFound();
    }

    /// <summary>
    /// Adds an action.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddActionAsync([FromBody] UpsertActionModel action, CancellationToken cancellationToken)
    {
        var actionItem = mapper.Map<UpsertActionItem>(action);

        return await actionRepository.AddActionAsync(actionItem, cancellationToken)
            ? this.NoContent()
            : this.BadRequest();
    }
}