// ------------------------------------------------------
// <copyright file="IAIChatClient.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Text.Json;
using DonkeyWork.Chat.AiServices.Clients.Models;
using DonkeyWork.Chat.AiTooling.Base.Models;
using DonkeyWork.Chat.Common.Models.Streaming;

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
        Func<ToolCallback, Task<JsonDocument>> toolAction,
        CancellationToken cancellationToken);

    /// <summary>
    /// send a conversation request with the model.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="toolDefinitions">The tool definitions.</param>
    /// <param name="toolAction">The tool actions.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A stream of <see cref="BaseStreamItem"/>.</returns>
    public IAsyncEnumerable<BaseStreamItem> ChatAsync(
        ChatRequest request,
        List<ToolDefinition> toolDefinitions,
        Func<ToolCallback, Task<JsonDocument>> toolAction,
        CancellationToken cancellationToken);
}