// ------------------------------------------------------
// <copyright file="AiChatProviders.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.AiServices.Clients;

/// <summary>
/// Known and supported Ai Chat Providers.
/// </summary>
public enum AiChatProviders
{
    /// <summary>
    /// Open Ai.
    /// </summary>
    OpenAi,

    /// <summary>
    /// Anthropic.
    /// </summary>
    Anthropic,

    /// <summary>
    /// Ollama.
    /// </summary>
    Ollama,
}