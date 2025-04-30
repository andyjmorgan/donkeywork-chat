// ------------------------------------------------------
// <copyright file="ActionExecutionRepository.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Models.Actions;
using DonkeyWork.Chat.Common.Models.Streaming;
using DonkeyWork.Persistence.Agent.Entity.ActionExecution;
using DonkeyWork.Persistence.Agent.Repository.ActionExecution.Models;
using DonkeyWork.Persistence.Common.Common;
using Microsoft.EntityFrameworkCore;

namespace DonkeyWork.Persistence.Agent.Repository.ActionExecution;

/// <inheritdoc />
public class ActionExecutionRepository(AgentPersistenceContext persistenceContext)
    : IActionExecutionRepository
{
    /// <inheritdoc />
    public async Task<Guid> CreateTaskAsync(Guid executionId, Guid actionId, string actionName, CancellationToken cancellationToken = default)
    {
        var actionExecution = persistenceContext.ActionExecutions.Add(new ActionExecutionEntity()
        {
            ActionId = actionId,
            ExecutionId = executionId,
            ActionName = actionName,
            Status = ActionExecutionStatus.Pending,
        });
        await persistenceContext.SaveChangesAsync(cancellationToken);
        return actionExecution.Entity.Id;
    }

    /// <inheritdoc />
    public async Task SetTaskRunningAsync(Guid executionId, CancellationToken cancellationToken = default)
    {
        await persistenceContext.ActionExecutions.Where(x => x.Id == executionId)
            .ExecuteUpdateAsync(
                update =>
                    update.SetProperty(x => x.Status, ActionExecutionStatus.InProgress),
                cancellationToken);
    }

    /// <inheritdoc />
    public async Task SetTaskCompletedStatusAsync(Guid executionId, List<BaseStreamItem> results, ActionExecutionStatus status, CancellationToken cancellationToken = default)
    {
        var existingExecution = await persistenceContext.ActionExecutions
            .FirstOrDefaultAsync(x => x.Id == executionId, cancellationToken);

        if (existingExecution is null)
        {
            return;
        }

        existingExecution.Status = status;
        existingExecution.EndTime = DateTimeOffset.UtcNow;
        existingExecution.Data = results;
        persistenceContext.ActionExecutions.Update(existingExecution);
        await persistenceContext.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<GetActionExecutionsItem> GetActionExecutionsAsync(PagingParameters pagingParameters, CancellationToken cancellationToken = default)
    {
        var actionQuery = persistenceContext.ActionExecutions
            .AsNoTracking()
            .AsQueryable();

        if (pagingParameters.Offset > 0)
        {
            actionQuery = actionQuery.Skip(pagingParameters.Offset);
        }

        var count = await actionQuery.CountAsync(cancellationToken);
        var actions = await actionQuery
            .OrderByDescending(a => a.CreatedAt) // Changed to descending order (newest first)
            .Take(pagingParameters.Limit)
            .AsNoTracking()
            .Select(a => new ActionExecutionItem()
            {
                Id = a.Id,
                ActionName = a.ActionName,
                ActionId = a.ActionId,
                CreatedAt = a.CreatedAt,
                EndTime = a.EndTime,
                ExecutionId = a.ExecutionId,
                ExecutionStatus = a.Status,
            })
            .AsSplitQuery()
            .ToListAsync(cancellationToken);

        return new GetActionExecutionsItem()
        {
            TotalCount = count,
            Actions = actions,
        };
    }

    /// <inheritdoc />
    public async Task<GetActionExecutionsItem> GetActionExecutionsByActionIdAsync(Guid actionId, PagingParameters pagingParameters, CancellationToken cancellationToken = default)
    {
        var actionQuery = persistenceContext.ActionExecutions
            .AsNoTracking()
            .Where(a => a.ActionId == actionId)
            .AsQueryable();

        if (pagingParameters.Offset > 0)
        {
            actionQuery = actionQuery.Skip(pagingParameters.Offset);
        }

        var count = await actionQuery.CountAsync(cancellationToken);
        var actions = await actionQuery
            .OrderByDescending(a => a.CreatedAt) // Newest first
            .Take(pagingParameters.Limit)
            .AsNoTracking()
            .Select(a => new ActionExecutionItem()
            {
                Id = a.Id,
                ActionName = a.ActionName,
                ActionId = a.ActionId,
                CreatedAt = a.CreatedAt,
                EndTime = a.EndTime,
                ExecutionId = a.ExecutionId,
                ExecutionStatus = a.Status,
            })
            .AsSplitQuery()
            .ToListAsync(cancellationToken);

        return new GetActionExecutionsItem()
        {
            TotalCount = count,
            Actions = actions,
        };
    }
}