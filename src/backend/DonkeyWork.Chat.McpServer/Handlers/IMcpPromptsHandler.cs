// ------------------------------------------------------
// <copyright file="IMcpPromptsHandler.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using ModelContextProtocol.Protocol.Types;
using ModelContextProtocol.Server;

namespace DonkeyWork.Chat.McpServer.Handlers;

/// <summary>
/// An interface for handling Mcp prompts.
/// </summary>
public interface IMcpPromptsHandler
{
    /// <summary>
    /// Handles the ListTools request and returns a result.
    /// </summary>
    /// <param name="context">The request context containing the parameters.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation, containing the ListTools result.</returns>
    Task<ListPromptsResult> HandleListAsync(RequestContext<ListPromptsRequestParams> context, CancellationToken cancellationToken);

    /// <summary>
    /// Handles a tool call.
    /// </summary>
    /// <param name="context">The request context.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<GetPromptResult> HandleGetAsync(RequestContext<GetPromptRequestParams> context, CancellationToken cancellationToken);
}