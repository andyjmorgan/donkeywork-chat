// ------------------------------------------------------
// <copyright file="ToolService.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.AiTooling.Base;
using DonkeyWork.Chat.AiTooling.ToolImplementations.CurrentDateTime.Tool;
using DonkeyWork.Chat.AiTooling.ToolImplementations.SerpApi.Tool;
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
}