// ------------------------------------------------------
// <copyright file="ChatFragment.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.Common.Models.Streaming.Chat;

/// <summary>
/// A chat fragment.
/// </summary>
public record ChatFragment : BaseChatFragment
{
    /// <summary>
    /// Gets the fragment text.
    /// </summary>
    public string Content { get; init; } = string.Empty;

    /// <summary>
    /// Gets the metadata associated with the fragment.
    /// </summary>
    public Dictionary<string, object?> Metadata { get; init; } = [];
}