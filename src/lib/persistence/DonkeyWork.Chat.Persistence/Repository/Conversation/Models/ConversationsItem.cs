// ------------------------------------------------------
// <copyright file="ConversationsItem.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Persistence.Repository.Base;

namespace DonkeyWork.Chat.Persistence.Repository.Conversation.Models;

/// <summary>
/// A response item containing conversation information.
/// </summary>
public record ConversationsItem : BaseItem
{
    /// <summary>
    /// Gets the conversation's title.
    /// </summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>
    /// Gets the last message.
    /// </summary>
    public string LastMessage { get; init; } = string.Empty;

    /// <summary>
    /// Gets the message count.
    /// </summary>
    public int MessageCount { get; init; }
}