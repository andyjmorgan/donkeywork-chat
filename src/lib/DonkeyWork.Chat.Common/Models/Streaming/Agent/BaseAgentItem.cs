// ------------------------------------------------------
// <copyright file="BaseAgentItem.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.Common.Models.Streaming.Agent;

/// <summary>
/// A base agent item.
/// </summary>
public record BaseAgentItem : BaseStreamItem
{
    /// <summary>
    /// Gets the agent id.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Gets the agent name.
    /// </summary>
    public string Name { get; set; } = string.Empty;
}
