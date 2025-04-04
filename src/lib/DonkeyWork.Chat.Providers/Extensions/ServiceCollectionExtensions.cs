// ------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Providers;
using DonkeyWork.Chat.Providers.Provider;
using DonkeyWork.Chat.Providers.Provider.Configuration;
using DonkeyWork.Chat.Providers.Provider.Implementation;
using DonkeyWork.Chat.Providers.Provider.Implementation.Microsoft;
using DonkeyWork.Chat.Providers.Provider.Implementation.Microsoft.Client;
using DonkeyWork.Chat.Providers.TokenManager;
using DonkeyWork.Chat.Providers.TokenManager.Implementation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DonkeyWork.Chat.Providers.Extensions;

/// <summary>
/// An extension class for the <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the credentials context persistence.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>A <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddProviderConfiguration(
        this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        serviceCollection.AddOptions<MicrosoftOAuthConfiguration>()
            .BindConfiguration(nameof(MicrosoftOAuthConfiguration))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        serviceCollection.AddKeyedScoped<IOAuthProvider, MicrosoftOAuthProvider>(nameof(UserProviderType.Microsoft));
        serviceCollection.AddKeyedScoped<IOAuthTokenRefreshHandler, MicrosoftTokenRefreshHandler>(nameof(UserProviderType.Microsoft));
        serviceCollection.AddScoped<IMicrosoftOAuthTokenClient, MicrosoftOAuthTokenClient>();
        return serviceCollection;
    }
}