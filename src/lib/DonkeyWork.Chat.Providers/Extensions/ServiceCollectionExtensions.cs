// ------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Models.Providers.Tools;
using DonkeyWork.Chat.Providers.Provider;
using DonkeyWork.Chat.Providers.Provider.Configuration;
using DonkeyWork.Chat.Providers.Provider.Implementation.Discord;
using DonkeyWork.Chat.Providers.Provider.Implementation.Discord.Client;
using DonkeyWork.Chat.Providers.Provider.Implementation.Google;
using DonkeyWork.Chat.Providers.Provider.Implementation.Google.Client;
using DonkeyWork.Chat.Providers.Provider.Implementation.Microsoft;
using DonkeyWork.Chat.Providers.Provider.Implementation.Microsoft.Client;
using DonkeyWork.Chat.Providers.Services.TokenRefresh;
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
        serviceCollection.AddKeyedScoped<IOAuthProvider, MicrosoftOAuthProvider>(nameof(ToolProviderType.Microsoft));
        serviceCollection.AddKeyedScoped<IOAuthTokenRefreshHandler, MicrosoftTokenRefreshHandler>(nameof(ToolProviderType.Microsoft));
        serviceCollection.AddKeyedScoped<ITokenRefreshService, MicrosoftTokenRefreshService>(nameof(ToolProviderType.Microsoft));
        serviceCollection.AddScoped<IMicrosoftOAuthTokenClient, MicrosoftOAuthTokenClient>();

        // Register Google services
        serviceCollection.AddKeyedScoped<IOAuthProvider, GoogleOAuthProvider>(nameof(ToolProviderType.Google));
        serviceCollection.AddKeyedScoped<IOAuthTokenRefreshHandler, GoogleTokenRefreshHandler>(nameof(ToolProviderType.Google));
        serviceCollection.AddKeyedScoped<ITokenRefreshService, GoogleTokenRefreshService>(nameof(ToolProviderType.Google));
        serviceCollection.AddScoped<IGoogleOAuthTokenClient, GoogleOAuthTokenClient>();

        // Register Discord services
        serviceCollection.AddKeyedScoped<IOAuthProvider, DiscordOAuthProvider>(nameof(ToolProviderType.Discord));
        serviceCollection.AddKeyedScoped<IOAuthTokenRefreshHandler, DiscordTokenRefreshHandler>(nameof(ToolProviderType.Discord));
        serviceCollection.AddKeyedScoped<ITokenRefreshService, DiscordTokenRefreshService>(nameof(ToolProviderType.Discord));
        serviceCollection.AddScoped<IDiscordOAuthTokenClient, DiscordOAuthTokenClient>();

        return serviceCollection;
    }
}