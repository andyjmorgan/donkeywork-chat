// ------------------------------------------------------
// <copyright file="ToolCall.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Text.Json;

namespace DonkeyWork.Chat.Common.Models.Streaming.Tool;

/// <summary>
/// A tool call.
/// </summary>
public record ToolCall : BaseToolCall
{
    /// <summary>
    /// Gets the tool call index.
    /// </summary>
    public int Index { get; init; }

    /// <summary>
    /// Gets the tool call name.
    /// </summary>
    required public string Name { get; init; }

    /// <summary>
    /// Gets the query parameters.
    /// </summary>
    required public JsonDocument QueryParameters { get; init; }
}