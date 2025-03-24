// ------------------------------------------------------
// <copyright file="GetConversationModel.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.Api.Models.Conversation;

/// <summary>
/// A model to get a conversation.
/// </summary>
public record GetConversationModel
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
    public IEnumerable<GetConversationMessageModel> Messages { get; init; } = [];
}