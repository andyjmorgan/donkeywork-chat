// ------------------------------------------------------
// <copyright file="OpenAiExtensions.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Text;
using DonkeyWork.Chat.AiTooling.Base.Models;
using OpenAI.Chat;

namespace DonkeyWork.Chat.AiServices.Clients.OpenAi;

/// <summary>
/// Helper Extensions for OpenAi.
/// </summary>
public static class OpenAiExtensions
{
    /// <summary>
    /// Creates an Open Ai Function tool from a <see cref="ToolDefinition"/>.
    /// </summary>
    /// <param name="toolDefinition">The tool definition.</param>
    /// <returns>The <see cref="ChatTool"/>.</returns>
    public static ChatTool CreateOpenAiFunctionTool(this ToolDefinition toolDefinition)
    {
        return ChatTool.CreateFunctionTool(
            toolDefinition.Name,
            toolDefinition.Description,
            functionParameters: BinaryData.FromBytes(
                Encoding.UTF8.GetBytes(toolDefinition.Parameters)));
    }
}