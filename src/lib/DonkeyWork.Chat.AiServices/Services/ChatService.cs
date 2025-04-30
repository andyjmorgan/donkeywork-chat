// ------------------------------------------------------
// <copyright file="ChatService.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.Json;
using DonkeyWork.Chat.AiServices.Clients;
using DonkeyWork.Chat.AiServices.Clients.Models;
using DonkeyWork.Chat.AiTooling.Exceptions;
using DonkeyWork.Chat.AiTooling.Services;
using DonkeyWork.Chat.Common.Contracts;
using DonkeyWork.Chat.Common.Models.Streaming;
using DonkeyWork.Chat.Common.Models.Streaming.Request;

namespace DonkeyWork.Chat.AiServices.Services;

/// <inheritdoc />
public class ChatService
    : IChatService
{
    private readonly IAIChatProviderFactory chatProviderFactory;
    private readonly IToolService toolService;
    private readonly IUserPostureService userPostureService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatService"/> class.
    /// </summary>
    /// <param name="chatClient">The chat client.</param>
    /// <param name="toolService">The tool service.</param>
    /// <param name="userPostureService">The user posture service.</param>
    public ChatService(IAIChatProviderFactory chatClient, IToolService toolService, IUserPostureService userPostureService)
    {
        this.chatProviderFactory = chatClient;
        this.toolService = toolService;
        this.userPostureService = userPostureService;
    }

    /// <inheritdoc />
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1101:PrefixLocalCallsWithThis", Justification = "This is a problem with record structures.")]
    public async IAsyncEnumerable<BaseStreamItem> GetResponseAsync(
        ChatServiceRequest chatServiceRequest,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var chatClient = this.chatProviderFactory.CreateChatClient(chatServiceRequest.Provider);
        Stopwatch requestStopwatch = Stopwatch.StartNew();
        yield return new RequestStart()
        {
            ExecutionId = chatServiceRequest.ExecutionId,
        };
        var toolDefinitions = toolService.GetUserScopedTools(await userPostureService.GetUserPosturesAsync(cancellationToken));
        var request = new ChatRequest()
        {
            ModelName = chatServiceRequest.Model,
            Id = chatServiceRequest.ExecutionId,
            Messages = chatServiceRequest.Messages,
        };

        if (!chatServiceRequest.ConversationId.HasValue)
        {
            chatServiceRequest = chatServiceRequest with { ConversationId = chatServiceRequest.ExecutionId };
        }

        await foreach (var streamItem in chatClient.StreamChatAsync(
                           request,
                           toolDefinitions,
                           toolAction: async x =>
                           {
                               var tool = toolDefinitions
                                   .FirstOrDefault(t => t.Name == x.ToolName);

                               if (tool is null)
                               {
                                   throw new UnknownToolDefinitionException(x.ToolName);
                               }

                               var result = await tool.Tool.InvokeFunctionAsync(
                                   x.ToolName,
                                   x.ToolParameters,
                                   cancellationToken);
                               if (result is JsonDocument jsonDocument)
                               {
                                   return jsonDocument;
                               }

                               return JsonDocument.Parse(
                                   JsonSerializer.Serialize(new
                                   {
                                       Result = result,
                                   }));
                           },
                           cancellationToken))
        {
            yield return streamItem;
        }

        requestStopwatch.Stop();
        yield return new RequestEnd()
        {
            ExecutionId = chatServiceRequest.ExecutionId,
            Duration = requestStopwatch.Elapsed,
            ConversationId = chatServiceRequest.ConversationId.Value,
        };
    }
}