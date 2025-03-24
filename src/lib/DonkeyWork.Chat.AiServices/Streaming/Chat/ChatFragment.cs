// ------------------------------------------------------
// <copyright file="ChatFragment.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.AiServices.Streaming.Chat;

/// <summary>
/// A chat fragment.
/// </summary>
public record ChatFragment : BaseChatFragment
{
    /// <summary>
    /// Gets the fragment text.
    /// </summary>
    public string Content { get; init; } = string.Empty;
}