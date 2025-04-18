// ------------------------------------------------------
// <copyright file="ApiKeyRepository.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Persistence.Common;
using DonkeyWork.Chat.Persistence.Entity.ApiKey;
using DonkeyWork.Chat.Persistence.Repository.ApiKey.Models;
using Microsoft.EntityFrameworkCore;

namespace DonkeyWork.Chat.Persistence.Repository.ApiKey;

/// <summary>
/// A repository for managing API keys.
/// </summary>
public class ApiKeyRepository(ApiPersistenceContext persistenceContext)
    : IApiKeyRepository
{
    /// <inheritdoc />
    public async Task<ApiKeyItem> CreateApiKeyAsync(ApiKeyItem apiKeyItem, CancellationToken cancellationToken = default)
    {
        var item = persistenceContext.Add(new ApiKeyEntity()
        {
            ApiKey = apiKeyItem.ApiKey,
            Description = apiKeyItem.Description,
            Name = apiKeyItem.Name,
            Enabled = apiKeyItem.IsEnabled,
        });
        await persistenceContext.SaveChangesAsync(cancellationToken);
        await item.ReloadAsync(cancellationToken);
        return new ApiKeyInfoItem()
        {
            ApiKey = item.Entity.ApiKey,
            IsEnabled = item.Entity.Enabled,
            Description = item.Entity.Description,
            Name = item.Entity.Name,
            Id = item.Entity.Id,
            CreatedAt = item.Entity.CreatedAt,
        };
    }

    /// <inheritdoc />
    public async Task<ApiKeyItem?> UpdateApiKeyAsync(
        Guid id,
        ApiKeyItem apiKey,
        CancellationToken cancellationToken = default)
    {
        var existingApiKey = await persistenceContext.ApiKeys
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken: cancellationToken);

        if (existingApiKey == null)
        {
            return null;
        }

        existingApiKey.Description = apiKey.Description ?? existingApiKey.Description;
        existingApiKey.Name = apiKey.Name;
        existingApiKey.Enabled = apiKey.IsEnabled;

        persistenceContext.Update(existingApiKey);
        await persistenceContext.SaveChangesAsync(cancellationToken);
        return new ApiKeyItem()
        {
            ApiKey = existingApiKey.ApiKey,
            Description = existingApiKey.Description,
            Name = existingApiKey.Name,
            IsEnabled = existingApiKey.Enabled,
        };
    }

    /// <inheritdoc />
    public async Task<GetApiKeysResponseItem> GetApiKeysAsync(PagingParameters pagingParameters, CancellationToken cancellationToken = default)
    {
        var query = persistenceContext.ApiKeys.Select(x => new ApiKeyInfoItem()
        {
            Name = x.Name,
            Description = x.Description,
            ApiKey = x.ApiKey,
            IsEnabled = x.Enabled,
            CreatedAt = x.CreatedAt,
            Id = x.Id,
        });

        var count = await query.CountAsync(cancellationToken);
        if (pagingParameters.Offset > 0)
        {
            query = query.Skip(pagingParameters.Offset);
        }

        return new GetApiKeysResponseItem()
        {
            TotalCount = count,
            Items = await query
                .Take(pagingParameters.Limit)
                .Select(x => new ApiKeyInfoItem()
                {
                    ApiKey = x.ApiKey,
                    IsEnabled = x.IsEnabled,
                    CreatedAt = x.CreatedAt,
                    Id = x.Id,
                    Description = x.Description,
                    Name = x.Name,
                })
                .AsNoTracking()
                .ToListAsync(cancellationToken),
        };
    }

    /// <inheritdoc />
    public async Task<ApiKeyItem?> GetApiKeyAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var apikey = await persistenceContext.ApiKeys
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken: cancellationToken);
        return apikey is null
            ? null
            : new ApiKeyItem()
            {
                ApiKey = apikey.ApiKey,
                IsEnabled = apikey.Enabled,
                Description = apikey.Description,
                Name = apikey.Name,
            };
    }

    /// <inheritdoc />
    public async Task DeleteApiKeyAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var apiKey = await persistenceContext.ApiKeys.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (apiKey == null)
        {
            return;
        }

        persistenceContext.Remove(apiKey);
        await persistenceContext.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Guid?> GetUserIdByApiKeyAsync(string apiKey, CancellationToken cancellationToken = default)
    {
        var key = await persistenceContext.ApiKeys
            .IgnoreQueryFilters()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.ApiKey == apiKey, cancellationToken);
        return key?.UserId;
    }
}