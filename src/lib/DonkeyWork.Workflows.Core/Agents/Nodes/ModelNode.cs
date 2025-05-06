// ------------------------------------------------------
// <copyright file="ModelNode.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.AiServices.Clients;
using DonkeyWork.Chat.AiServices.Clients.Models;
using DonkeyWork.Chat.AiTooling.Services;
using DonkeyWork.Chat.Common.Models.Agents.Metadata;
using DonkeyWork.Chat.Common.Models.Agents.Results;
using DonkeyWork.Chat.Common.Models.Chat;
using DonkeyWork.Chat.Common.Models.Streaming;
using DonkeyWork.Chat.Common.Models.Streaming.Chat;
using DonkeyWork.Workflows.Core.Agents.Execution;
using Microsoft.Extensions.Logging;

namespace DonkeyWork.Workflows.Core.Agents.Nodes;

/// <summary>
/// A model node.
/// </summary>
public class ModelNode : BaseAgentNode
{
    private readonly IAIChatProviderFactory aiChatProviderFactory;
    private readonly IToolService toolService;

    /// <summary>
    /// Gets or sets the model parameters.
    /// </summary>
    public AgentModelNodeMetadata Parameters { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ModelNode"/> class.
    /// </summary>
    /// <param name="aiChatProviderFactory">The AI chat provider factory.</param>
    /// <param name="toolService">The tool service.</param>
    /// <param name="agentContext">The agent context.</param>
    /// <param name="logger">The logger.</param>
    public ModelNode(
        IAIChatProviderFactory aiChatProviderFactory,
        IToolService toolService,
        IAgentContext agentContext,
        ILogger<BaseAgentNode> logger)
        : base(agentContext, logger)
    {
        this.toolService = toolService;
        this.aiChatProviderFactory = aiChatProviderFactory;
    }

    /// <inheritdoc />
    protected internal override async Task<BaseAgentNodeResult> ExecuteNodeAsync(List<BaseAgentNodeResult> inputs, CancellationToken cancellationToken = default)
    {
        var tools = this.toolService.GetUserScopedTools(this.AgentContext.UserPosture);
        var chatProvider = this.aiChatProviderFactory.CreateChatClient(this.Parameters.ModelConfiguration.ProviderType);

        var messages = this.AgentContext
            .Prompts
            .Where(
                x =>
                    this.Parameters.SystemPrompts.Contains(x.Key))
            .Select(x =>
                new GenericChatMessage()
                {
                    Role = GenericMessageRole.System,
                    Content = string.Concat(x.Value.Content),
                })
            .Concat(this.AgentContext.InputDetails.MessageHistory);
        messages = messages
            .Concat([
                new GenericChatMessage()
                {
                    Role = GenericMessageRole.User,
                    Content = string.Join(
                        Environment.NewLine,
                        inputs.Select(x => x.Text())),
                },
            ])
            .ToList();

        List<BaseStreamItem> results = new List<BaseStreamItem>();
        if (this.Parameters.ModelConfiguration.Streaming)
        {
            await foreach (var result in chatProvider.StreamChatAsync(
                               new ChatRequest()
                               {
                                   ModelName = this.Parameters.ModelConfiguration.ModelName,
                                   Id = Guid.NewGuid(),
                                   Messages = messages.ToList(),
                               },
                               tools,
                               async callback => await this.toolService.ExecuteToolAsync(callback, cancellationToken),
                               cancellationToken: cancellationToken))

            {
                await this.AgentContext.StreamService.AddStreamItem(result, cancellationToken);
                results.Add(result);
            }
        }
        else
        {
            await foreach (var result in chatProvider.ChatAsync(
                               new ChatRequest()
                               {
                                   ModelName = this.Parameters.ModelConfiguration.ModelName,
                                   Id = Guid.NewGuid(),
                                   Messages = messages.ToList(),
                               },
                               tools,
                               async callback => await this.toolService.ExecuteToolAsync(callback, cancellationToken),
                               cancellationToken: cancellationToken))

            {
                await this.AgentContext.StreamService.AddStreamItem(result, cancellationToken);
                results.Add(result);
            }
        }

        return new ModelNodeResult(this)
        {
            Message = string.Concat(results.OfType<ChatFragment>().Select(x => x.Content).ToList()),
            Metadata = [],
            WasStreamed = false,
        };
    }
}
