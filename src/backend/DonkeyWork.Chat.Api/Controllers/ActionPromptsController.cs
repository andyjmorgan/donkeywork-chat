// ------------------------------------------------------
// <copyright file="ActionPromptsController.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using AutoMapper;
using DonkeyWork.Chat.Api.Models.Prompt;
using DonkeyWork.Chat.Common.Models.Prompt;
using DonkeyWork.Chat.Common.Models.Prompt.Content;
using DonkeyWork.Persistence.Agent.Repository.Prompt;
using DonkeyWork.Persistence.Agent.Repository.Prompt.Models.ActionPrompt;
using DonkeyWork.Persistence.Common.Common;
using Microsoft.AspNetCore.Mvc;

namespace DonkeyWork.Chat.Api.Controllers;

/// <summary>
/// Handles action prompts.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ActionPromptsController(
    IPromptRepository promptRepository,
    IMapper mapper)
    : ControllerBase
{
    /// <summary>
    /// Gets a paged list of action prompts.
    /// </summary>
    /// <param name="pagingParameters">The paging parameters.</param>
    /// <returns>A <see cref="GetActionPromptsModel"/>.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetActionPromptsModel))]
    public async Task<IActionResult> GetActionPromptsAsync([FromQuery] PagingParameters pagingParameters)
    {
        return this.Ok(mapper.Map<GetActionPromptsModel>(await promptRepository.GetActionPromptsAsync(pagingParameters)));
    }

    /// <summary>
    /// Deletes an action prompt.
    /// </summary>
    /// <param name="id">The action prompt id.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteActionPromptAsync(Guid id)
    {
        return await promptRepository.DeleteActionPromptAsync(id) ? this.NoContent() : this.NotFound();
    }

    /// <summary>
    /// Updates an action prompt.
    /// </summary>
    /// <param name="id">The action prompt id.</param>
    /// <param name="prompt">The action prompt.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [HttpPatch("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateActionPromptAsync(Guid id, [FromBody] UpsertActionPromptModel prompt)
    {
        return await promptRepository.UpdateActionPromptAsync(id, new UpsertActionPromptItem()
        {
            Name = prompt.Name,
            Variables = prompt.Variables,
            Description = prompt.Description,
            Messages = prompt.Messages.Select(x => new PromptMessage()
            {
                Content = new TextContent()
                {
                    Text = x.Content,
                },
                Role = x.Role,
            }).ToList(),
        }) ? this.NoContent() : this.NotFound();
    }

    /// <summary>
    /// Adds an action prompt.
    /// </summary>
    /// <param name="prompt">The action prompt.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddActionPromptAsync([FromBody] UpsertActionPromptModel prompt)
    {
        var result = await promptRepository.AddActionPromptAsync(
            new UpsertActionPromptItem()
            {
                Name = prompt.Name,
                Variables = prompt.Variables,
                Description = prompt.Description,
                Messages = prompt.Messages.Select(x => new PromptMessage()
                {
                    Content = new TextContent()
                    {
                        Text = x.Content,
                    },
                    Role = x.Role,
                }).ToList(),
            });

        return result ? this.NoContent() : this.NotFound();
    }
}