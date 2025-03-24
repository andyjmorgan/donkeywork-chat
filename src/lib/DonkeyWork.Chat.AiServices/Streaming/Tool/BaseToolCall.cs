// ------------------------------------------------------
// <copyright file="BaseToolCall.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.AiServices.Streaming.Chat;

namespace DonkeyWork.Chat.AiServices.Streaming.Tool;

/// <summary>
/// A base tool call.
/// </summary>
public abstract record BaseToolCall : BaseChatFragment
{
    /// <summary>
    /// Gets the toolcall id.
    /// </summary>
    public Guid ToolCallId { get; init; }
}