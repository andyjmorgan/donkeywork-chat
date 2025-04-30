// ------------------------------------------------------
// <copyright file="ConversationsController.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using AutoMapper;
using DonkeyWork.Chat.Api.Models.Conversation;
using DonkeyWork.Persistence.Chat.Repository.Conversation;
using DonkeyWork.Persistence.Common.Common;
using Microsoft.AspNetCore.Mvc;

namespace DonkeyWork.Chat.Api.Controllers;

/// <summary>
/// Handles conversations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ConversationsController(
    IConversationRepository conversationRepository,
    IMapper mapper)
    : ControllerBase
{
    /// <summary>
    /// Gets a paged list of conversations.
    /// </summary>
    /// <param name="pagingParameters">The paging parameters.</param>
    /// <returns>A <see cref="GetConversationsModel"/>.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetConversationsModel))]
    public async Task<IActionResult> GetConversationsAsync([FromQuery] PagingParameters pagingParameters)
    {
        return this.Ok(mapper.Map<GetConversationsModel>(await conversationRepository.GetConversationsAsync(pagingParameters, this.HttpContext.RequestAborted)));
    }

    /// <summary>
    /// Gets a paged list of conversations.
    /// </summary>
    /// <param name="conversationId">The conversation id.</param>
    /// <returns>A <see cref="GetConversationsModel"/>.</returns>
    [HttpGet("{conversationId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetConversationsModel))]
    public async Task<IActionResult> GetConversationAsync([FromRoute] Guid conversationId)
    {
        return this.Ok(mapper.Map<GetConversationModel>(await conversationRepository.GetConversationAsync(conversationId, this.HttpContext.RequestAborted)));
    }

    /// <summary>
    /// Deletes a conversation.
    /// </summary>
    /// <param name="conversationId">The conversation id.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [HttpDelete("{conversationId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteConversationAsync(Guid conversationId)
    {
        return await conversationRepository.DeleteConversationAsync(conversationId, this.HttpContext.RequestAborted) ? this.NoContent() : this.NotFound();
    }

    /// <summary>
    /// Updates a conversation title.
    /// </summary>
    /// <param name="conversationId">The conversation Id.</param>
    /// <param name="title">The title.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [HttpPatch("{conversationId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateConversationTitleAsync(Guid conversationId, [FromBody] string title)
    {
        return await conversationRepository.UpdateConversationTitleAsync(conversationId, title, this.HttpContext.RequestAborted) ? this.NoContent() : this.NotFound();
    }
}