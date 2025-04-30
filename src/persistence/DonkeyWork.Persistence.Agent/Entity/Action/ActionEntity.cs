// ------------------------------------------------------
// <copyright file="ActionEntity.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DonkeyWork.Chat.Common.Models.Actions;
using DonkeyWork.Chat.Common.Models.Providers.Tools;
using DonkeyWork.Persistence.Common.Entity.Base;

namespace DonkeyWork.Persistence.Agent.Entity.Action;

/// <summary>
/// Gets or sets the action entity.
/// </summary>
public class ActionEntity : BaseUserEntity
{
    /// <summary>
    /// Gets or sets the action type.
    /// </summary>
    [MaxLength(256)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the action description.
    /// </summary>
    [MaxLength(512)]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the action icon.
    /// </summary>
    [MaxLength(512)]
    public string? Icon { get; set; }

    /// <summary>
    /// Gets or sets the system prompts navigation property.
    /// </summary>
    public virtual ICollection<ActionSystemPromptRelationEntity> SystemPrompts { get; set; } = [];

    /// <summary>
    /// Gets or sets action prompts navigation property.
    /// </summary>
    public virtual ICollection<ActionPromptRelationEntity> ActionPrompts { get; set; } = [];

    /// <summary>
    /// Gets or sets the allowed tools.
    /// </summary>
    [Column(TypeName = "jsonb")]
    public List<ToolProviderApplicationType> AllowedTools { get; set; } = [];

    /// <summary>
    /// Gets or sets the action model configuration.
    /// </summary>
    [Column(TypeName = "jsonb")]
    public ActionModelConfiguration ActionModelConfiguration { get; set; } = new ActionModelConfiguration();
}
