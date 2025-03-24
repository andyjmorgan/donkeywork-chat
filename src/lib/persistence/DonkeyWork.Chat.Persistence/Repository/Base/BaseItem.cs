// ------------------------------------------------------
// <copyright file="BaseItem.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.Persistence.Repository.Base;

/// <summary>
/// A base entity to item.
/// </summary>
public abstract record BaseItem
{
    /// <summary>
    /// Gets the conversation's unique identifier.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Gets the last message date.
    /// </summary>
    public DateTimeOffset UpdatedAt { get; init; }

    /// <summary>
    /// Gets the date created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; init; }
}