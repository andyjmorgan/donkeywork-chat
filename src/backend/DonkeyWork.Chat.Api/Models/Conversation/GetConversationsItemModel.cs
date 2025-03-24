// ------------------------------------------------------
// <copyright file="GetConversationsItemModel.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.Api.Models.Conversation;

/// <summary>
/// A conversation List Model.
/// </summary>
public record GetConversationsItemModel
{
    /// <summary>
    /// Gets the conversation id.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Gets the Title.
    /// </summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>
    /// Gets a portion of the last message.
    /// </summary>
    public string LastMessage { get; init; } = string.Empty;

    /// <summary>
    /// Gets the message Count.
    /// </summary>
    public int MessageCount { get; init; }

    /// <summary>
    /// Gets the Created At.
    /// </summary>
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>
    /// Gets the Updated At.
    /// </summary>
    public DateTimeOffset UpdatedAt { get; init; }
}