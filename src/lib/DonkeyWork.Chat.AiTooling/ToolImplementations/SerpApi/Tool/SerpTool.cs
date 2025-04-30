// ------------------------------------------------------
// <copyright file="SerpTool.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.ComponentModel;
using System.Text.Json;
using DonkeyWork.Chat.AiTooling.Attributes;
using DonkeyWork.Chat.AiTooling.ToolImplementations.SerpApi.Api;
using DonkeyWork.Chat.Common.Models.Providers.Tools;
using Microsoft.Extensions.Logging;

namespace DonkeyWork.Chat.AiTooling.ToolImplementations.SerpApi.Tool;

/// <inheritdoc cref="ISerpTool"/>
[GenericToolProvider(ToolProviderType.Serp)]
[ToolProviderApplicationType(ToolProviderApplicationType.Serp)]
public class SerpTool(ISerpApiSearch serpApiSearch, ILogger<SerpTool> logger)
    : Base.Tool(logger), ISerpTool
{
    /// <inheritdoc />
    [ToolFunction]
    [Description("A tool to search the internet for relevant results.")]
    public JsonDocument SearchGoogleAsync(
        [Description("Search terms that you wish to use to search the internet.")]
        string query)
    {
        return serpApiSearch.SearchGoogle(query);
    }
}