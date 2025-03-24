// ------------------------------------------------------
// <copyright file="ICurrentDateTimeTool.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.AiTooling.Base;

namespace DonkeyWork.Chat.AiTooling.ToolImplementations.CurrentDateTime.Tool;

/// <summary>
/// A tool to get the current date and time.
/// </summary>
public interface ICurrentDateTimeTool : ITool
{
    /// <summary>
    /// Gets the current datetime.
    /// </summary>
    /// <returns>A string.</returns>
    public string GetCurrentDateTime();
}