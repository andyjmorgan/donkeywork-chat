// ------------------------------------------------------
// <copyright file="IStreamService.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Models.Streaming;

namespace DonkeyWork.Workflows.Core.Agents.Stream;

/// <summary>
/// A stream service.
/// </summary>
public interface IStreamService
{
    /// <summary>
    /// Streams an active execution.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="BaseStreamItem"/>.</returns>
    public IAsyncEnumerable<BaseStreamItem> StreamAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a stream message to the channel.
    /// </summary>
    /// <param name="streamItem">The stream item to add.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task AddStreamItem(BaseStreamItem streamItem, CancellationToken cancellationToken = default);

    /// <summary>
    /// Finalizes the stream.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task FinalizeStream(CancellationToken cancellationToken = default);
}
