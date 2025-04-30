// ------------------------------------------------------
// <copyright file="ActionExecutionService.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json;
using DonkeyWork.Chat.AiServices.Clients;
using DonkeyWork.Chat.AiServices.Clients.Models;
using DonkeyWork.Chat.AiTooling.Exceptions;
using DonkeyWork.Chat.AiTooling.Services;
using DonkeyWork.Chat.Common.Contracts;
using DonkeyWork.Chat.Common.Models;
using DonkeyWork.Chat.Common.Models.Prompt.Content;
using DonkeyWork.Chat.Common.Models.Streaming;
using DonkeyWork.Chat.Common.Models.Streaming.Request;
using DonkeyWork.Persistence.Agent.Repository.Action.Models.Execution;

namespace DonkeyWork.Workflows.Core.Actions.Services.ActionExecution;

/// <inheritdoc />
public class ActionExecutionService : IActionExecutionService
{
    private readonly IAIChatProviderFactory chatProviderFactory;
    private readonly IToolService toolService;
    private readonly IUserPostureService userPostureService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ActionExecutionService"/> class.
    /// </summary>
    /// <param name="chatProviderFactory">The chat provider factory.</param>
    /// <param name="toolService">The tool service.</param>
    /// <param name="userPostureService">The user posture service.</param>
    public ActionExecutionService(IAIChatProviderFactory chatProviderFactory, IToolService toolService, IUserPostureService userPostureService)
    {
        this.chatProviderFactory = chatProviderFactory;
        this.toolService = toolService;
        this.userPostureService = userPostureService;
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<BaseStreamItem> ExecuteActionAsync(ActionExecutionItem actionItem, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        // Pull the client.
        // hydrate the messages.
        // Send the messages to the client.
        // Collect the results.
        // Set the result.
        var chatClient = this.chatProviderFactory.CreateChatClient(actionItem.ActionModelConfiguration.ProviderType);
        yield return new RequestStart()
        {
            ExecutionId = actionItem.ExecutionId,
        };

        var toolDefinitions = this.toolService.GetUserScopedTools(await this.userPostureService.GetUserPosturesAsync(cancellationToken));
        var request = new ChatRequest()
        {
            Metadata = actionItem.ActionModelConfiguration.Metadata.Select(x => new KeyValuePair<string, string>(x.Key, x.Value.ToString() ?? string.Empty)).ToDictionary(),
            ModelName = actionItem.ActionModelConfiguration.ModelName,
            Id = actionItem.ExecutionId,
            Messages = actionItem.SystemPrompts.SelectMany(x => x.Content).Select(x => new GenericChatMessage
            {
                Content = x,
                Role = GenericMessageRole.System,
            }).ToList(),
        };

        request.Messages.AddRange(actionItem.ActionItems.SelectMany(x => x.Messages).Where(x => x.Content is TextContent).Select(x => new GenericChatMessage()
        {
            Content = ((TextContent)x.Content).Text,
            Role = x.Role == MessageOwner.Assistant ? GenericMessageRole.Assistant : GenericMessageRole.User,
        }));

        Stopwatch stopWatch = Stopwatch.StartNew();
        await foreach (var streamItem in chatClient.ChatAsync(
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

                               var result = await tool.Tool!.InvokeFunctionAsync(
                                   x.ToolName,
                                   x.ToolParameters,
                                   cancellationToken);
                               if (result is { } jsonDocument)
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

        yield return new RequestEnd()
        {
            ExecutionId = actionItem.ExecutionId,
            Duration = stopWatch.Elapsed,
        };
    }
}
