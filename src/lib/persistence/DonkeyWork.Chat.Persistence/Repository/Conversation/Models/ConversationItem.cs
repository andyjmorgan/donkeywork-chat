// ------------------------------------------------------
// <copyright file="ConversationItem.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.Persistence.Repository.Conversation.Models;

/// <summary>
/// A conversation item.
/// </summary>
public record ConversationItem
{
    /// <summary>
    /// Gets the conversation id.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Gets the conversation's title.
    /// </summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>
    /// Gets the conversation's messages.
    /// </summary>
    public IEnumerable<ConversationMessageItem> Messages { get; init; } = [];
}