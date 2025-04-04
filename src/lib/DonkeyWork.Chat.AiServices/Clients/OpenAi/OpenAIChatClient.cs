// ------------------------------------------------------
// <copyright file="OpenAIChatClient.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.ClientModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json;
using DonkeyWork.Chat.AiServices.Clients.Models;
using DonkeyWork.Chat.AiServices.Clients.OpenAi.Configuration;
using DonkeyWork.Chat.AiServices.Streaming;
using DonkeyWork.Chat.AiServices.Streaming.Chat;
using DonkeyWork.Chat.AiServices.Streaming.Tool;
using DonkeyWork.Chat.AiTooling.Base.Models;
using DonkeyWork.Chat.AiTooling.Exceptions;
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Chat;

namespace DonkeyWork.Chat.AiServices.Clients.OpenAi;

/// <summary>
/// An open Ai chat client.
/// </summary>
/// <param name="configuration">The configuration.</param>
public class OpenAIChatClient(IOptions<OpenAiConfiguration> configuration)
    : IAIChatClient
{
    private readonly OpenAIClient openAiClient = new (
        new ApiKeyCredential(configuration.Value.ApiKey ?? string.Empty),
        new OpenAIClientOptions());

    /// <inheritdoc />
    public async IAsyncEnumerable<BaseStreamItem> StreamChatAsync(
        ChatRequest request,
        List<ToolDefinition> toolDefinitions,
        Func<ToolCallback, Task<string>> toolAction,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var chatFragment in this.StreamChatAsync(
                           ConvertMessages(request),
                           CreateChatCompletionOptions(request, toolDefinitions),
                           this.openAiClient.GetChatClient(request.ModelName),
                           toolAction: async x => await toolAction(x),
                           cancellationToken))
        {
#pragma warning disable SA1101
            yield return chatFragment with { ExecutionId = request.Id };
#pragma warning restore SA1101
        }
    }

    private static ChatCompletionOptions CreateChatCompletionOptions(ChatRequest request, List<ToolDefinition> toolDefinitions)
    {
        var options = new ChatCompletionOptions()
        {
            Temperature = request.Temperature,
            AllowParallelToolCalls = toolDefinitions.Any() ? true : null,

            // ReSharper disable once RedundantEmptyObjectOrCollectionInitializer, we need to initialize the list.
            Tools = { },
        };

        IList<ChatTool> tools = toolDefinitions.Select(
                x =>
                    x.CreateOpenAiFunctionTool())
            .ToList();
        foreach (var tool in tools)
        {
            options.Tools.Add(tool);
        }

        return options;
    }

    private static List<ChatMessage> ConvertMessages(ChatRequest request)
    {
        List<ChatMessage> messages = new List<ChatMessage>();
        foreach (var i in request.GetValidMessages())
        {
            switch (i.Role)
            {
                case GenericMessageRole.User:
                    messages.Add(new UserChatMessage(i.Content));
                    break;
                case GenericMessageRole.System:
                    messages.Add(new SystemChatMessage(i.Content));
                    break;
                case GenericMessageRole.Assistant:
                    messages.Add(new AssistantChatMessage(i.Content));
                    break;
            }
        }

        return messages;
    }

    private static BaseStreamItem CreateUsage(ChatTokenUsage tokenUsage, Guid chatId)
    {
        return new TokenUsage()
        {
            InputTokens = tokenUsage.InputTokenCount,
            OutputTokens = tokenUsage.OutputTokenCount,
            ChatId = chatId,
        };
    }

    private static ChatToolCall CreateToolCall(IGrouping<int, StreamingChatToolCallUpdate> toolCall)
    {
        var toolCallParameters = toolCall.Select(x => x.FunctionArgumentsUpdate)
            .Where(x => x.ToArray().Length > 0)
            .ToList();

        var toolCallFunction = toolCall.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.FunctionName));
        var toolCallId = toolCall.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.ToolCallId));

        var toolBytes = toolCallParameters.SelectMany(x => x.ToArray()).ToArray();

        if (toolCallFunction is null || toolCallId is null)
        {
            throw new ToolArgumentMissingException("Tool call is missing required data");
        }

        var functionToolCall = ChatToolCall.CreateFunctionToolCall(
            toolCallId.ToolCallId,
            toolCallFunction.FunctionName,
            new BinaryData(toolBytes));

        return functionToolCall;
    }

    private async IAsyncEnumerable<BaseStreamItem> StreamChatAsync(
        List<ChatMessage> messages,
        ChatCompletionOptions options,
        ChatClient chatClient,
        Func<ToolCallback, Task<string>> toolAction,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        bool awaitToolCalls = false;
        Guid chatId = Guid.NewGuid();

        // We can have situations where it'll start talking then make a tool call.
        List<StreamingChatCompletionUpdate> updates = new List<StreamingChatCompletionUpdate>();
        foreach (StreamingChatCompletionUpdate i in chatClient.CompleteChatStreaming(
                     messages,
                     options,
                     cancellationToken))
        {
            // todo: capture model name and message provider id.
            if (updates.Count == 0)
            {
                yield return new ChatStartFragment()
                {
                    ChatId = chatId,
                    ModelName = i.Model,
                    MessageProviderId = i.CompletionId,
                };
            }

            updates.Add(i);
            if (i.Usage is not null)
            {
                yield return CreateUsage(i.Usage, chatId);
            }

            if (!awaitToolCalls && i.ToolCallUpdates.Any())
            {
                awaitToolCalls = true;
            }

            if (awaitToolCalls)
            {
                continue;
            }

            if (string.IsNullOrEmpty(i.ContentUpdate.FirstOrDefault()?.Text ?? string.Empty))
            {
                continue;
            }

            // We can have situations where it'll start talking then make a tool call.
            yield return new ChatFragment()
            {
                Content = i.ContentUpdate.First().Text,
                ChatId = chatId,
            };
        }

        yield return new ChatEndFragment()
        {
            ChatId = chatId,
        };

        if (awaitToolCalls)
        {
            var toolCalls = updates.Where(x => x.ToolCallUpdates.Any())
                .SelectMany(x => x.ToolCallUpdates)
                .GroupBy(x => x.Index);

            // If the LLM has answered partially before making tool calls, we need to gather them up.
            var chatFragments = updates
                .SelectMany(x => x.ContentUpdate)

                // ReSharper disable once RedundantEnumerableCastCall
                .OfType<ChatMessageContentPart>()
                .ToList();
            if (chatFragments.Any())
            {
                messages.Add(new AssistantChatMessage(chatFragments));
            }

            foreach (var toolCall in toolCalls)
            {
                var toolCallId = Guid.NewGuid();
                var functionToolCall = CreateToolCall(toolCall);

                messages.Add(new AssistantChatMessage([functionToolCall]));
                var argument = JsonDocument.Parse(
                    string.Join(
                        string.Empty,
                        functionToolCall.FunctionArguments));

                yield return new ToolCall()
                {
                    Name = functionToolCall.FunctionName,
                    QueryParameters = argument,
                    Index = toolCall.Key,
                    ToolCallId = toolCallId,
                    ChatId = chatId,
                };

                Stopwatch toolStopwatch = Stopwatch.StartNew();
                var toolResult = await toolAction(
                    new ToolCallback()
                    {
                        ToolName = functionToolCall.FunctionName,
                        ToolParameters = argument,
                    });
                toolStopwatch.Stop();

                yield return new ToolResult()
                {
                    Result = toolResult,
                    Duration = toolStopwatch.Elapsed,
                    ToolCallId = toolCallId,
                };

                messages.Add(
                    new ToolChatMessage(
                        functionToolCall.Id,
                        toolResult));
            }

            await foreach (var fragment in
                           this.StreamChatAsync(
                               messages,
                               options,
                               chatClient,
                               toolAction,
                               cancellationToken))
            {
                yield return fragment;
            }
        }
    }
}