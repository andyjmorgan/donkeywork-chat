// ------------------------------------------------------
// <copyright file="CurrentDateTimeTool.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.ComponentModel;
using System.Text.Json;
using DonkeyWork.Chat.AiTooling.Attributes;

namespace DonkeyWork.Chat.AiTooling.ToolImplementations.CurrentDateTime.Tool;

/// <inheritdoc cref="ICurrentDateTimeTool"/>
public class CurrentDateTimeTool : Base.Tool, ICurrentDateTimeTool
{
    /// <summary>
    /// A tool to get the current date and time in utc format.
    /// </summary>
    /// <returns>A string representing a <see cref="DateTimeOffset"/>.</returns>
    [ToolFunction]
    [Description("A tool to get the current date and time in ISO 8601 format.")]
    public JsonDocument GetCurrentDateTime()
    {
        return JsonDocument.Parse(JsonSerializer.Serialize(new
        {
            ISO8601 = DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.ffffZ"),
        }));
    }
}