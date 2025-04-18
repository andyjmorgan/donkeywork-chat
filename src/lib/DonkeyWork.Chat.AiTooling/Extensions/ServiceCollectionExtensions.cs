// ------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.AiTooling.Base;
using DonkeyWork.Chat.AiTooling.Services;
using DonkeyWork.Chat.AiTooling.ToolImplementations.CurrentDateTime.Tool;
using DonkeyWork.Chat.AiTooling.ToolImplementations.Discord.Common.Api;
using DonkeyWork.Chat.AiTooling.ToolImplementations.Discord.Guild.Tool;
using DonkeyWork.Chat.AiTooling.ToolImplementations.GoogleApi.Common.Api;
using DonkeyWork.Chat.AiTooling.ToolImplementations.GoogleApi.Drive;
using DonkeyWork.Chat.AiTooling.ToolImplementations.GoogleApi.Gmail;
using DonkeyWork.Chat.AiTooling.ToolImplementations.GoogleApi.Identity;
using DonkeyWork.Chat.AiTooling.ToolImplementations.MicrosoftGraph.Common.Api;
using DonkeyWork.Chat.AiTooling.ToolImplementations.MicrosoftGraph.Drive.Tool;
using DonkeyWork.Chat.AiTooling.ToolImplementations.MicrosoftGraph.Identity;
using DonkeyWork.Chat.AiTooling.ToolImplementations.MicrosoftGraph.Mail.Tool;
using DonkeyWork.Chat.AiTooling.ToolImplementations.MicrosoftGraph.Todo;
using DonkeyWork.Chat.AiTooling.ToolImplementations.SerpApi.Api;
using DonkeyWork.Chat.AiTooling.ToolImplementations.SerpApi.Configuration;
using DonkeyWork.Chat.AiTooling.ToolImplementations.SerpApi.Tool;
using DonkeyWork.Chat.AiTooling.ToolImplementations.Swarmpit.Api;
using DonkeyWork.Chat.AiTooling.ToolImplementations.Swarmpit.Tool;
using Microsoft.Extensions.DependencyInjection;

namespace DonkeyWork.Chat.AiTooling.Extensions;

/// <summary>
/// A service collection extension for adding the AI services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds AI services.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <returns>A <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddToolServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddOptions<SerpApiConfiguration>()
            .BindConfiguration(nameof(SerpApiConfiguration))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        FindAndAddIToolImplementations(serviceCollection);
        return serviceCollection.AddScoped<IToolService, ToolService>()
            .AddScoped<ISerpApiSearch, SerpApiSearch>()
            .AddScoped<ISerpTool, SerpTool>()
            .AddScoped<IMicrosoftGraphDriveTool, MicrosoftGraphDriveTool>()
            .AddScoped<IMicrosoftGraphMailTool, MicrosoftGraphMailTool>()
            .AddScoped<IMicrosoftGraphTodoTool, MicrosoftGraphTodoTool>()
            .AddScoped<IMicrosoftGraphIdentityTool, MicrosoftGraphIdentityTool>()
            .AddScoped<IDiscordGuildTool, DiscordGuildTool>()
            .AddScoped<IMicrosoftGraphApiClientFactory, MicrosoftGraphApiClientFactory>()
            .AddScoped<IDiscordApiClientFactory, DiscordApiClientFactory>()
            .AddScoped<IGoogleApiClientFactory, GoogleApiClientFactory>()
            .AddScoped<IGoogleDriveTool, GoogleDriveTool>()
            .AddScoped<IGmailTool, GmailTool>()
            .AddScoped<IGoogleIdentityTool, GoogleIdentityTool>()
            .AddScoped<ICurrentDateTimeTool, CurrentDateTimeTool>()
            .AddHttpClient()
            .AddScoped<ISwarmpitClientFactory, SwarmpitClientFactory>()
            .AddScoped<ISwarmpitTool, SwarmpitTool>();
    }

    private static void FindAndAddIToolImplementations(IServiceCollection serviceCollection)
    {
        var toolTypes = typeof(ITool).Assembly
            .GetTypes()
            .Where(t => typeof(ITool).IsAssignableFrom(t) &&
                        !t.IsInterface &&
                        !t.IsAbstract)
            .ToList();

        foreach (var toolType in toolTypes)
        {
            serviceCollection.AddScoped(typeof(ITool), toolType);
        }
    }
}