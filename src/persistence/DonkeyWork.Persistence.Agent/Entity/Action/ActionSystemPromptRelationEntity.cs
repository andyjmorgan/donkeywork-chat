// ------------------------------------------------------
// <copyright file="ActionSystemPromptRelationEntity.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Persistence.Agent.Entity.Prompt;

namespace DonkeyWork.Persistence.Agent.Entity.Action;

/// <summary>
/// An action system prompt relation entity.
/// </summary>
public class ActionSystemPromptRelationEntity : ActionBaseRelation
{
    /// <summary>
    /// Gets or sets the system prompt id.
    /// </summary>
    public Guid SystemPromptId { get; set; }

    /// <summary>
    /// Gets or sets the system prompt.
    /// </summary>
    public virtual SystemPromptEntity? SystemPrompt { get; set; }
}