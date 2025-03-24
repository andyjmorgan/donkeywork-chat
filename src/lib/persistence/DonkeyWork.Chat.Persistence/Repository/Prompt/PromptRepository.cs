// ------------------------------------------------------
// <copyright file="PromptRepository.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Persistence.Common;
using DonkeyWork.Chat.Persistence.Entity.Prompt;
using DonkeyWork.Chat.Persistence.Repository.Prompt.Models;
using Microsoft.EntityFrameworkCore;

namespace DonkeyWork.Chat.Persistence.Repository.Prompt;

/// <inheritdoc />
public class PromptRepository(ApiPersistenceContext persistenceContext)
    : IPromptRepository
{
    /// <inheritdoc />
    public async Task<GetPromptsResponseItem> GetPromptsAsync(PagingParameters pagingParameters, CancellationToken cancellationToken = default)
    {
        var promptQuery = persistenceContext.Prompts.AsQueryable();
        if (pagingParameters.Offset > 0)
        {
            promptQuery = promptQuery.Skip(pagingParameters.Offset);
        }

        var count = await promptQuery.CountAsync(cancellationToken);
        var prompts = await promptQuery
            .OrderBy(c => c.UpdatedAt)
            .Take(pagingParameters.Limit)
            .Select(c => new PromptItem
            {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description,
                Content = c.Content,
                UpdatedAt = c.UpdatedAt,
                CreatedAt = c.CreatedAt,
                UsageCount = c.UsageCount,
            })
            .ToListAsync(cancellationToken);

        return new GetPromptsResponseItem()
        {
            TotalCount = count,
            Prompts = prompts,
        };
    }

    /// <inheritdoc />
    public async Task<PromptContentItem?> GetPromptContentAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var prompt = await persistenceContext.Prompts.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (prompt is null)
        {
            return default;
        }

        await persistenceContext.Prompts
            .Where(p => p.Id == id)
            .ExecuteUpdateAsync(
                p => p.SetProperty(
                    u => u.UsageCount,
                    u => u.UsageCount + 1),
                cancellationToken);

        return new PromptContentItem
        {
            Content = prompt.Content,
        };
    }

    /// <inheritdoc />
    public async Task<bool> DeletePromptAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var prompt = await persistenceContext.Prompts.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (prompt == null)
        {
            return false;
        }

        persistenceContext.Prompts.Remove(prompt);
        return await persistenceContext.SaveChangesAsync(cancellationToken) > 0;
    }

    /// <inheritdoc />
    public async Task<bool> UpdatePromptAsync(Guid id, UpsertPromptItem prompt, CancellationToken cancellationToken = default)
    {
        var existingPrompt = await persistenceContext.Prompts.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (existingPrompt == null)
        {
            return false;
        }

        existingPrompt.Title = prompt.Title;
        existingPrompt.Description = prompt.Description;
        existingPrompt.Content = prompt.Content;
        return await persistenceContext.SaveChangesAsync(cancellationToken) > 0;
    }

    /// <inheritdoc />
    public async Task<bool> AddPromptAsync(UpsertPromptItem prompt, CancellationToken cancellationToken = default)
    {
        persistenceContext.Prompts.Add(
            new PromptEntity()
            {
                Content = prompt.Content,
                Description = prompt.Description,
                Title = prompt.Title,
            });

        return await persistenceContext.SaveChangesAsync(cancellationToken) > 0;
    }

    /// <inheritdoc />
    public async Task IncreasePromptUsageCountAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await persistenceContext.Prompts
            .Where(p => p.Id == id)
            .ExecuteUpdateAsync(
                p =>
                    p.SetProperty(
                        u => u.UsageCount,
                        u => u.UsageCount + 1),
                cancellationToken);
    }
}