// ------------------------------------------------------
// <copyright file="GetConversationsModel.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.Api.Models.Conversation;

/// <summary>
/// Gets the conversations model.
/// </summary>
public record GetConversationsModel
{
    /// <summary>
    /// Gets the count of conversations.
    /// </summary>
    public int Count { get; init; }

    /// <summary>
    /// Gets the conversations.
    /// </summary>
    public IEnumerable<GetConversationsItemModel> Conversations { get; init; } = [];
}