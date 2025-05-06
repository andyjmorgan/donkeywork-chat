// ------------------------------------------------------
// <copyright file="IAgentOrchestrator.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.AiServices.Clients.Models;
using DonkeyWork.Chat.Common.Models.Chat;

namespace DonkeyWork.Workflows.Core.Agents.Orchestrator;

/// <summary>
/// An agent orchestrator.
/// </summary>
public interface IAgentOrchestrator
{
    /// <summary>
    /// Executes the agent orchestrator.
    /// </summary>
    /// <param name="agentId">The agent id.</param>
    /// <param name="messages">The messages.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task ExecuteAsync(
        Guid agentId,
        List<GenericChatMessage> messages,
        CancellationToken cancellationToken = default);
}
