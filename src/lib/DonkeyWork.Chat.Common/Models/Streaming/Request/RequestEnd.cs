// ------------------------------------------------------
// <copyright file="RequestEnd.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.Common.Models.Streaming.Request;

/// <summary>
/// A request end.
/// </summary>
public record RequestEnd : BaseStreamItem
{
    /// <summary>
    /// Gets the duration of the request.
    /// </summary>
    public TimeSpan Duration { get; init; }

    /// <summary>
    /// Gets the conversation id.
    /// </summary>
    public Guid? ConversationId { get; init; }
}