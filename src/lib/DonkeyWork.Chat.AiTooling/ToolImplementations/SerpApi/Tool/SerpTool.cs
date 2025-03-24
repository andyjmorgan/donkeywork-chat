// ------------------------------------------------------
// <copyright file="SerpTool.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.ComponentModel;
using System.Text.Json;
using DonkeyWork.Chat.AiTooling.Base.Attributes;
using DonkeyWork.Chat.AiTooling.ToolImplementations.SerpApi.Api;

namespace DonkeyWork.Chat.AiTooling.ToolImplementations.SerpApi.Tool;

/// <inheritdoc cref="ISerpTool"/>
public class SerpTool(ISerpApiSearch serpApiSearch)
    : Base.Tool, ISerpTool
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