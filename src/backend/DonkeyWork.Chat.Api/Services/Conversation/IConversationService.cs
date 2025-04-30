// ------------------------------------------------------
// <copyright file="IConversationService.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.AiServices.Services;
using DonkeyWork.Chat.Common.Models.Streaming;

namespace DonkeyWork.Chat.Api.Services.Conversation;

/// <summary>
/// A conversation service interface.
/// </summary>
public interface IConversationService
{
    /// <summary>
    /// wraps the chat service in a conversation service.
    /// </summary>
    /// <param name="chatServiceRequest">The chat service request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A stream of <see cref="BaseStreamItem"/>.</returns>
    public IAsyncEnumerable<BaseStreamItem> GetResponseAsync(
        ChatServiceRequest chatServiceRequest,
        CancellationToken cancellationToken = default);
}