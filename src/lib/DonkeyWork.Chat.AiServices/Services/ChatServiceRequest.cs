// ------------------------------------------------------
// <copyright file="ChatServiceRequest.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.AiServices.Clients.Models;
using DonkeyWork.Chat.Common.Models.Chat;
using DonkeyWork.Chat.Common.Models.Providers;

namespace DonkeyWork.Chat.AiServices.Services;

/// <summary>
/// A chat service request.
/// </summary>
public record ChatServiceRequest
{
    /// <summary>
    /// Gets the provider.
    /// </summary>
    public AiChatProvider Provider { get; init; }

    /// <summary>
    /// Gets the conversation id.
    /// </summary>
    public Guid? ConversationId { get; init; }

    /// <summary>
    /// Gets the execution id.
    /// </summary>
    public Guid ExecutionId { get; init; } = Guid.NewGuid();

    /// <summary>
    /// Gets the model.
    /// </summary>
    required public string Model { get; init; }

    /// <summary>
    /// Gets the provider parameters.
    /// </summary>
    public Dictionary<string, object> Parameters { get; init; } = [];

    /// <summary>
    /// Gets the messages.
    /// </summary>
    public List<GenericChatMessage> Messages { get; init; } = [];
}