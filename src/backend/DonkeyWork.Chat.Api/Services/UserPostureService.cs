// ------------------------------------------------------
// <copyright file="UserPostureService.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Contracts;
using DonkeyWork.Chat.Common.Providers;
using DonkeyWork.Chat.Persistence.Repository.Integration;

namespace DonkeyWork.Chat.Api.Services;

/// <inheritdoc />
public class UserPostureService(IIntegrationRepository integrationRepository)
    : IUserPostureService
{
    /// <inheritdoc />
    public async Task<List<UserProviderPosture>> GetUserPosturesAsync(CancellationToken cancellationToken = default)
    {
        var userIntegrations = await integrationRepository.GetUserOAuthTokensAsync(cancellationToken);
        return userIntegrations.Select(x =>
            new UserProviderPosture()
            {
                ProviderType = x.Provider,
                Scopes = x.Scopes,
                Keys = x.Metadata,
            }).ToList();
    }

    /// <inheritdoc />
    public async Task<UserProviderPosture?> GetUserPostureAsync(UserProviderType providerType, CancellationToken cancellationToken = default)
    {
        var userIntegrations = await integrationRepository.GetUserOAuthTokenAsync(providerType, cancellationToken);
        return userIntegrations == null
            ? null
            : new UserProviderPosture()
            {
                ProviderType = userIntegrations.Provider,
                Scopes = userIntegrations.Scopes,
                Keys = userIntegrations.Metadata,
            };
    }
}