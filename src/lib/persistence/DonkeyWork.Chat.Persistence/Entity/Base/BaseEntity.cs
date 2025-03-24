// ------------------------------------------------------
// <copyright file="BaseEntity.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.ComponentModel.DataAnnotations;

namespace DonkeyWork.Chat.Persistence.Entity.Base;

/// <summary>
/// A base Entity.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Gets the conversation ID.
    /// </summary>
    [Key]
    public Guid Id { get; init; }

    /// <summary>
    /// Gets the time the entity was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets or sets the last updated time.
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }
}