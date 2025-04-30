// ------------------------------------------------------
// <copyright file="PromptsController.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using AutoMapper;
using DonkeyWork.Chat.Api.Models.Conversation;
using DonkeyWork.Chat.Api.Models.Prompt;
using DonkeyWork.Persistence.Agent.Repository.Prompt;
using DonkeyWork.Persistence.Agent.Repository.Prompt.Models.SystemPrompt;
using DonkeyWork.Persistence.Common.Common;
using Microsoft.AspNetCore.Mvc;

namespace DonkeyWork.Chat.Api.Controllers;

/// <summary>
/// Handles prompts.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PromptsController(
    IPromptRepository promptRepository,
    IMapper mapper)
    : ControllerBase
{
    /// <summary>
    /// Gets a paged list of prompts.
    /// </summary>
    /// <param name="pagingParameters">The paging parameters.</param>
    /// <returns>A <see cref="GetConversationsModel"/>.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetPromptsModel))]
    public async Task<IActionResult> GetPromptsAsync([FromQuery] PagingParameters pagingParameters)
    {
        return this.Ok(mapper.Map<GetPromptsModel>(await promptRepository.GetPromptsAsync(pagingParameters)));
    }

    /// <summary>
    /// Deletes a prompt.
    /// </summary>
    /// <param name="id">The prompt id.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePromptAsync(Guid id)
    {
        return await promptRepository.DeletePromptAsync(id) ? this.NoContent() : this.NotFound();
    }

    /// <summary>
    /// Updates a prompt.
    /// </summary>
    /// <param name="id">The prompt id.</param>
    /// <param name="prompt">The prompt.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [HttpPatch("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePromptAsync(Guid id, [FromBody] UpsertPromptModel prompt)
    {
        return await promptRepository.UpdatePromptAsync(id, mapper.Map<UpsertPromptItem>(prompt)) ? this.NoContent() : this.NotFound();
    }

    /// <summary>
    /// Adds a prompt.
    /// </summary>
    /// <param name="prompt">The prompt.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddPromptAsync([FromBody] UpsertPromptModel prompt)
    {
        return await promptRepository.AddPromptAsync(mapper.Map<UpsertPromptItem>(prompt)) ? this.NoContent() : this.NotFound();
    }
}