// ------------------------------------------------------
// <copyright file="IDelayTool.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Text.Json;
using DonkeyWork.Chat.AiTooling.Base;

namespace DonkeyWork.Chat.AiTooling.ToolImplementations.Delay.Tool;

/// <summary>
/// A tool to delay the execution of a task.
/// </summary>
public interface IDelayTool : ITool
{
    /// <summary>
    /// A tool to delay the execution of a task.
    /// </summary>
    /// <param name="milliseconds">The duration.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task<JsonDocument> DelayAsync(double milliseconds);
}