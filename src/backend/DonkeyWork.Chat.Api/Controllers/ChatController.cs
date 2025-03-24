// ------------------------------------------------------
// <copyright file="ChatController.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using DonkeyWork.Chat.AiServices.Clients.Models;
using DonkeyWork.Chat.AiServices.Services;
using DonkeyWork.Chat.Api.Models.Chat;
using DonkeyWork.Chat.Api.Services.Conversation;
using DonkeyWork.Chat.Persistence.Repository.Prompt;
using Microsoft.AspNetCore.Mvc;

namespace DonkeyWork.Chat.Api.Controllers;

/// <summary>
/// A chat controller.
/// </summary>
/// <param name="conversationService">The chat service.</param>
[ApiController]
[Route("api/[controller]")]
public class ChatController(IConversationService conversationService, IPromptRepository promptRepository)
    : ControllerBase
{
    /// <summary>
    /// Post a chat request.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [HttpPost]
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1101:PrefixLocalCallsWithThis", Justification = "This is a problem with record structures.")]
    public async Task PostChatAsync(
        [FromBody] ChatRequestModel request,
        CancellationToken cancellationToken = default)
    {
        this.HttpContext.Response.Headers.Append("Content-Type", "text/event-stream");
        if (request.PromptId.HasValue)
        {
            var promptContent = await promptRepository.GetPromptContentAsync(request.PromptId.Value, cancellationToken);
            if (promptContent is not null)
            {
                request.Messages.Insert(0, new GenericChatMessage()
                {
                    Content = promptContent.Content,
                    Role = GenericMessageRole.System,
                });
            }
        }

        await foreach (var streamItem in conversationService.GetResponseAsync(
                           new ChatServiceRequest() // Todo: automap the above.
                           {
                               Model = request.Model,
                               Provider = request.Provider,
                               Messages = request.Messages,
                               Parameters = request.Parameters,
                               ConversationId = request.ConversationId,
                           },
                           this.HttpContext.RequestAborted))
        {
            await this.HttpContext.Response.WriteAsync(
                $"event: {streamItem.GetType().Name}{Environment.NewLine}",
                this.HttpContext.RequestAborted);

            await this.HttpContext.Response.WriteAsync(
                $"data: {JsonSerializer.Serialize(streamItem)}",
                this.HttpContext.RequestAborted);

            await this.HttpContext.Response.WriteAsync(
                $"{Environment.NewLine}{Environment.NewLine}",
                this.HttpContext.RequestAborted);

            await this.HttpContext.Response.Body.FlushAsync(this.HttpContext.RequestAborted);
        }

        await this.HttpContext.Response.Body.FlushAsync(this.HttpContext.RequestAborted);
        await this.HttpContext.Response.CompleteAsync();
    }
}