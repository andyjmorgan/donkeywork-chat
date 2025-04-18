// ------------------------------------------------------
// <copyright file="ToolService.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.AiTooling.Base;
using DonkeyWork.Chat.AiTooling.Base.Models;
using DonkeyWork.Chat.AiTooling.ToolImplementations.CurrentDateTime.Tool;
using DonkeyWork.Chat.AiTooling.ToolImplementations.SerpApi.Tool;
using DonkeyWork.Chat.Common.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace DonkeyWork.Chat.AiTooling.Services;

/// <inheritdoc />
public class ToolService(IServiceProvider serviceProvider)
    : IToolService
{
    /// <inheritdoc />
    public List<ITool> GetPublicTools()
    {
        return new List<ITool>()
        {
            // Serp is a public tool.
            serviceProvider.GetRequiredService<ISerpTool>(),
            serviceProvider.GetRequiredService<ICurrentDateTimeTool>(),
        };
    }
    
    /// <inheritdoc />
    public List<ToolDefinition> GetUserScopedTools(ToolProviderPosture toolPosture)
    {
        var tools = serviceProvider.GetServices<ITool>().ToList();
        List<ToolDefinition> userScopedTools = [];
        foreach (var tool in tools)
        {
            userScopedTools.AddRange(tool.GetToolDefinitions(toolPosture));
        }

        return userScopedTools;
    }
}