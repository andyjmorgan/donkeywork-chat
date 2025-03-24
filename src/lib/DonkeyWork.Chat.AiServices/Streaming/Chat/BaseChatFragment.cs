// ------------------------------------------------------
// <copyright file="BaseChatFragment.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.AiServices.Streaming.Chat;

/// <summary>
/// A base chat fragment.
/// </summary>
public abstract record BaseChatFragment : BaseStreamItem
{
    /// <summary>
    /// Gets the Chat id.
    /// </summary>
    public Guid ChatId { get; init; }
}