// ------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Persistence.Interceptors;
using DonkeyWork.Chat.Persistence.Repository.Conversation;
using DonkeyWork.Chat.Persistence.Repository.Integration;
using DonkeyWork.Chat.Persistence.Repository.Prompt;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DonkeyWork.Chat.Persistence.Extensions;

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
    public static IServiceCollection AddCredentialsPersistence(
        this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        return serviceCollection
            .AddScoped<IConversationRepository, ConversationRepository>()
            .AddScoped<IPromptRepository, PromptRepository>()
            .AddScoped<IIntegrationRepository, IntegrationRepository>()
            .AddScoped<CreatedOrUpdatedInterceptor>()
            .AddDbContext<ApiPersistenceContext>(
                (serviceProvider, options) =>
                {
                    options.EnableDetailedErrors();
                    options.EnableSensitiveDataLogging();
                    options.UseNpgsql(
                        configuration.GetConnectionString(
                            nameof(ApiPersistenceContext)),
                        o =>
                        {
                            o.MigrationsHistoryTable("__EFMigrationsHistory", "ApiPersistence");
                            o.ConfigureDataSource(d => d.EnableDynamicJson());
                        });

                    options.AddInterceptors(serviceProvider.GetRequiredService<CreatedOrUpdatedInterceptor>());
                });
    }
}