// ------------------------------------------------------
// <copyright file="IntegrationRepository.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Providers;
using DonkeyWork.Chat.Persistence.Entity.Provider;
using DonkeyWork.Chat.Persistence.Repository.Integration.Models;
using Microsoft.EntityFrameworkCore;

namespace DonkeyWork.Chat.Persistence.Repository.Integration;

/// <inheritdoc />
public class IntegrationRepository(ApiPersistenceContext persistenceContext)
    : IIntegrationRepository
{
    /// <inheritdoc />
    public async Task<List<UserIntegrationItem>> GetUserIntegrationsAsync(CancellationToken cancellationToken = default)
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
        var userTokens = await persistenceContext.UserTokens.AsNoTracking().ToListAsync();
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
    public async Task DeleteIntegrationAsync(UserProviderType type, CancellationToken cancellationToken = default)
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
}