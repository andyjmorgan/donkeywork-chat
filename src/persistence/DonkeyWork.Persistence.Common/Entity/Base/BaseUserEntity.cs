// ------------------------------------------------------
// <copyright file="BaseUserEntity.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Persistence.Common.Entity.Base;

/// <summary>
/// A base user entity.
/// </summary>
public abstract class BaseUserEntity : BaseEntity
{
    /// <summary>
    /// Gets or sets the user ID associated with the entity.
    /// </summary>
    public Guid UserId { get; set; }
}