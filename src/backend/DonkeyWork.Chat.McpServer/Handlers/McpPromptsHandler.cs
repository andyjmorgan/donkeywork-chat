// ------------------------------------------------------
// <copyright file="McpPromptsHandler.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Security.Claims;
using DonkeyWork.Chat.Common.UserContext;
using DonkeyWork.Chat.Persistence.Common;
using DonkeyWork.Chat.Persistence.Repository.Prompt;
using ModelContextProtocol.Protocol.Types;
using ModelContextProtocol.Server;

namespace DonkeyWork.Chat.McpServer.Handlers;

/// <inheritdoc />
public class McpPromptsHandler(

    IHttpContextAccessor httpContextAccessor,
    IUserContextProvider userContextProvider,
    IPromptRepository promptRepository)
    : IMcpPromptsHandler
{
    /// <inheritdoc />
    public async Task<ListPromptsResult> HandleListAsync(RequestContext<ListPromptsRequestParams> context, CancellationToken cancellationToken)
    {
        if (httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated == true)
        {
            this.HandleUserProvider();
        }

        var prompts = await promptRepository.GetPromptsAsync(new PagingParameters(), cancellationToken);

        return new ListPromptsResult()
        {
            Prompts = prompts.Prompts.Select(
                x => new Prompt
                {
                    Name = x.Title,
                    Description = x.Description,
                    Arguments = [],
                }).ToList(),
            NextCursor = (prompts.Prompts.Count() - 1).ToString(),
        };
    }

    /// <inheritdoc />
    public async Task<GetPromptResult> HandleGetAsync(RequestContext<GetPromptRequestParams> context, CancellationToken cancellationToken)
    {
        if (httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated == true)
        {
            this.HandleUserProvider();
            var prompt = await promptRepository.GetPromptContentItemAsync(context.Params?.Name ?? string.Empty, cancellationToken);
            if (prompt != null)
            {
                return new GetPromptResult()
                {
                    Description = prompt.Content,
                    Messages = new List<PromptMessage>()
                    {
                        new PromptMessage()
                        {
                            Content = new Content()
                            {
                                Type = "text",
                                Text = prompt.Content,
                            },
                            Role = Role.Assistant,
                        },
                    },
                };
            }
        }

        return new GetPromptResult()
        {
            Description = "Prompt not found",
            Messages = [],
        };
    }

    private void HandleUserProvider()
    {
        if (userContextProvider?.UserId == Guid.Empty)
        {
            var userIdClaim = httpContextAccessor.HttpContext?.User.Claims
                .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim is not null)
            {
                userContextProvider.SetUserId(Guid.Parse(userIdClaim.Value));
            }
        }
    }
}