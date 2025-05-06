// ------------------------------------------------------
// <copyright file="IAgentComposer.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Contracts;
using DonkeyWork.Workflows.Core.Agents.Nodes;

namespace DonkeyWork.Workflows.Core.Agents.Composer;

/// <summary>
/// An agent composer.
/// </summary>
public interface IAgentComposer
{
    /// <summary>
    /// Composes an agent.
    /// </summary>
    /// <param name="id">The agent id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="List{T}"/> of <see cref="BaseAgentNode"/>.</returns>
    public Task<List<IAgentNode>> ComposeAgentAsync(Guid id, CancellationToken cancellationToken = default);
}
