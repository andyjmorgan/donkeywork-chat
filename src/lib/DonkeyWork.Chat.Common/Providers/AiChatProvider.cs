// ------------------------------------------------------
// <copyright file="AiChatProvider.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.Common.Providers;

/// <summary>
/// Known and supported Ai Chat Providers.
/// </summary>
public enum AiChatProvider
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

    /// <summary>
    /// Google gemini.
    /// </summary>
    Gemini,
}