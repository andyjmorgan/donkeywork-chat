// ------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.AiServices.Clients;
using DonkeyWork.Chat.AiServices.Clients.Anthropic;
using DonkeyWork.Chat.AiServices.Clients.Anthropic.Configuration;
using DonkeyWork.Chat.AiServices.Clients.Google;
using DonkeyWork.Chat.AiServices.Clients.Google.Configuration;
using DonkeyWork.Chat.AiServices.Clients.OpenAi;
using DonkeyWork.Chat.AiServices.Clients.OpenAi.Configuration;
using DonkeyWork.Chat.AiServices.Services;
using DonkeyWork.Chat.Common.Models.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace DonkeyWork.Chat.AiServices.Extensions;

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
    public static IServiceCollection AddAiServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddOptions<OpenAiConfiguration>()
            .BindConfiguration(nameof(OpenAiConfiguration))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        serviceCollection.AddOptions<AnthropicConfiguration>()
            .BindConfiguration(nameof(AnthropicConfiguration))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        serviceCollection.AddOptions<GeminiConfiguration>()
            .BindConfiguration(nameof(GeminiConfiguration))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return serviceCollection.AddScoped<IAIChatProviderFactory, AIChatProviderFactory>()
            .AddScoped<IChatService, ChatService>()
            .AddKeyedScoped<IAIChatClient, OpenAIChatClient>(AiChatProvider.OpenAi)
            .AddKeyedScoped<IAIChatClient, GeminiChatClient>(AiChatProvider.Gemini)
            .AddKeyedScoped<IAIChatClient, AnthropicChatClient>(AiChatProvider.Anthropic);
    }
}