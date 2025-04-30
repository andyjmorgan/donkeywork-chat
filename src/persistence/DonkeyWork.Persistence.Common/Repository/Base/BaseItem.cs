// ------------------------------------------------------
// <copyright file="BaseItem.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Persistence.Common.Repository.Base;

/// <summary>
/// A base repository item.
/// </summary>
public abstract record BaseItem
{
    /// <summary>
    /// Gets the item's ID.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Gets the creation time.
    /// </summary>
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>
    /// Gets the last updated time.
    /// </summary>
    public DateTimeOffset UpdatedAt { get; init; }
}