// ------------------------------------------------------
// <copyright file="ActionBaseRelation.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Persistence.Common.Entity.Base;

namespace DonkeyWork.Persistence.Agent.Entity.Action;

/// <summary>
/// A base action relation entity.
/// </summary>
public abstract class ActionBaseRelation : BaseUserEntity
{
    /// <summary>
    /// Gets or sets the action id.
    /// </summary>
    public Guid ActionId { get; set; }

    /// <summary>
    /// Gets or sets the action entity.
    /// </summary>
    public virtual ActionEntity? Action { get; set; }
}