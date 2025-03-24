// ------------------------------------------------------
// <copyright file="IAIChatClient.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.AiServices.Clients.Models;
using DonkeyWork.Chat.AiServices.Streaming;
using DonkeyWork.Chat.AiTooling.Base.Models;

namespace DonkeyWork.Chat.AiServices.Clients;

/// <summary>
/// A Chat client interface.
/// </summary>
public interface IAIChatClient
{
    /// <summary>
    /// stream a conversation request with the model.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="toolDefinitions">The tool definitions.</param>
    /// <param name="toolAction">The tool actions.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A stream of <see cref="BaseStreamItem"/>.</returns>
    public IAsyncEnumerable<BaseStreamItem> StreamChatAsync(
        ChatRequest request,
        List<ToolDefinition> toolDefinitions,
        Func<ToolCallback, Task<string>> toolAction,
        CancellationToken cancellationToken);
}