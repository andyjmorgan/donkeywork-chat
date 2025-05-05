// ------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Persistence.Agent;
using DonkeyWork.Workflows.Core.Actions.Services.ActionConsumer;
using DonkeyWork.Workflows.Core.Actions.Services.ActionExecution;
using DonkeyWork.Workflows.Core.Actions.Services.ActionOrchestrator;
using DonkeyWork.Workflows.Core.Scheduler.Jobs;
using DonkeyWork.Workflows.Core.Scheduler.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;

namespace DonkeyWork.Workflows.Core.Extensions;

/// <summary>
/// A service collection extension for adding the user context.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the user context.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>A <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddWorkflowCore(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        // Register the queue as a singleton
        serviceCollection.AddSingleton<IActionExecutionQueueService, ActionExecutionQueueService>();

        // Register the host service as a singleton
        serviceCollection.AddSingleton<IActionExecutionHostService, ActionExecutionHostService>(provider =>
            new ActionExecutionHostService(
                provider.GetRequiredService<ILogger<ActionExecutionHostService>>(),
                provider.GetRequiredService<IActionExecutionQueueService>(),
                provider.GetRequiredService<IServiceScopeFactory>()));

        serviceCollection.AddQuartz(q =>
        {
            q.AddJob<ActionJob>(opts =>
            {
                opts.WithDescription("Action job")
                    .StoreDurably();
            });
            q.UsePersistentStore(options =>
            {
                options.UseNewtonsoftJsonSerializer();
                options.UseClustering();
                options.PerformSchemaValidation = true;
                options.UsePostgres(builder =>
                {
                    builder.ConnectionString = configuration.GetConnectionString(nameof(AgentPersistenceContext)) ??
                                               string.Empty;
                    builder.TablePrefix = "qrtz_";
                });
            });
        });

        serviceCollection.AddQuartzHostedService(options =>
        {
            options.AwaitApplicationStarted = true;

            // awaits jobs for shutdown.
            options.WaitForJobsToComplete = true;
        });

        // Register the publisher service as scoped
        serviceCollection.AddScoped<IActionExecutionPublisherService, ActionExecutionPublisherService>();
        serviceCollection.AddScoped<IActionOrchestratorService, ActionOrchestratorService>();
        serviceCollection.AddScoped<IActionExecutionService, ActionExecutionService>();
        serviceCollection.AddTransient<IQuartzSchemaInitializer, QuartzSchemaInitializer>();
        return serviceCollection;
    }
}
