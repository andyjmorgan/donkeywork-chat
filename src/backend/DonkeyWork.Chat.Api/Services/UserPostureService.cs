// ------------------------------------------------------
// <copyright file="UserPostureService.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Contracts;
using DonkeyWork.Chat.Common.Providers;
using DonkeyWork.Chat.Common.Providers.GenericProvider;
using DonkeyWork.Chat.Persistence.Repository.Integration;

namespace DonkeyWork.Chat.Api.Services;

/// <inheritdoc />
public class UserPostureService(IIntegrationRepository integrationRepository)
    : IUserPostureService
{
    /// <inheritdoc />
    public async Task<ToolProviderPosture> GetUserPosturesAsync(CancellationToken cancellationToken = default)
    {
        var userIntegrations = await integrationRepository.GetUserOAuthTokensAsync(cancellationToken);
        var genericIntegrations = await integrationRepository.GetGenericIntegrationsAsync(cancellationToken);
        return new ToolProviderPosture()
        {
            UserTokens = userIntegrations.Select(x => new UserProviderPosture()
            {
                ProviderType = x.Provider,
                Scopes = x.Scopes,
                Keys = x.Metadata,
            }).ToList(),
            GenericIntegrations = genericIntegrations
                .Where(x => x.Configuration != null)
                .Select(x => new GenericProviderPosture()
                {
                    ProviderType = x.ProviderType,
                    Configuration = BaseGenericProviderConfiguration.FromJson(x.Configuration!),
                }).ToList(),
        };
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

    /// <inheritdoc />
    public async Task<GenericProviderPosture?> GetUserGenericPostureAsync(GenericProviderType providerType, CancellationToken cancellationToken = default)
    {
        var userIntegration = await integrationRepository.GetGenericIntegrationAsync(providerType, cancellationToken);
        return userIntegration == null
            ? null
            : new GenericProviderPosture()
            {
                ProviderType = userIntegration.ProviderType,
                Configuration = BaseGenericProviderConfiguration.FromJson(userIntegration.Configuration),
            };
    }
}