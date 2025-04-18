// ------------------------------------------------------
// <copyright file="IntegrationRepository.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Providers;
using DonkeyWork.Chat.Common.Providers.GenericProvider;
using DonkeyWork.Chat.Persistence.Entity.Provider;
using DonkeyWork.Chat.Persistence.Repository.Integration.Models;
using Microsoft.EntityFrameworkCore;

namespace DonkeyWork.Chat.Persistence.Repository.Integration;

/// <inheritdoc />
public class IntegrationRepository(ApiPersistenceContext persistenceContext)
    : IIntegrationRepository
{
    /// <inheritdoc />
    public async Task<List<UserIntegrationItem>> GetUserOAuthIntegrationsAsync(CancellationToken cancellationToken = default)
    {
        var userTokens = await persistenceContext.UserTokens
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        return userTokens.Select(x => new UserIntegrationItem()
        {
            Provider = x.ProviderType,
            Scopes = x.Scopes,
        }).ToList();
    }

    /// <inheritdoc />
    public async Task<List<UserOAuthTokenItem>> GetUserOAuthTokensAsync(CancellationToken cancellationToken = default)
    {
        var userTokens = await persistenceContext.UserTokens
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        return userTokens.Select(x => new UserOAuthTokenItem()
        {
            Provider = x.ProviderType,
            Scopes = x.Scopes,
            Metadata = x.Data,
            Expiration = x.ExpiresAt,
        }).ToList();
    }

    /// <inheritdoc />
    public async Task<UserOAuthTokenItem?> GetUserOAuthTokenAsync(UserProviderType providerType, CancellationToken cancellationToken = default)
    {
        var userToken = await persistenceContext.UserTokens
            .AsNoTracking()
            .Where(x => x.ProviderType == providerType)
            .FirstOrDefaultAsync(cancellationToken);

        return userToken == null
            ? null
            : new UserOAuthTokenItem
            {
                Provider = userToken.ProviderType,
                Scopes = userToken.Scopes,
                Metadata = userToken.Data,
                Expiration = userToken.ExpiresAt,
            };
    }

    /// <inheritdoc />
    public async Task DeleteOauthIntegrationAsync(UserProviderType type, CancellationToken cancellationToken = default)
    {
        persistenceContext.UserTokens.RemoveRange(
            persistenceContext.UserTokens
            .Where(x => x.ProviderType == type));
        await persistenceContext.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddOrUpdateUserOAuthTokenAsync(UserOAuthTokenItem oauthToken, CancellationToken cancellationToken = default)
    {
        var existingToken = await persistenceContext.UserTokens.Where(x => x.ProviderType == oauthToken.Provider).FirstOrDefaultAsync(cancellationToken);
        if (existingToken != null)
        {
            existingToken.ExpiresAt = oauthToken.Expiration;
            existingToken.Data = oauthToken.Metadata;
            existingToken.Scopes = oauthToken.Scopes;
            await persistenceContext.SaveChangesAsync(cancellationToken);
            return;
        }

        var userToken = new UserTokenEntity()
        {
            ProviderType = oauthToken.Provider,
            ExpiresAt = oauthToken.Expiration,
            Data = oauthToken.Metadata,
            Scopes = oauthToken.Scopes,
        };
        persistenceContext.UserTokens.Add(userToken);
        await persistenceContext.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<GenericIntegrationItem>> GetGenericIntegrationsAsync(CancellationToken cancellationToken = default)
    {
        var integrations = await persistenceContext.GenericProviders.ToListAsync(cancellationToken);
        return integrations.Select(
            x => new GenericIntegrationItem()
        {
            ProviderType = x.ProviderType,
            IsEnabled = x.IsEnabled,
            Configuration = x.Configuration,
        }).ToList();
    }

    /// <inheritdoc />
    public async Task<GenericIntegrationItem?> GetGenericIntegrationAsync(GenericProviderType providerType, CancellationToken cancellationToken = default)
    {
        var integration = await persistenceContext.GenericProviders.Where(x => x.ProviderType == providerType).FirstOrDefaultAsync(cancellationToken);
        return integration == null ? null : new GenericIntegrationItem()
        {
            ProviderType = integration.ProviderType,
            IsEnabled = integration.IsEnabled,
            Configuration = integration.Configuration,
        };
    }

    /// <inheritdoc />
    public async Task UpsertGenericIntegrationAsync(
        GenericIntegrationItem genericIntegrationItem,
        CancellationToken cancellationToken = default)
    {
        var existingIntegration = await persistenceContext.GenericProviders
            .Where(x => x.ProviderType == genericIntegrationItem.ProviderType)
            .FirstOrDefaultAsync(cancellationToken);

        if (existingIntegration != null)
        {
            existingIntegration.IsEnabled = genericIntegrationItem.IsEnabled;
            if (!string.IsNullOrWhiteSpace(genericIntegrationItem.Configuration))
            {
                existingIntegration.Configuration = genericIntegrationItem.Configuration;
            }

            persistenceContext.GenericProviders.Update(existingIntegration);
        }
        else
        {
            var newIntegration = new GenericProviderEntity()
            {
                ProviderType = genericIntegrationItem.ProviderType,
                IsEnabled = genericIntegrationItem.IsEnabled,
                Configuration = genericIntegrationItem.Configuration,
            };
            persistenceContext.GenericProviders.Add(newIntegration);
        }

        await persistenceContext.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task DeleteGenericIntegrationAsync(GenericProviderType providerType, CancellationToken cancellationToken = default)
    {
        await persistenceContext.GenericProviders.Where(x => x.ProviderType == providerType).ExecuteDeleteAsync(cancellationToken);
    }
}