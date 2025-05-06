// ------------------------------------------------------
// <copyright file="ToolService.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Text.Json;
using DonkeyWork.Chat.AiTooling.Base;
using DonkeyWork.Chat.AiTooling.Base.Models;
using DonkeyWork.Chat.AiTooling.Exceptions;
using DonkeyWork.Chat.AiTooling.ToolImplementations.CurrentDateTime.Tool;
using DonkeyWork.Chat.AiTooling.ToolImplementations.Delay.Tool;
using DonkeyWork.Chat.Common.Models.Providers.Posture;
using Microsoft.Extensions.DependencyInjection;

namespace DonkeyWork.Chat.AiTooling.Services;

/// <inheritdoc />
public class ToolService(IServiceProvider serviceProvider)
    : IToolService
{
    private List<ToolDefinition> userScopedTools = [];
    private SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

    /// <inheritdoc />
    public List<ITool> GetPublicTools()
    {
        return new List<ITool>()
        {
            // Serp is a public tool.
            serviceProvider.GetRequiredService<ICurrentDateTimeTool>(),
            serviceProvider.GetRequiredService<IDelayTool>(),
        };
    }

    /// <inheritdoc />
    public List<ToolDefinition> GetUserScopedTools(ToolProviderPosture toolPosture)
    {
        this.semaphore.Wait();
        if (this.userScopedTools.Any())
        {
            return this.userScopedTools;
        }

        var tools = serviceProvider.GetServices<ITool>().ToList();
        foreach (var tool in tools)
        {
            this.userScopedTools.AddRange(tool.GetToolDefinitions(toolPosture));
        }

        this.semaphore.Release();
        return this.userScopedTools;
    }

    /// <inheritdoc />
    public async Task<JsonDocument> ExecuteToolAsync(ToolCallback toolCallback, CancellationToken cancellationToken = default)
    {
        var tool = this.userScopedTools.FirstOrDefault(t => t.Name == toolCallback.ToolName);

        if (tool is null || tool.Tool is null)
        {
            throw new UnknownToolDefinitionException(toolCallback.ToolName);
        }

        var result = await tool.Tool.InvokeFunctionAsync(
            toolCallback.ToolName,
            toolCallback.ToolParameters,
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
    }
}
