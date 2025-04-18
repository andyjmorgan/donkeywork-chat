// ------------------------------------------------------
// <copyright file="ConversationService.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using DonkeyWork.Chat.AiServices.Clients.Models;
using DonkeyWork.Chat.AiServices.Services;
using DonkeyWork.Chat.AiServices.Streaming;
using DonkeyWork.Chat.AiServices.Streaming.Chat;
using DonkeyWork.Chat.AiServices.Streaming.Request;
using DonkeyWork.Chat.AiServices.Streaming.Tool;
using DonkeyWork.Chat.Persistence.Entity.Conversation;
using DonkeyWork.Chat.Persistence.Repository.Conversation;

namespace DonkeyWork.Chat.Api.Services.Conversation;

/// <summary>
/// A conversation service.
/// </summary>
/// <param name="chatService">The chat service.</param>
public class ConversationService(IChatService chatService, IConversationRepository conversationRepository)
    : IConversationService
{
    /// <inheritdoc />
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1101:PrefixLocalCallsWithThis", Justification = "Records are not This.")]
    public async IAsyncEnumerable<BaseStreamItem> GetResponseAsync(
        ChatServiceRequest chatServiceRequest,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        Dictionary<Guid, ToolCall> toolCalls = [];

        // A switch for detecting a new conversation.
        if (!chatServiceRequest.ConversationId.HasValue)
        {
            chatServiceRequest = chatServiceRequest with { ConversationId = Guid.NewGuid() };
            await conversationRepository.AddConversationAsync(chatServiceRequest.ConversationId.Value, chatServiceRequest.Messages.First(x => x.Role == GenericMessageRole.User).Content, cancellationToken);
        }

        await conversationRepository.AddMessageToConversationAsync(
            chatServiceRequest.ConversationId.Value,
            chatServiceRequest.ExecutionId,
            MessageOwner.User,
            chatServiceRequest.Messages.Last(x => x.Role == GenericMessageRole.User).Content,
            cancellationToken);

        StringBuilder stringBuilder = new ();

        await foreach (var message in chatService.GetResponseAsync(chatServiceRequest, cancellationToken))
        {
            if (message is ChatFragment chatFragment)
            {
                stringBuilder.Append(chatFragment.Content);
            }

            if (message is ToolCall toolCall)
            {
                toolCalls.Add(toolCall.ToolCallId, toolCall);
            }

            if (message is ToolResult toolResult)
            {
                var toolRequestCall = toolCalls.GetValueOrDefault(toolResult.ToolCallId);
                {
                    if (toolRequestCall is not null)
                    {
                        await conversationRepository.AddToolCallToConversationAsync(
                            chatServiceRequest.ConversationId.Value,
                            chatServiceRequest.ExecutionId,
                            toolRequestCall.QueryParameters.RootElement.GetRawText(),
                            toolResult.Result,
                            toolRequestCall.Name,
                            cancellationToken);
                    }
                }
            }

            if (message is RequestEnd requestEnd)
            {
                requestEnd = requestEnd with { ConversationId = chatServiceRequest.ConversationId.Value };
                yield return requestEnd;
            }

            yield return message;
        }

        await conversationRepository.AddMessageToConversationAsync(
            chatServiceRequest.ConversationId.Value,
            chatServiceRequest.ExecutionId,
            MessageOwner.Assistant,
            stringBuilder.ToString(),
            cancellationToken);
    }
}