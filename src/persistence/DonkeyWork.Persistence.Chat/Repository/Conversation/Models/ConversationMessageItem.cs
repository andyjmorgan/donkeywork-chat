// ------------------------------------------------------
// <copyright file="ConversationMessageItem.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Models;

namespace DonkeyWork.Persistence.Chat.Repository.Conversation.Models;

/// <summary>
/// A conversation message item.
/// </summary>
public record ConversationMessageItem
{
    /// <summary>
    /// Gets the id.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Gets the message pair id.
    /// </summary>
    public Guid MessagePairId { get; init; }

    /// <summary>
    /// Gets the owner.
    /// </summary>
    public MessageOwner Owner { get; init; }

    /// <summary>
    /// Gets the content.
    /// </summary>
    public string Content { get; init; } = string.Empty;

    /// <summary>
    /// Gets the timestamp.
    /// </summary>
    public DateTimeOffset Timestamp { get; init; }
}