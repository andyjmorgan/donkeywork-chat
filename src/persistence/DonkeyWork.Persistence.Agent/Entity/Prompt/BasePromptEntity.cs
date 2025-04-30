// ------------------------------------------------------
// <copyright file="BasePromptEntity.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using DonkeyWork.Persistence.Common.Entity.Base;

namespace DonkeyWork.Persistence.Agent.Entity.Prompt;

/// <summary>
/// A base prompt entity.
/// </summary>
public abstract class BasePromptEntity : BaseUserEntity
{
    /// <summary>
    /// Gets or sets the prompt name.
    /// </summary>
    [MaxLength(128)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the prompt description.
    /// </summary>
    [MaxLength(256)]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets the usage count.
    /// </summary>
    public int UsageCount { get; init; }
}