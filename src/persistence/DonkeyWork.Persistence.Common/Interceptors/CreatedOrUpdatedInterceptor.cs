// ------------------------------------------------------
// <copyright file="CreatedOrUpdatedInterceptor.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Services.UserContext;
using DonkeyWork.Persistence.Common.Entity.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace DonkeyWork.Persistence.Common.Interceptors;

/// <inheritdoc />
public class CreatedOrUpdatedInterceptor(IUserContextProvider userContextProvider)
    : SaveChangesInterceptor
{
    /// <inheritdoc />
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        if (eventData.Context is null)
        {
            return result;
        }

        this.UpdateEntityCreatedOrUpdated(eventData);

        return result;
    }

    /// <inheritdoc />
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is null)
        {
            return new ValueTask<InterceptionResult<int>>(result);
        }

        this.UpdateEntityCreatedOrUpdated(eventData);

        return new ValueTask<InterceptionResult<int>>(result);
    }

    private void UpdateEntityCreatedOrUpdated(DbContextEventData eventData)
    {
        // ReSharper disable once NullableWarningSuppressionIsUsed // Reason: false positive
        foreach (var entry in eventData.Context!
                     .ChangeTracker.Entries().Where(
                         entry => entry is
                         {
                             State: EntityState.Modified or EntityState.Added,
                             Entity: BaseEntity
                         }))
        {
            BaseEntity entity = (BaseEntity)entry.Entity;

            entity.UpdatedAt = DateTimeOffset.UtcNow;
            if (entry.State is EntityState.Added)
            {
                if (entity is BaseUserEntity baseUserEntity)
                {
                    baseUserEntity.UserId = userContextProvider.UserId;
                }
            }
        }
    }
}