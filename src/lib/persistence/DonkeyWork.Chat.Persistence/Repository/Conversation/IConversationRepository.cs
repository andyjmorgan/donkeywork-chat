// ------------------------------------------------------
// <copyright file="IConversationRepository.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Persistence.Common;
using DonkeyWork.Chat.Persistence.Entity.Conversation;
using DonkeyWork.Chat.Persistence.Repository.Conversation.Models;

namespace DonkeyWork.Chat.Persistence.Repository.Conversation;

/// <summary>
/// A repository for conversations.
/// </summary>
public interface IConversationRepository
{
    /// <summary>
    /// Gets all conversations.
    /// </summary>
    /// <param name="pagingParameters">The paging parameters.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A response of <see cref="GetConversationsResponse"/>.</returns>
    public Task<GetConversationsResponse> GetConversationsAsync(PagingParameters pagingParameters, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a conversation.
    /// </summary>
    /// <param name="conversationId">The id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task<ConversationItem?> GetConversationAsync(Guid conversationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a conversation.
    /// </summary>
    /// <param name="conversationId">The conversation id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task<bool> DeleteConversationAsync(Guid conversationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Renames a conversation title.
    /// </summary>
    /// <param name="conversationId">The conversation id.</param>
    /// <param name="title">The new title.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task<bool> UpdateConversationTitleAsync(Guid conversationId, string title, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a conversation.
    /// </summary>
    /// <param name="id">The conversation id.</param>
    /// <param name="userContent">The user content.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task AddConversationAsync(Guid id, string userContent, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a conversation.
    /// </summary>
    /// <param name="conversationId">The conversation id.</param>
    /// <param name="executionId">The execution id.</param>
    /// <param name="messageOwner">The owner.</param>
    /// <param name="content">The content.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task AddMessageToConversationAsync(Guid conversationId, Guid executionId, MessageOwner messageOwner, string content, CancellationToken cancellationToken = default);
}