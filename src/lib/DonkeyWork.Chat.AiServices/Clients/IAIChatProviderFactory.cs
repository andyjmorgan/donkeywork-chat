// ------------------------------------------------------
// <copyright file="IAIChatProviderFactory.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Providers;

namespace DonkeyWork.Chat.AiServices.Clients;

/// <summary>
/// An AI chat provider factory.
/// </summary>
public interface IAIChatProviderFactory
{
    /// <summary>
    /// Create a chat client.
    /// </summary>
    /// <param name="provider">The provider.</param>
    /// <returns>A <see cref="IAIChatClient"/>.</returns>
    public IAIChatClient CreateChatClient(AiChatProvider provider);
}