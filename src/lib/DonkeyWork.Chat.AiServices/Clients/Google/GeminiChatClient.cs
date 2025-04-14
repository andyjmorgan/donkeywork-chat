// ------------------------------------------------------
// <copyright file="GeminiChatClient.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using Anthropic.SDK.Messaging;
using DonkeyWork.Chat.AiServices.Clients.Google.Configuration;
using DonkeyWork.Chat.AiServices.Clients.Models;
using DonkeyWork.Chat.AiServices.Streaming;
using DonkeyWork.Chat.AiServices.Streaming.Chat;
using DonkeyWork.Chat.AiServices.Streaming.Tool;
using DonkeyWork.Chat.AiTooling.Base.Models;
using GenerativeAI;
using GenerativeAI.Core;
using GenerativeAI.Types;
using Microsoft.Extensions.Options;
using Tool = GenerativeAI.Types.Tool;

namespace DonkeyWork.Chat.AiServices.Clients.Google;

/// <summary>
/// A google chat client.
/// </summary>
public class GeminiChatClient : IAIChatClient
{
    private readonly GoogleAi googleAiClient;
    /// <summary>
    /// Initializes a new instance of the <see cref="GeminiChatClient"/> class.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <param name="httpClientFactory">THe http client factory.</param>
    public GeminiChatClient(IOptions<GeminiConfiguration> options, IHttpClientFactory httpClientFactory)
    {
        this.googleAiClient = new GoogleAi(options.Value.ApiKey, client: httpClientFactory.CreateClient(nameof(GeminiChatClient)));
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<BaseStreamItem> StreamChatAsync(
        ChatRequest request,
        List<ToolDefinition> toolDefinitions,
        Func<ToolCallback, Task<JsonDocument>> toolAction,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var chatModel = this.googleAiClient.CreateGeminiModel(request.ModelName);
        chatModel.FunctionCallingBehaviour = new FunctionCallingBehaviour()
        {
            AutoCallFunction = false,
        };

        var requestContent = this.GenerateContent(request, toolDefinitions);
        await foreach (var chatFragment in this.StreamChatAsync(requestContent, chatModel, toolAction, cancellationToken))
        {
            yield return chatFragment with { ExecutionId = request.Id };
        }
    }

    private async IAsyncEnumerable<BaseStreamItem> StreamChatAsync(
        GenerateContentRequest requestContent,
        GeminiModel chatModel,
        Func<ToolCallback, Task<JsonDocument>> toolAction,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        Guid chatId = Guid.NewGuid();
        List<Part> parts = new List<Part>();
        bool hasResponded = false;
        await foreach (var message in chatModel.StreamContentAsync(requestContent, cancellationToken))
        {
            // Google will report the same token count for all messages in the stream.
            if (message.UsageMetadata is not null && message.UsageMetadata.CandidatesTokenCount > 0)
            {
                yield return new TokenUsage()
                {
                    ChatId = chatId,
                    InputTokens = message.UsageMetadata?.PromptTokensDetails?.Sum(x => x.TokenCount) ?? 0,
                    OutputTokens = message.UsageMetadata?.CandidatesTokenCount ?? 0,
                };
            }

            if (!hasResponded)
            {
                yield return new ChatStartFragment()
                {
                    ChatId = chatId,
                    ModelName = message.ModelVersion ?? string.Empty,
                };
                hasResponded = true;
            }

            if (message.Candidates is not null)
            {
                foreach (var candidate in message.Candidates)
                {
                    parts.AddRange(candidate.Content?.Parts ?? []);
                    foreach (var part in candidate?.Content?.Parts ?? [])
                    {
                        if (part.Text is not null)
                        {
                            yield return new ChatFragment()
                            {
                                Content = message.Text,
                                ChatId = chatId,
                            };
                        }
                    }
                }
            }
        }

        yield return new ChatEndFragment()
        {
            ChatId = chatId,
        };

        // Catch tool calls at the end.
        if (parts.Any(x => x.FunctionCall is not null))
        {
            var toolCalls = parts.Where(x => x.FunctionCall is not null).ToList();

            // Add model response.
            var modelResponse = new Content()
            {
                Role = "model",
                Parts = [],
            };

            var textParts = parts.Where(x => x.Text is not null).ToList();

            if (textParts.Any())
            {
                modelResponse.Parts.Add(
                    new Part()
                    {
                        Text = string.Join(string.Empty, parts.Where(x => x.Text is not null).Select(x => x.Text)),
                    });
            }

            var toolResponseContent = new Content()
            {
                Role = "function",
                Parts = [],
            };

            foreach (var toolCall in toolCalls)
            {
                if (toolCall.FunctionCall?.Args is null)
                {
                    continue;
                }

                modelResponse.Parts.Add(new Part()
                {
                    FunctionCall = toolCall.FunctionCall,
                });

                var jsonDoc = JsonDocument.Parse(toolCall.FunctionCall.Args.Root.ToJsonString());
                Stopwatch toolStopwatch = Stopwatch.StartNew();

                var toolCallId = Guid.NewGuid();
                yield return new ToolCall()
                {
                    Name = toolCall.FunctionCall.Name,
                    QueryParameters = jsonDoc,
                    Index = toolCalls.IndexOf(toolCall),
                    ToolCallId = toolCallId,
                    ChatId = chatId,
                };

                var toolResult = await toolAction.Invoke(new ToolCallback()
                {
                    ToolParameters = jsonDoc,
                    Index = toolCalls.IndexOf(toolCall),
                    ToolName = toolCall.FunctionCall.Name,
                    ToolCallId = toolCall.FunctionCall?.Id ?? string.Empty,
                });

                // Google does not like arrays in the root function response.
                if (toolResult.RootElement.ValueKind == JsonValueKind.Array)
                {
                    var dynamicObject = new JsonObject
                    {
                        ["result"] = JsonNode.Parse(toolResult.RootElement.GetRawText()),
                    };
                    toolResult = JsonDocument.Parse(dynamicObject.Root.ToJsonString());
                }

                toolStopwatch.Stop();
                toolResponseContent.Parts.Add(new Part()
                {
                    FunctionResponse = new FunctionResponse()
                    {
                        Name = toolCall.FunctionCall?.Name ?? string.Empty,
                        Response = JsonNode.Parse(toolResult.RootElement.GetRawText()),
                        Id = toolCall.FunctionCall?.Id ?? null,
                    },
                });

                yield return new ToolResult()
                {
                    Result = toolResult.RootElement.GetRawText(),
                    Duration = toolStopwatch.Elapsed,
                    ToolCallId = toolCallId,
                };
            }

            requestContent.Contents.Add(modelResponse);
            requestContent.Contents.Add(toolResponseContent);
            await foreach (var fragment in
                           this.StreamChatAsync(
                               requestContent,
                               chatModel,
                               toolAction,
                               cancellationToken))
            {
                yield return fragment;
            }
        }
    }

    private GenerateContentRequest GenerateContent(ChatRequest request, List<ToolDefinition> toolDefinitions)
    {
        var chatMessages = request.GetNonSystemMessages();
        var systemMessages = request.GetSystemMessages();
        return new GenerateContentRequest()
        {
            Contents = request.GetNonSystemMessages().Select(x => new Content()
            {
                Role = x.Role.ToString(),
                Parts = new List<Part>()
                {
                    new Part()
                    {
                        Text = x.Content,
                    },
                },
            }).ToList(),
            SystemInstruction = systemMessages.Count == 0 ? null
                : new Content()
                {
                    Role = "system",
                    Parts = request.GetSystemMessages().Select(x => new Part()
                    {
                        Text = x.Content,
                    }).ToList(),
                },
            Tools = toolDefinitions.Select(x => new Tool
            {
                FunctionDeclarations = new List<FunctionDeclaration>()
                {
                    new FunctionDeclaration()
                    {
                        Name = x.Name,
                        Description = x.Description,
                        Parameters = this.GetToolFunctionDefinitionSchema(x.ToolFunctionDefinition),
                    },
                },
            }).ToList(),
        };
    }

    private Schema GetToolFunctionParameterDefinitionSchema(
        ToolFunctionParameterDefinition toolFunctionParameterDefinition)
    {
        return new Schema()
        {
            Description = toolFunctionParameterDefinition.Description,
            Type = toolFunctionParameterDefinition.Type,
            Enum = toolFunctionParameterDefinition.Enum,
            Items = toolFunctionParameterDefinition.Items is not null
                ? this.GetToolFunctionParameterDefinitionSchema(toolFunctionParameterDefinition.Items)
                : null,
        };
    }

    private Schema GetToolFunctionDefinitionSchema(ToolFunctionDefinition toolFunctionDefinition)
    {
        return new Schema()
        {
            Type = toolFunctionDefinition.Type,
            Required = toolFunctionDefinition.Required,
            Properties = toolFunctionDefinition.Properties
                .Select(
                    p =>
                        new KeyValuePair<string,Schema>(
                            p.Key, this.GetToolFunctionParameterDefinitionSchema(p.Value))).ToDictionary(),
        };
    }
}