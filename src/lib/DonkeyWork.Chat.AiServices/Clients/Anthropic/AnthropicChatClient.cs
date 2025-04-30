// ------------------------------------------------------
// <copyright file="AnthropicChatClient.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Anthropic.SDK;
using Anthropic.SDK.Common;
using Anthropic.SDK.Messaging;
using DonkeyWork.Chat.AiServices.Clients.Anthropic.Configuration;
using DonkeyWork.Chat.AiServices.Clients.Models;
using DonkeyWork.Chat.AiTooling.Base.Models;
using DonkeyWork.Chat.Common.Models.Streaming;
using DonkeyWork.Chat.Common.Models.Streaming.Chat;
using DonkeyWork.Chat.Common.Models.Streaming.Tool;
using Microsoft.Extensions.Options;
using AnthropicSDK = Anthropic.SDK;

namespace DonkeyWork.Chat.AiServices.Clients.Anthropic;

/// <inheritdoc />
public class AnthropicChatClient : IAIChatClient
{
    private static readonly JsonSerializerOptions ToolsSerializationOptions = new JsonSerializerOptions()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    private readonly AnthropicClient anthropicClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="AnthropicChatClient"/> class.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="httpClientFactory">The http client factory.</param>
    public AnthropicChatClient(IOptions<AnthropicConfiguration> configuration, IHttpClientFactory httpClientFactory)
    {
        var apiAuth = new APIAuthentication(configuration.Value.ApiKey);
        this.anthropicClient =
            new AnthropicClient(apiAuth, httpClientFactory.CreateClient(nameof(AnthropicChatClient)));
    }

