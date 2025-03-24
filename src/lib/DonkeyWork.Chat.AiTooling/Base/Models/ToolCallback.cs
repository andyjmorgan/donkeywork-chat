// ------------------------------------------------------
// <copyright file="ToolCallback.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Text.Json;

namespace DonkeyWork.Chat.AiTooling.Base.Models;

/// <summary>
/// A tool callback.
/// </summary>
public class ToolCallback
{
    /// <summary>
    /// Gets or sets the tool index.
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// Gets or sets the tool name.
    /// </summary>
    public string ToolName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the tool call id.
    /// </summary>
    public string ToolCallId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the tool parameters.
    /// </summary>
    required public JsonDocument ToolParameters { get; set; }
}