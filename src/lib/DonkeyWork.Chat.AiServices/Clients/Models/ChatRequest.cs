// ------------------------------------------------------
// <copyright file="ChatRequest.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.AiServices.Clients.Models;

/// <summary>
/// A chat request.
/// </summary>
public record ChatRequest
{
    /// <summary>
    /// Gets the model name.
    /// </summary>
    required public string ModelName { get; init; }

    /// <summary>
    /// Gets the temperature.
    /// </summary>
    public int Temperature { get; init; } = 0;

    /// <summary>
    /// Gets a unique identifier for the chat request.
    /// </summary>
    public Guid Id { get; init; } = Guid.NewGuid();

    /// <summary>
    /// Gets the chat request metadata.
    /// </summary>
    public Dictionary<string, string> Metadata { get; init; } = [];

    /// <summary>
    /// Gets the messages.
    /// </summary>
    public List<GenericChatMessage> Messages { get; init; } = [];

    /// <summary>
    /// Gets the system messages.
    /// </summary>
    /// <returns>A List of system messages.</returns>
    internal List<GenericChatMessage> GetSystemMessages() =>
        this.GetValidMessages().Where(x => x.Role == GenericMessageRole.System).ToList();

    /// <summary>
    /// Gets the system messages.
    /// </summary>
    /// <returns>A List of system messages.</returns>
    internal List<GenericChatMessage> GetNonSystemMessages() =>
        this.GetValidMessages().Where(x => x.Role != GenericMessageRole.System).ToList();

    /// <summary>
    /// Gets the valid messages.
    /// </summary>
    /// <returns>A list of valid messages.</returns>
    internal List<GenericChatMessage> GetValidMessages() =>
    this.Messages.Where(x => !string.IsNullOrWhiteSpace(x.Content)).ToList();
}
