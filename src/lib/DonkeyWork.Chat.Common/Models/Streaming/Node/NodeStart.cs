// ------------------------------------------------------
// <copyright file="NodeStart.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.Common.Models.Streaming.Node;

/// <summary>
/// A node start message.
/// </summary>
public record NodeStart : BaseNodeItem
{
    /// <summary>
    /// Gets the node start time.
    /// </summary>
    public DateTimeOffset StartTime { get; init; } = DateTimeOffset.UtcNow;
}
