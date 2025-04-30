// ------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Persistence.Chat.Repository.Conversation;
using DonkeyWork.Persistence.Common.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DonkeyWork.Persistence.Chat.Extensions;

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
    public static IServiceCollection AddChatPersistence(
        this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        return serviceCollection
            .AddScoped<IConversationRepository, ConversationRepository>()
            .AddScoped<CreatedOrUpdatedInterceptor>()
            .AddDbContext<ChatPersistenceContext>(
                (serviceProvider, options) =>
                {
                    options.EnableDetailedErrors();
                    options.EnableSensitiveDataLogging();
                    options.UseNpgsql(
                        configuration.GetConnectionString(
                            nameof(ChatPersistenceContext)),
                        o =>
                        {
                            o.MigrationsHistoryTable("__EFMigrationsHistory", nameof(ChatPersistenceContext));
                            o.ConfigureDataSource(d => d.EnableDynamicJson());
                        });

                    options.AddInterceptors(serviceProvider.GetRequiredService<CreatedOrUpdatedInterceptor>());
                });
    }
}