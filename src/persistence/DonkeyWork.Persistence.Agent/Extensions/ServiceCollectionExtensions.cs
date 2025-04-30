// ------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Persistence.Agent.Repository.Action;
using DonkeyWork.Persistence.Agent.Repository.ActionExecution;
using DonkeyWork.Persistence.Agent.Repository.Prompt;
using DonkeyWork.Persistence.Common.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace DonkeyWork.Persistence.Agent.Extensions;

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
    public static IServiceCollection AddAgentPersistence(
        this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        serviceCollection
            .AddScoped<IPromptRepository, PromptRepository>()
            .AddScoped<IActionRepository, ActionRepository>()
            .AddScoped<IActionExecutionRepository, ActionExecutionRepository>();

        return serviceCollection
            .AddScoped<CreatedOrUpdatedInterceptor>()
            .AddDbContext<AgentPersistenceContext>(
                (serviceProvider, options) =>
                {
                    options.EnableDetailedErrors();
                    options.EnableSensitiveDataLogging();
                    options.UseNpgsql(
                        configuration.GetConnectionString(
                            nameof(AgentPersistenceContext)),
                        o =>
                        {
                            o.MigrationsHistoryTable("__EFMigrationsHistory", nameof(AgentPersistenceContext));
                            o.ConfigureDataSource(d => d.EnableDynamicJson());
                        });

                    options.AddInterceptors(serviceProvider.GetRequiredService<CreatedOrUpdatedInterceptor>());
                });
    }
}
