// ------------------------------------------------------
// <copyright file="DelayTool.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.ComponentModel;
using System.Text.Json;
using DonkeyWork.Chat.AiTooling.Attributes;

namespace DonkeyWork.Chat.AiTooling.ToolImplementations.Delay.Tool;

/// <summary>
/// A tool to delay the execution of a task.
/// </summary>
public class DelayTool
    : Base.Tool, IDelayTool
{
    /// <inheritdoc />
    [ToolFunction]
    [Description("A tool to get the current date and time in ISO 8601 format.")]
    public async Task<JsonDocument> DelayAsync(double milliseconds)
    {
        await Task.Delay(TimeSpan.FromMilliseconds(milliseconds));
        return JsonDocument.Parse(JsonSerializer.Serialize(new
        {
            Message = $"Delayed for {milliseconds} milliseconds.",
            Timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.ffffZ"),
        }));
    }
}