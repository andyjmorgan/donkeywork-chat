// ------------------------------------------------------
// <copyright file="TokenUsage.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.AiServices.Streaming.Chat;

namespace DonkeyWork.Chat.AiServices.Streaming;

/// <summary>
/// A token usage.
/// </summary>
public record TokenUsage : BaseChatFragment
{
    /// <summary>
    /// Gets the input tokens.
    /// </summary>
    public int InputTokens { get; init; }

    /// <summary>
    /// Gets the output tokens.
    /// </summary>
    public int OutputTokens { get; init; }
}