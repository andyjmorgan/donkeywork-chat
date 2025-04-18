// ------------------------------------------------------
// <copyright file="McpToolsHandler.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Security.Claims;
using System.Text.Json;
using DonkeyWork.Chat.AiTooling.Base.Models;
using DonkeyWork.Chat.AiTooling.Services;
using DonkeyWork.Chat.Common.Contracts;
using DonkeyWork.Chat.Common.Providers;
using DonkeyWork.Chat.Common.UserContext;
using ModelContextProtocol.Protocol.Types;
using ModelContextProtocol.Server;

namespace DonkeyWork.Chat.McpServer.Handlers;

/// <inheritdoc />
public class McpToolsHandler(
    IToolService toolService,
    IHttpContextAccessor httpContextAccessor,
    IUserPostureService userPostureService,
    IUserContextProvider userContextProvider)
    : IMcpToolsHandler
{
    /// <summary>
    /// Serializes an object to a <see cref="JsonElement"/>.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <typeparam name="T">The type.</typeparam>
    /// <returns>A <see cref="JsonElement"/>.</returns>
    public static JsonElement SerializeToJsonElement<T>(T obj)
    {
        // Serialize the object to a JSON string
        string jsonString = JsonSerializer.Serialize(obj);

        // Parse the JSON string into a JsonDocument
        using JsonDocument jsonDocument = JsonDocument.Parse(jsonString);

        // Return the root element of the JsonDocument
        return jsonDocument.RootElement.Clone();
    }

    /// <inheritdoc />
    public async Task<ListToolsResult> HandleListAsync(
        RequestContext<ListToolsRequestParams> context,
        CancellationToken cancellationToken)
    {
        var tools = await this.GetToolsAsync(cancellationToken);

        return new ListToolsResult()
        {
            Tools = tools.Select(x => new Tool
            {
                Description = x.Description,
                Name = x.Name,
                InputSchema = SerializeToJsonElement(x.ToolFunctionDefinition),
            }).ToList(),
        };
    }

    /// <inheritdoc />
    public async Task<CallToolResponse> HandleCallAsync(RequestContext<CallToolRequestParams> context, CancellationToken cancellationToken)
    {
        var tools = await this.GetToolsAsync(cancellationToken);
        var tool = tools.FirstOrDefault(t => t.Name == context?.Params?.Name);
        if (tool is null)
        {
            return ReturnErrorResult("Tool not found");
        }

        if (context.Params is null)
        {
           return ReturnErrorResult("Parameters not provided");
        }

        var result = await tool.Tool.InvokeFunctionAsync(
            context.Params.Name,
            context.Params.Arguments,
            cancellationToken);

        return ReturnResultOrError(result);
    }

    private static CallToolResponse ReturnErrorResult(string error)
    {
        return new CallToolResponse()
        {
            Content = new List<Content>()
            {
                new Content()
                {
                    Type = "text",
                    Text = error,
                },
            },
            IsError = true,
        };
    }

    private static CallToolResponse ReturnResultOrError(JsonDocument? result)
    {
        if (result is null)
        {
            return new CallToolResponse()
            {
                Content = new List<Content>()
                {
                    new Content()
                    {
                        Type = "text",
                        Text = "empty response from tool",
                    },
                },
                IsError = true,
            };
        }

        return new CallToolResponse()
        {
            Content = new List<Content>()
            {
                new Content()
                {
                    Type = "text",
                    Text = result?.RootElement.GetRawText(),
                },
            },
            IsError = false,
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

    private async Task<List<ToolDefinition>> GetToolsAsync(CancellationToken cancellationToken)
    {
        List<ToolDefinition> tools;
        if (httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated == true)
        {
            this.HandleUserProvider();
            tools = toolService.GetUserScopedTools(await userPostureService.GetUserPosturesAsync(cancellationToken));
        }
        else
        {
            tools = toolService.GetPublicTools().SelectMany(x => x.GetToolDefinitions(new ToolProviderPosture()
            {
                GenericIntegrations = [],
                UserTokens = [],
            })).ToList();
        }

        return tools;
    }
}