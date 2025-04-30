// ------------------------------------------------------
// <copyright file="ActionPromptRelationEntity.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Persistence.Agent.Entity.Prompt;

namespace DonkeyWork.Persistence.Agent.Entity.Action;

/// <summary>
/// Gets or sets the action prompt relation entity.
/// </summary>
public class ActionPromptRelationEntity : ActionBaseRelation
{
    /// <summary>
    /// Gets or sets the action id.
    /// </summary>
    public Guid ActionPromptId { get; set; }

    /// <summary>
    /// Gets or sets the action prompt.
    /// </summary>
    public virtual ActionPromptEntity? ActionPrompt { get; set; }
}
