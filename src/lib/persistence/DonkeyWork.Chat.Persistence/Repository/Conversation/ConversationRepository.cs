// ------------------------------------------------------
// <copyright file="ConversationRepository.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Persistence.Common;
using DonkeyWork.Chat.Persistence.Entity.Conversation;
using DonkeyWork.Chat.Persistence.Repository.Conversation.Models;
using Microsoft.EntityFrameworkCore;

namespace DonkeyWork.Chat.Persistence.Repository.Conversation;

/// <summary>
/// The conversation repository.
/// </summary>
public class ConversationRepository(ApiPersistenceContext persistenceContext)
    : IConversationRepository
{
    /// <inheritdoc />
    public async Task<GetConversationsResponse> GetConversationsAsync(
        PagingParameters pagingParameters,
        CancellationToken cancellationToken = default)
    {
        var conversationQuery = persistenceContext.Conversations.AsQueryable();
        if (pagingParameters.Offset > 0)
        {
            conversationQuery = conversationQuery.Skip(pagingParameters.Offset);
        }

        var count = await conversationQuery.CountAsync(cancellationToken);
        var conversations = await conversationQuery
            .OrderBy(c => c.UpdatedAt)
            .Take(pagingParameters.Limit)
            .Select(c => new ConversationsItem
            {
                Id = c.Id,
                Title = c.Title,
                LastMessage = c.MessageEntities
                    .OrderByDescending(m => m.CreatedAt)
                    .Select(m => m.Message)
                    .FirstOrDefault() ?? string.Empty,
                MessageCount = c.MessageEntities.Count,
                UpdatedAt = c.UpdatedAt,
                CreatedAt = c.CreatedAt,
            })
            .ToListAsync(cancellationToken);

        return new GetConversationsResponse
        {
            TotalCount = count,
            Conversations = conversations,
        };
    }

    /// <inheritdoc />
    public async Task<ConversationItem?> GetConversationAsync(Guid conversationId, CancellationToken cancellationToken = default)
    {
        return await persistenceContext.Conversations
            .Where(c => c.Id == conversationId)
            .Select(c => new ConversationItem
            {
                Id = c.Id,
                Title = c.Title,
                Messages = c.MessageEntities
                    .OrderBy(m => m.CreatedAt)
                    .Select(m => new ConversationMessageItem
                    {
                        Id = m.MessagePairId,
                        Owner = m.MessageOwner,
                        Content = m.Message,
                        Timestamp = m.UpdatedAt,
                        MessagePairId = m.MessagePairId,
                    })
                    .ToList(),
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteConversationAsync(Guid conversationId, CancellationToken cancellationToken = default)
    {
        var conversation = await persistenceContext.Conversations.FirstOrDefaultAsync(x => x.Id == conversationId, cancellationToken);
        if (conversation == null)
        {
            return false;
        }

        persistenceContext.Conversations.Remove(conversation);
        return await persistenceContext.SaveChangesAsync(cancellationToken) > 0;
    }

    /// <inheritdoc />
    public async Task<bool> UpdateConversationTitleAsync(Guid conversationId, string title, CancellationToken cancellationToken = default)
    {
        var conversation = await persistenceContext.Conversations.FirstOrDefaultAsync(x => x.Id == conversationId, cancellationToken);
        if (conversation == null)
        {
            return false;
        }

        conversation.Title = title;
        return await persistenceContext.SaveChangesAsync(cancellationToken) > 0;
    }

    /// <inheritdoc />
    public async Task AddConversationAsync(Guid id, string userContent, CancellationToken cancellationToken = default)
    {
        persistenceContext.Conversations.Add(new ConversationEntity()
        {
            Id = id,
            Title = userContent.Length > 64 ? userContent.Substring(0, 64) : userContent,
        });

        await persistenceContext.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddMessageToConversationAsync(
        Guid conversationId,
        Guid executionId,
        MessageOwner messageOwner,
        string content,
        CancellationToken cancellationToken = default)
    {
        persistenceContext.ConversationMessages.Add(
            new ConversationMessageEntity()
            {
                ConversationId = conversationId,
                MessageOwner = messageOwner,
                MessagePairId = executionId,
                Message = content,
            });

        await persistenceContext.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddToolCallToConversationAsync(
        Guid conversationId,
        Guid executionId,
        string request,
        string response,
        string toolname,
        CancellationToken cancellationToken = default)
    {
        persistenceContext.ToolCalls.Add(
            new ()
            {
                ConversationId = conversationId,
                MessagePairId = executionId,
                Request = request,
                Response = response,
                ToolName = toolname,
            });

        await persistenceContext.SaveChangesAsync(cancellationToken);
    }
}