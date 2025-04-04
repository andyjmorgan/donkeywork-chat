// ------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.AiTooling.Base;
using DonkeyWork.Chat.AiTooling.Services;
using DonkeyWork.Chat.AiTooling.ToolImplementations.CurrentDateTime.Tool;
using DonkeyWork.Chat.AiTooling.ToolImplementations.MicrosoftGraph.Common.Api;
using DonkeyWork.Chat.AiTooling.ToolImplementations.MicrosoftGraph.Drive.Tool;
using DonkeyWork.Chat.AiTooling.ToolImplementations.MicrosoftGraph.Mail.Tool;
using DonkeyWork.Chat.AiTooling.ToolImplementations.SerpApi.Api;
using DonkeyWork.Chat.AiTooling.ToolImplementations.SerpApi.Configuration;
using DonkeyWork.Chat.AiTooling.ToolImplementations.SerpApi.Tool;
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

        return serviceCollection.AddScoped<IToolService, ToolService>()
            .AddScoped<ISerpApiSearch, SerpApiSearch>()
            .AddScoped<ISerpTool, SerpTool>()
            .AddScoped<ITool, SerpTool>()
            .AddScoped<ITool, CurrentDateTimeTool>()
            .AddScoped<ITool, MicrosoftGraphDriveTool>()
            .AddScoped<ITool, MicrosoftGraphMailTool>()
            .AddScoped<IMicrosoftGraphDriveTool, MicrosoftGraphDriveTool>()
            .AddScoped<IMicrosoftGraphMailTool, MicrosoftGraphMailTool>()
            .AddScoped<IMicrosoftGraphApiClientFactory, MicrosoftGraphApiClientFactory>()
            .AddScoped<ICurrentDateTimeTool, CurrentDateTimeTool>();
    }
}