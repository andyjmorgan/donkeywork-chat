// ------------------------------------------------------
// <copyright file="PromptRepository.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Persistence.Agent.Entity.Prompt;
using DonkeyWork.Persistence.Agent.Repository.Prompt.Models.ActionPrompt;
using DonkeyWork.Persistence.Agent.Repository.Prompt.Models.SystemPrompt;
using DonkeyWork.Persistence.Common.Common;
using Microsoft.EntityFrameworkCore;

namespace DonkeyWork.Persistence.Agent.Repository.Prompt;

/// <inheritdoc />
public class PromptRepository(AgentPersistenceContext persistenceContext)
    : IPromptRepository
{
    /// <inheritdoc />
    public async Task<GetPromptsResponseItem> GetPromptsAsync(PagingParameters pagingParameters, CancellationToken cancellationToken = default)
    {
        var promptQuery = persistenceContext.SystemPrompts
            .AsNoTracking()
            .AsQueryable();
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
                Name = c.Name,
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
    public async Task<GetActionPromptsResponseItem> GetActionPromptsAsync(PagingParameters pagingParameters, CancellationToken cancellationToken = default)
    {
        var promptQuery = persistenceContext.ActionPrompts
            .AsNoTracking()
            .AsQueryable();
        if (pagingParameters.Offset > 0)
        {
            promptQuery = promptQuery.Skip(pagingParameters.Offset);
        }

        var count = await promptQuery.CountAsync(cancellationToken);
        var prompts = await promptQuery
            .OrderBy(c => c.UpdatedAt)
            .Take(pagingParameters.Limit)
            .Select(c => new ActionPromptItem
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                Variables = c.Variables,
                Messages = c.Messages,
                UpdatedAt = c.UpdatedAt,
                CreatedAt = c.CreatedAt,
                UsageCount = c.UsageCount,
            })
            .ToListAsync(cancellationToken);

        return new GetActionPromptsResponseItem()
        {
            TotalCount = count,
            Prompts = prompts,
        };
    }

    /// <inheritdoc />
    public async Task<PromptContentItem?> GetPromptContentItemAsync(string title, CancellationToken cancellationToken = default)
    {
        var prompt = await persistenceContext.SystemPrompts
            .AsNoTracking()
            .SingleOrDefaultAsync(c => c.Name == title, cancellationToken);

        if (prompt == null)
        {
            return null;
        }

        return new PromptContentItem()
        {
            Description = prompt.Description,
            Name = prompt.Name,
            Content = prompt.Content,
        };
    }

    /// <inheritdoc />
    public async Task<ActionPromptItem?> GetActionPromptContentItemAsync(string title, CancellationToken cancellationToken = default)
    {
        var prompt = await persistenceContext.ActionPrompts
            .AsNoTracking()
            .SingleOrDefaultAsync(c => c.Name == title, cancellationToken);

        if (prompt == null)
        {
            return null;
        }

        return new ActionPromptItem()
        {
            Id = prompt.Id,
            Description = prompt.Description,
            Name = prompt.Name,
            Variables = prompt.Variables,
            Messages = prompt.Messages,
            CreatedAt = prompt.CreatedAt,
            UpdatedAt = prompt.UpdatedAt,
            UsageCount = prompt.UsageCount,
        };
    }

    /// <inheritdoc />
    public async Task<PromptContentItem?> GetPromptContentAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var prompt = await persistenceContext.SystemPrompts.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (prompt is null)
        {
            return default;
        }

        await persistenceContext.SystemPrompts
            .Where(p => p.Id == id)
            .ExecuteUpdateAsync(
                p => p.SetProperty(
                    u => u.UsageCount,
                    u => u.UsageCount + 1),
                cancellationToken);

        return new PromptContentItem
        {
            Name = prompt.Name,
            Description = prompt.Description,
            Content = prompt.Content,
        };
    }

    /// <inheritdoc />
    public async Task<ActionPromptItem?> GetActionPromptContentAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var prompt = await persistenceContext.ActionPrompts.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (prompt is null)
        {
            return default;
        }

        await persistenceContext.ActionPrompts
            .Where(p => p.Id == id)
            .ExecuteUpdateAsync(
                p => p.SetProperty(
                    u => u.UsageCount,
                    u => u.UsageCount + 1),
                cancellationToken);

        return new ActionPromptItem
        {
            Id = prompt.Id,
            Name = prompt.Name,
            Description = prompt.Description,
            Variables = prompt.Variables,
            Messages = prompt.Messages,
            CreatedAt = prompt.CreatedAt,
            UpdatedAt = prompt.UpdatedAt,
            UsageCount = prompt.UsageCount,
        };
    }

    /// <inheritdoc />
    public async Task<bool> DeletePromptAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var prompt = await persistenceContext.SystemPrompts.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (prompt == null)
        {
            return false;
        }

        persistenceContext.SystemPrompts.Remove(prompt);
        return await persistenceContext.SaveChangesAsync(cancellationToken) > 0;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteActionPromptAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var prompt = await persistenceContext.ActionPrompts.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (prompt == null)
        {
            return false;
        }

        persistenceContext.ActionPrompts.Remove(prompt);
        return await persistenceContext.SaveChangesAsync(cancellationToken) > 0;
    }

    /// <inheritdoc />
    public async Task<bool> UpdatePromptAsync(Guid id, UpsertPromptItem prompt, CancellationToken cancellationToken = default)
    {
        var existingPrompt = await persistenceContext.SystemPrompts.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (existingPrompt == null)
        {
            return false;
        }

        existingPrompt.Name = prompt.Name;
        existingPrompt.Description = prompt.Description;
        existingPrompt.Content = prompt.Content;
        return await persistenceContext.SaveChangesAsync(cancellationToken) > 0;
    }

    /// <inheritdoc />
    public async Task<bool> UpdateActionPromptAsync(Guid id, UpsertActionPromptItem prompt, CancellationToken cancellationToken = default)
    {
        var existingPrompt = await persistenceContext.ActionPrompts.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (existingPrompt == null)
        {
            return false;
        }

        existingPrompt.Name = prompt.Name;
        existingPrompt.Description = prompt.Description;
        existingPrompt.Variables = prompt.Variables;
        existingPrompt.Messages = prompt.Messages;
        return await persistenceContext.SaveChangesAsync(cancellationToken) > 0;
    }

    /// <inheritdoc />
    public async Task<bool> AddPromptAsync(UpsertPromptItem prompt, CancellationToken cancellationToken = default)
    {
        persistenceContext.SystemPrompts.Add(
            new SystemPromptEntity()
            {
                Content = prompt.Content,
                Description = prompt.Description,
                Name = prompt.Name,
            });

        return await persistenceContext.SaveChangesAsync(cancellationToken) > 0;
    }

    /// <inheritdoc />
    public async Task<bool> AddActionPromptAsync(UpsertActionPromptItem prompt, CancellationToken cancellationToken = default)
    {
        persistenceContext.ActionPrompts.Add(
            new ActionPromptEntity()
            {
                Description = prompt.Description,
                Name = prompt.Name,
                Variables = prompt.Variables,
                Messages = prompt.Messages,
            });

        return await persistenceContext.SaveChangesAsync(cancellationToken) > 0;
    }

    /// <inheritdoc />
    public async Task IncreasePromptUsageCountAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await persistenceContext.SystemPrompts
            .Where(p => p.Id == id)
            .ExecuteUpdateAsync(
                p =>
                    p.SetProperty(
                        u => u.UsageCount,
                        u => u.UsageCount + 1),
                cancellationToken);
    }

    /// <inheritdoc />
    public async Task IncreaseActionPromptUsageCountAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await persistenceContext.ActionPrompts
            .Where(p => p.Id == id)
            .ExecuteUpdateAsync(
                p =>
                    p.SetProperty(
                        u => u.UsageCount,
                        u => u.UsageCount + 1),
                cancellationToken);
    }
}
