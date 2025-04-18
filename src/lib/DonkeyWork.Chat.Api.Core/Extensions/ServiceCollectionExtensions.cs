// ------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Api.Core.Configuration;
using DonkeyWork.Chat.Api.Core.Services.Keycloak;
using Microsoft.Extensions.DependencyInjection;

namespace DonkeyWork.Chat.Api.Core.Extensions;

/// <summary>
/// A service collection extension for adding the Core Api services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds AI services.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <returns>A <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddApiCoreServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddOptions<KeycloakConfiguration>()
            .BindConfiguration(nameof(KeycloakConfiguration))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        return serviceCollection.AddScoped<IKeycloakClient, KeycloakClient>();
    }
}