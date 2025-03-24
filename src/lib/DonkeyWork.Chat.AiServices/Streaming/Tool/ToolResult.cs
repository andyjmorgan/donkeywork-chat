// ------------------------------------------------------
// <copyright file="ToolResult.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.AiServices.Streaming.Tool;

/// <summary>
/// A tool result.
/// </summary>
public record ToolResult : BaseToolCall
{
    /// <summary>
    /// Gets the duration of a tool call.
    /// </summary>
    public TimeSpan Duration { get; init; }

    /// <summary>
    /// Gets the result of a tool call.
    /// </summary>
    required public string Result { get; init; }
}