    /// <inheritdoc />
    [SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1101:Prefix local calls with this",
        Justification = "This is a record parameter.")]
    public async IAsyncEnumerable<BaseStreamItem> StreamChatAsync(
        ChatRequest request,
        List<ToolDefinition> toolDefinitions,
        Func<ToolCallback, Task<JsonDocument>> toolAction,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        Guid chatId = Guid.NewGuid();
        var messageParameters = CreateMessageParameters(request, toolDefinitions);

        await foreach (var message in this.StreamChatAsync(
                           request,
                           toolAction,
                           messageParameters,
                           chatId,
                           cancellationToken))
        {
            yield return message with { ExecutionId = request.Id };
        }
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<BaseStreamItem> ChatAsync(
        ChatRequest request,
        List<ToolDefinition> toolDefinitions,
        Func<ToolCallback, Task<JsonDocument>> toolAction,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        Guid chatId = Guid.NewGuid();
        var messageParameters = CreateMessageParameters(request, toolDefinitions);

        await foreach (var message in this.ChatAsync(
                           request,
                           toolAction,
                           messageParameters,
                           chatId,
                           cancellationToken))
        {
#pragma warning disable SA1101
            yield return message with { ExecutionId = request.Id };
#pragma warning restore SA1101
        }
    }

    private static BaseStreamItem CreateUsageMessage(ChatRequest request, Guid chatId, Usage usage)
    {
        return new TokenUsage
        {
            ExecutionId = request.Id,
            ChatId = chatId,
            InputTokens = usage.InputTokens,
            OutputTokens = usage.OutputTokens,
        };
    }

    private static MessageParameters CreateMessageParameters(ChatRequest request, List<ToolDefinition> toolDefinitions)
    {
        MessageParameters messageParameters = new MessageParameters()
        {
            Model = request.ModelName,
            MaxTokens = 8192,
            Messages = request.GetNonSystemMessages()
                .Select(x => new Message
                {
                    Role = x.Role == GenericMessageRole.Assistant ? RoleType.Assistant : RoleType.User,
                    Content = new List<ContentBase>()
                    {
                        new TextContent()
                        {
                            Text = x.Content,
                        },
                    },
                }).ToList(),
            System = request.GetSystemMessages()
                .Select(x => new SystemMessage(x.Content)).ToList(),
            Tools = toolDefinitions.Select(
                    x =>
                        new AnthropicSDK.Common.Tool(
                            new Function(
                                x.Name,
                                x.Description,
                                JsonSerializer.Serialize(x.ToolFunctionDefinition, ToolsSerializationOptions))))
                .ToList(),
        };

        if (request.Metadata.TryGetValue(nameof(KnownMetaDataFields.Temperature), out var temperature))
        {
            messageParameters.Temperature = Convert.ToDecimal(temperature);
        }

        if (request.Metadata.TryGetValue(nameof(KnownMetaDataFields.MaxTokens), out var maxTokens))
        {
            messageParameters.MaxTokens = Convert.ToInt32(maxTokens);
        }

        if (request.Metadata.TryGetValue(nameof(KnownMetaDataFields.TopK), out var topK))
        {
            messageParameters.TopK = Convert.ToInt32(topK);
        }

        if (request.Metadata.TryGetValue(nameof(KnownMetaDataFields.ThinkingEnabled), out var thinkingEnabled))
        {
            messageParameters.Thinking = new ThinkingParameters();
            if (request.Metadata.TryGetValue(nameof(KnownMetaDataFields.BudgetThinkingTokens), out var thinkingBudget))
            {
                messageParameters.Thinking.BudgetTokens = Convert.ToInt32(thinkingBudget);
            }
        }

        return messageParameters;
    }

    private async IAsyncEnumerable<BaseStreamItem> ChatAsync(
        ChatRequest request,
        Func<ToolCallback, Task<JsonDocument>> toolAction,
        MessageParameters messageParameters,
        Guid chatId,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        List<Function> toolCallFunctions = [];
        StringBuilder stringBuilder = new StringBuilder();
        var result = await this.anthropicClient.Messages.GetClaudeMessageAsync(messageParameters, cancellationToken);
        yield return new ChatStartFragment()
        {
            ExecutionId = request.Id,
            ChatId = chatId,
            ModelName = result.Model,
            MessageProviderId = result.Id,
        };
        yield return new ChatEndFragment()
        {
            ChatId = chatId,
        };

        var jsonResult = JsonSerializer.Serialize(messageParameters, new JsonSerializerOptions()
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        });
        yield return CreateUsageMessage(request, chatId, result.Usage);
        foreach (var item in result.Content)
        {
                if (item is TextContent textContent)
                {
                    stringBuilder.Append(textContent.Text);
                    yield return new ChatFragment()
                    {
                        ExecutionId = request.Id,
                        ChatId = chatId,
                        Content = textContent.Text,
                    };
                }
        }

        toolCallFunctions.AddRange(result.ToolCalls);
        if (toolCallFunctions.Any())
        {
            await foreach (var p in this.HandleToolCallsAsync(
                               toolAction,
                               messageParameters,
                               chatId,
                               stringBuilder,
                               toolCallFunctions,
                               cancellationToken))
            {
                yield return p;
            }

            await foreach (var message in this.ChatAsync(
                               request,
                               toolAction,
                               messageParameters,
                               chatId,
                               cancellationToken))
            {
                yield return message;
            }
        }
    }

    private async IAsyncEnumerable<BaseStreamItem> StreamChatAsync(
        ChatRequest request,
        Func<ToolCallback, Task<JsonDocument>> toolAction,
        MessageParameters messageParameters,
        Guid chatId,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        List<Function> toolCallFunctions = [];
        StringBuilder stringBuilder = new StringBuilder();
        await foreach (var message in this.anthropicClient.Messages.StreamClaudeMessageAsync(
                           messageParameters,
                           cancellationToken))
        {
            if (message is null)
            {
                continue;
            }

            if (message.Usage is not null)
            {
                yield return CreateUsageMessage(request, chatId, message.Usage);
            }

            if (message.StreamStartMessage is not null)
            {
                yield return new ChatStartFragment()
                {
                    ExecutionId = request.Id,
                    ChatId = chatId,
                    ModelName = message.StreamStartMessage.Model,
                    MessageProviderId = message.Id,
                };

                if (message.StreamStartMessage.Usage is not null)
                {
                    yield return CreateUsageMessage(request, chatId, message.StreamStartMessage.Usage);
                }
            }

            if (message.Delta is not null)
            {
                if (!string.IsNullOrEmpty(message.Delta.Text))
                {
                    stringBuilder.Append(message.Delta.Text);

                    yield return new ChatFragment()
                    {
                        ExecutionId = request.Id,
                        ChatId = chatId,
                        Content = message.Delta.Text,
                    };
                }
            }

            if (message.ToolCalls is not null && message.ToolCalls.Any())
            {
                toolCallFunctions.AddRange(message.ToolCalls);
            }
        }

        yield return new ChatEndFragment()
        {
            ChatId = chatId,
        };

        if (toolCallFunctions.Any())
        {
            await foreach (var p in this.HandleToolCallsAsync(
                               toolAction,
                               messageParameters,
                               chatId,
                               stringBuilder,
                               toolCallFunctions,
                               cancellationToken))
            {
                yield return p;
            }

            await foreach (var message in this.StreamChatAsync(
                               request,
                               toolAction,
                               messageParameters,
                               chatId,
                               cancellationToken))
            {
                yield return message;
            }
        }
    }

    private async IAsyncEnumerable<BaseStreamItem> HandleToolCallsAsync(
        Func<ToolCallback,
            Task<JsonDocument>> toolAction,
        MessageParameters messageParameters,
        Guid chatId,
        StringBuilder stringBuilder,
        List<Function> toolCallFunctions,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        // We'll pad the tool calls as anthropic will continue to stream from the previous token.
        yield return new ChatFragment
        {
            ChatId = chatId,
            Content = $"{Environment.NewLine}{Environment.NewLine}",
        };

        var assistantMessage = new Message()
        {
            Role = RoleType.Assistant,
            Content = [],
        };

        var currentResponse = stringBuilder.ToString();
        if (!string.IsNullOrWhiteSpace(currentResponse))
        {
            assistantMessage.Content.Add(new TextContent()
            {
                Text = $"{currentResponse}{Environment.NewLine}{Environment.NewLine}",
            });
        }

        var userMessage = new Message()
        {
            Role = RoleType.User,
            Content = [],
        };

        foreach (var toolCall in toolCallFunctions)
        {
            var jsonDoc = JsonDocument.Parse(string.IsNullOrEmpty(toolCall.Arguments.ToString())
                ? "{}"
                : toolCall.Arguments.ToString());
            var index = toolCallFunctions.IndexOf(toolCall);
            var toolCallId = Guid.NewGuid();
            assistantMessage.Content.Add(new ToolUseContent()
            {
                Name = toolCall.Name,
                Input = toolCall.Parameters,
                Id = toolCall.Id,
            });

            yield return new ToolCall()
            {
                Name = toolCall.Name,
                QueryParameters = jsonDoc,
                Index = index,
                ToolCallId = toolCallId,
                ChatId = chatId,
            };

            Stopwatch toolStopwatch = Stopwatch.StartNew();
            var toolResult = await toolAction.Invoke(new ToolCallback()
            {
                ToolParameters = jsonDoc,
                Index = toolCallFunctions.IndexOf(toolCall),
                ToolName = toolCall.Name,
                ToolCallId = toolCall.Id,
            });

            yield return new ToolResult()
            {
                Result = toolResult.RootElement.GetRawText(),
                Duration = toolStopwatch.Elapsed,
                ToolCallId = toolCallId,
            };

            userMessage.Content.Add(new ToolResultContent
            {
                ToolUseId = toolCall.Id,
                Content =
                [
                    new TextContent
                    {
                        Text = toolResult.RootElement.GetRawText(),
                    },
                ],
                IsError = false,
            });
        }

        messageParameters.Messages.Add(assistantMessage);
        messageParameters.Messages.Add(userMessage);
    }
}
