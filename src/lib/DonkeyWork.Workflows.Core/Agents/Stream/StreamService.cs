// ------------------------------------------------------
// <copyright file="StreamService.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Runtime.CompilerServices;
using System.Threading.Channels;
using DonkeyWork.Chat.Common.Models.Streaming;

namespace DonkeyWork.Workflows.Core.Agents.Stream;

/// <summary>
/// Gets a stream service.
/// </summary>
public class StreamService : IStreamService
{
    private readonly Channel<BaseStreamItem> channel;

    /// <summary>
    /// Initializes a new instance of the <see cref="StreamService"/> class.
    /// </summary>
    public StreamService()
    {
        this.channel = Channel.CreateUnbounded<BaseStreamItem>();
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<BaseStreamItem> StreamAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var item in this.channel.Reader.ReadAllAsync(cancellationToken))
        {
            yield return item;
        }
    }

    /// <inheritdoc />
    public async Task AddStreamItem(BaseStreamItem streamItem, CancellationToken cancellationToken = default)
    {
        await this.channel.Writer.WriteAsync(streamItem, cancellationToken);
    }

    /// <inheritdoc />
    public Task FinalizeStream(CancellationToken cancellationToken = default)
    {
        this.channel.Writer.Complete();
        return Task.CompletedTask;
    }
}
