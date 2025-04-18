// ------------------------------------------------------
// <copyright file="AIChatProviderFactory.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace DonkeyWork.Chat.AiServices.Clients;

/// <inheritdoc />
public class AIChatProviderFactory(IServiceProvider serviceProvider)
    : IAIChatProviderFactory
{
    /// <inheritdoc />
    public IAIChatClient CreateChatClient(AiChatProvider provider)
    {
        return serviceProvider.GetKeyedService<IAIChatClient>(provider)
               ?? throw new NotImplementedException($"Provider {provider} not implemented");
    }
}