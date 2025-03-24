// ------------------------------------------------------
// <copyright file="CreateConversationItem.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.Persistence.Repository.Conversation.Models;

/// <summary>
/// A model to create a conversation.
/// </summary>
public class CreateConversationItem
{
    /// <summary>
    /// Gets the conversation id.
    /// </summary>
    public Guid Id { get; init; }
}