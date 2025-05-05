// ------------------------------------------------------
// <copyright file="ActionRespository.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Persistence.Agent.Entity.Action;
using DonkeyWork.Persistence.Agent.Repository.Action.Models;
using DonkeyWork.Persistence.Agent.Repository.Action.Models.Execution;
using DonkeyWork.Persistence.Common.Common;
using Microsoft.EntityFrameworkCore;

namespace DonkeyWork.Persistence.Agent.Repository.Action;

/// <inheritdoc />
public class ActionRepository(AgentPersistenceContext persistenceContext)
    : IActionRepository
{
    /// <inheritdoc />
    public async Task<GetActionsResponseItem> GetActionsAsync(
        PagingParameters pagingParameters,
        CancellationToken cancellationToken = default)
    {
        var actionQuery = persistenceContext.Actions
            .AsNoTracking()
            .AsQueryable();

        if (pagingParameters.Offset > 0)
        {
            actionQuery = actionQuery.Skip(pagingParameters.Offset);
        }

        var count = await actionQuery.CountAsync(cancellationToken);
        var actions = await actionQuery
            .OrderBy(a => a.Name)
            .Take(pagingParameters.Limit)
            .Select(a => new ActionItem
            {
                Id = a.Id,
                Name = a.Name,
                Description = a.Description,
                Icon = a.Icon,
                ModelConfiguration = a.ModelConfiguration,
                SystemPromptIds = a.SystemPrompts.Select(sp => sp.SystemPromptId).ToList(),
                UserPromptIds = a.ActionPrompts.Select(up => up.ActionPromptId).ToList(),
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt,
                AllowedTools = a.AllowedTools,
            })
            .AsSplitQuery()
            .ToListAsync(cancellationToken);

        return new GetActionsResponseItem
        {
            TotalCount = count,
            Actions = actions,
        };
    }

    /// <inheritdoc />
    public async Task<ActionItem?> GetActionByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var action = await persistenceContext.Actions
            .AsNoTracking()
            .Include(a => a.SystemPrompts)
            .Include(a => a.ActionPrompts)
            .SingleOrDefaultAsync(a => a.Name == name, cancellationToken);

        if (action == null)
        {
            return null;
        }

        return new ActionItem
        {
            Id = action.Id,
            Name = action.Name,
            Description = action.Description,
            Icon = action.Icon,
            ModelConfiguration = action.ModelConfiguration,
            SystemPromptIds = action.SystemPrompts.Select(sp => sp.SystemPromptId).ToList(),
            UserPromptIds = action.ActionPrompts.Select(up => up.ActionPromptId).ToList(),
            CreatedAt = action.CreatedAt,
            UpdatedAt = action.UpdatedAt,
            AllowedTools = action.AllowedTools,
        };
    }

    /// <inheritdoc />
    public async Task<ActionItem?> GetActionByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var action = await persistenceContext.Actions
            .AsNoTracking()
            .Include(a => a.SystemPrompts)
            .Include(a => a.ActionPrompts)
            .SingleOrDefaultAsync(a => a.Id == id, cancellationToken);

        if (action == null)
        {
            return null;
        }

        return new ActionItem
        {
            Id = action.Id,
            Name = action.Name,
            Description = action.Description,
            Icon = action.Icon,
            ModelConfiguration = action.ModelConfiguration,
            SystemPromptIds = action.SystemPrompts.Select(sp => sp.SystemPromptId).ToList(),
            UserPromptIds = action.ActionPrompts.Select(up => up.ActionPromptId).ToList(),
            CreatedAt = action.CreatedAt,
            UpdatedAt = action.UpdatedAt,
            AllowedTools = action.AllowedTools,
        };
    }

    /// <inheritdoc />
    public async Task<ActionExecutionItem?> GetActionByIdForExecutionAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var action = await persistenceContext.Actions
            .AsNoTracking()
            .Include(a => a.SystemPrompts)
            .ThenInclude(s => s.SystemPrompt)
            .Include(a => a.ActionPrompts)
            .ThenInclude(a => a.ActionPrompt)
            .SingleOrDefaultAsync(a => a.Id == id, cancellationToken);

        if (action == null)
        {
            return null;
        }

        var actionExecutionItem = new ActionExecutionItem
        {
            AllowedTools = action.AllowedTools,
            ModelConfiguration = action.ModelConfiguration,
            ActionId = action.Id,
            Name = action.Name,
        };

        foreach (var item in action.SystemPrompts)
        {
            if (item.SystemPrompt == null)
            {
                throw new InvalidDataException("System prompt is null");
            }

            actionExecutionItem.SystemPrompts.Add(new SystemPromptExecutionItem()
            {
                Name = item.SystemPrompt.Name,
                Id = item.SystemPrompt.Id,
                Content = item.SystemPrompt.Content,
            });
        }

        foreach (var item in action.ActionPrompts)
        {
            if (item.ActionPrompt == null)
            {
                throw new InvalidDataException("System prompt is null");
            }

            actionExecutionItem.ActionItems.Add(new ActionPromptExecutionItem()
            {
                Name = item.ActionPrompt.Name,
                Id = item.ActionPrompt.Id,
                Variables = item.ActionPrompt.Variables,
                Messages = item.ActionPrompt.Messages,
            });
        }

        return actionExecutionItem;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteActionAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var action = await persistenceContext.Actions
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        if (action == null)
        {
            return false;
        }

        persistenceContext.Actions.Remove(action);
        return await persistenceContext.SaveChangesAsync(cancellationToken) > 0;
    }

    /// <inheritdoc />
    public async Task<bool> UpdateActionAsync(
        Guid id,
        UpsertActionItem actionItem,
        CancellationToken cancellationToken = default)
    {
        using var transaction = await persistenceContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var action = await persistenceContext.Actions
                .Include(a => a.SystemPrompts)
                .Include(a => a.ActionPrompts)
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

            if (action == null)
            {
                return false;
            }

            // Update basic properties
            action.Name = actionItem.Name;
            action.Description = actionItem.Description;
            action.Icon = actionItem.Icon;
            action.AllowedTools = actionItem.AllowedTools;
            action.ModelConfiguration = actionItem.ModelConfiguration;

            // Update system prompt relations
            var currentSystemPromptIds = action.SystemPrompts.Select(sp => sp.SystemPromptId).ToList();
            var systemPromptsToRemove = action.SystemPrompts
                .Where(sp => !actionItem.SystemPromptIds.Contains(sp.SystemPromptId))
                .ToList();
            var systemPromptIdsToAdd = actionItem.SystemPromptIds
                .Except(currentSystemPromptIds)
                .ToList();

            // Remove relations that are no longer needed
            foreach (var relation in systemPromptsToRemove)
            {
                persistenceContext.ActionSystemPromptRelations.Remove(relation);
            }

            // Add new relations
            foreach (var promptId in systemPromptIdsToAdd)
            {
                persistenceContext.ActionSystemPromptRelations.Add(new ActionSystemPromptRelationEntity
                {
                    ActionId = action.Id,
                    SystemPromptId = promptId,
                });
            }

            // Update user prompt relations
            var currentUserPromptIds = action.ActionPrompts.Select(up => up.ActionPromptId).ToList();
            var userPromptsToRemove = action.ActionPrompts
                .Where(up => !actionItem.UserPromptIds.Contains(up.ActionPromptId))
                .ToList();
            var userPromptIdsToAdd = actionItem.UserPromptIds
                .Except(currentUserPromptIds)
                .ToList();

            // Remove relations that are no longer needed
            foreach (var relation in userPromptsToRemove)
            {
                persistenceContext.ActionPromptRelations.Remove(relation);
            }

            // Add new relations
            foreach (var promptId in userPromptIdsToAdd)
            {
                persistenceContext.ActionPromptRelations.Add(new ActionPromptRelationEntity
                {
                    ActionId = action.Id,
                    ActionPromptId = promptId,
                });
            }

            await persistenceContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return true;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<bool> AddActionAsync(UpsertActionItem actionItem, CancellationToken cancellationToken = default)
    {
        using var transaction = await persistenceContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            // Create the action entity
            var action = new ActionEntity
            {
                Name = actionItem.Name,
                Description = actionItem.Description,
                Icon = actionItem.Icon,
                ModelConfiguration = actionItem.ModelConfiguration,
                AllowedTools = actionItem.AllowedTools,
            };

            // Add the action to the context
            persistenceContext.Actions.Add(action);
            await persistenceContext.SaveChangesAsync(cancellationToken);

            // Add system prompt relations
            foreach (var promptId in actionItem.SystemPromptIds)
            {
                persistenceContext.ActionSystemPromptRelations.Add(new ActionSystemPromptRelationEntity
                {
                    ActionId = action.Id,
                    SystemPromptId = promptId,
                });
            }

            // Add user prompt relations
            foreach (var promptId in actionItem.UserPromptIds)
            {
                persistenceContext.ActionPromptRelations.Add(new ActionPromptRelationEntity
                {
                    ActionId = action.Id,
                    ActionPromptId = promptId,
                });
            }

            await persistenceContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return true;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
