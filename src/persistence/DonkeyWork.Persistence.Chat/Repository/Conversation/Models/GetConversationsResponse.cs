// ------------------------------------------------------
// <copyright file="GetConversationsResponse.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Persistence.Chat.Repository.Conversation.Models;

/// <summary>
/// A response containing conversations.
/// </summary>
public record GetConversationsResponse
{
    /// <summary>
    /// Gets the total count of conversations.
    /// </summary>
    public int TotalCount { get; init; }

    /// <summary>
    /// Gets the conversations.
    /// </summary>
    public IEnumerable<ConversationsItem> Conversations { get; init; } = [];
}