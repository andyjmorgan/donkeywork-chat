// ------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Persistence.Common.Interceptors;
using DonkeyWork.Persistence.User.Repository.ApiKey;
using DonkeyWork.Persistence.User.Repository.Integration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DonkeyWork.Persistence.User.Extensions;

/// <summary>
/// Extension methods for the <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the credentials context persistence.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>A <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddUserPersistence(
        this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        return serviceCollection
            .AddScoped<IIntegrationRepository, IntegrationRepository>()
            .AddScoped<IApiKeyRepository, ApiKeyRepository>()
            .AddScoped<CreatedOrUpdatedInterceptor>()
            .AddDbContext<UserPersistenceContext>(
                (serviceProvider, options) =>
                {
                    options.EnableDetailedErrors();
                    options.EnableSensitiveDataLogging();
                    options.UseNpgsql(
                        configuration.GetConnectionString(
                            nameof(UserPersistenceContext)),
                        o =>
                        {
                            o.MigrationsHistoryTable("__EFMigrationsHistory", nameof(UserPersistenceContext));
                            o.ConfigureDataSource(d => d.EnableDynamicJson());
                        });

                    options.AddInterceptors(serviceProvider.GetRequiredService<CreatedOrUpdatedInterceptor>());
                });
    }
}