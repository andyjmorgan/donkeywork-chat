// ------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Providers;
using DonkeyWork.Chat.Providers.Provider;
using DonkeyWork.Chat.Providers.Provider.Configuration;
using DonkeyWork.Chat.Providers.Provider.Implementation.Discord;
using DonkeyWork.Chat.Providers.Provider.Implementation.Discord.Client;
using DonkeyWork.Chat.Providers.Provider.Implementation.Google;
using DonkeyWork.Chat.Providers.Provider.Implementation.Google.Client;
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

        serviceCollection.AddOptions<GoogleOAuthConfiguration>()
            .BindConfiguration(nameof(GoogleOAuthConfiguration))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        serviceCollection.AddOptions<DiscordOAuthConfiguration>()
            .BindConfiguration(nameof(DiscordOAuthConfiguration))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        // Register Microsoft services
        serviceCollection.AddKeyedScoped<IOAuthProvider, MicrosoftOAuthProvider>(nameof(UserProviderType.Microsoft));
        serviceCollection.AddKeyedScoped<IOAuthTokenRefreshHandler, MicrosoftTokenRefreshHandler>(nameof(UserProviderType.Microsoft));
        serviceCollection.AddScoped<IMicrosoftOAuthTokenClient, MicrosoftOAuthTokenClient>();

        // Register Google services
        serviceCollection.AddKeyedScoped<IOAuthProvider, GoogleOAuthProvider>(nameof(UserProviderType.Google));
        serviceCollection.AddKeyedScoped<IOAuthTokenRefreshHandler, GoogleTokenRefreshHandler>(nameof(UserProviderType.Google));
        serviceCollection.AddScoped<IGoogleOAuthTokenClient, GoogleOAuthTokenClient>();

        // Register Discord services
        serviceCollection.AddKeyedScoped<IOAuthProvider, DiscordOAuthProvider>(nameof(UserProviderType.Discord));
        serviceCollection.AddKeyedScoped<IOAuthTokenRefreshHandler, DiscordTokenRefreshHandler>(nameof(UserProviderType.Discord));
        serviceCollection.AddScoped<IDiscordOAuthTokenClient, DiscordOAuthTokenClient>();

        return serviceCollection;
    }
}