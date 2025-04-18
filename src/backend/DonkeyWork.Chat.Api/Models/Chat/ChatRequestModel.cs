// ------------------------------------------------------
// <copyright file="ChatRequestModel.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.AiServices.Clients.Models;
using DonkeyWork.Chat.Common.Providers;

namespace DonkeyWork.Chat.Api.Models.Chat;

/// <summary>
/// A chat request model.
/// </summary>
public record ChatRequestModel
{
    /// <summary>
    /// Gets the provider.
    /// </summary>
    public AiChatProvider Provider { get; init; } = AiChatProvider.Anthropic;

    /// <summary>
    /// Gets the model.
    /// </summary>
    // public string Model { get; init; } = "gpt-4o-mini";
    public string Model { get; init; } = "claude-3-5-haiku-latest";

    /// <summary>
    /// Gets the messages.
    /// </summary>
    public List<GenericChatMessage> Messages { get; init; } = [];

    /// <summary>
    /// Gets the chat parameters.
    /// </summary>
    public Dictionary<string, object> Parameters { get; init; } = [];

    /// <summary>
    /// Gets the conversation id.
    /// </summary>
    public Guid? ConversationId { get; init; }

    /// <summary>
    /// Gets the (Optional) prompt id.
    /// </summary>
    public Guid? PromptId { get; init; }
}