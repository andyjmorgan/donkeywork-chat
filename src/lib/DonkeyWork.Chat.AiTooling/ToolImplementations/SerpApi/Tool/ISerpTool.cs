// ------------------------------------------------------
// <copyright file="ISerpTool.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Text.Json;
using DonkeyWork.Chat.AiTooling.Base;

namespace DonkeyWork.Chat.AiTooling.ToolImplementations.SerpApi.Tool;

/// <summary>
/// A tool to search the internet for relevant results.
/// </summary>
public interface ISerpTool : ITool
{
    /// <summary>
    /// A tool to search the internet for relevant results.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <returns>A <see cref="JsonDocument"/>.</returns>
    public JsonDocument SearchGoogleAsync(string query);
}