// ------------------------------------------------------
// <copyright file="IChatService.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.AiServices.Streaming;

namespace DonkeyWork.Chat.AiServices.Services;

/// <summary>
/// A chat service interface.
/// </summary>
public interface IChatService
{
    /// <summary>
    /// Get a response from the chat service.
    /// </summary>
    /// <param name="chatServiceRequest">The chat service request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A stream of <see cref="BaseStreamItem"/>.</returns>
    public IAsyncEnumerable<BaseStreamItem> GetResponseAsync(
        ChatServiceRequest chatServiceRequest,
        CancellationToken cancellationToken = default);
}