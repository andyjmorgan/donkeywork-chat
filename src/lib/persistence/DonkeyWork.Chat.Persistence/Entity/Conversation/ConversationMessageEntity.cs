// ------------------------------------------------------
// <copyright file="ConversationMessageEntity.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Persistence.Entity.Base;

// ReSharper disable EntityFramework.ModelValidation.UnlimitedStringLength
namespace DonkeyWork.Chat.Persistence.Entity.Conversation;

/// <summary>
/// A conversation message.
/// This can be paired with the <see cref="MessagePairId"/> to form the request and response.
/// </summary>
public class ConversationMessageEntity : BaseUserEntity
{
    /// <summary>
    /// Gets the conversation Id.
    /// </summary>
    public Guid ConversationId { get; init; }

    /// <summary>
    /// Gets the message pair (the message pair is the same for both the sender and the receiver).
    /// </summary>
    public Guid MessagePairId { get; init; }

    /// <summary>
    /// Gets the message owner.
    /// </summary>
    public MessageOwner MessageOwner { get; init; }

    /// <summary>
    /// Gets the message.
    /// </summary>
    public string Message { get; init; } = string.Empty;

    /// <summary>
    /// Gets the conversation entity.
    /// </summary>
    public virtual ConversationEntity? Conversation { get; init; }
}