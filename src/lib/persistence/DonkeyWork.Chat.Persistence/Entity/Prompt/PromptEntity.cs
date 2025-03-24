// ------------------------------------------------------
// <copyright file="PromptEntity.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using DonkeyWork.Chat.Persistence.Entity.Base;

// ReSharper disable EntityFramework.ModelValidation.UnlimitedStringLength
namespace DonkeyWork.Chat.Persistence.Entity.Prompt;

/// <summary>
/// Gets the prompt entity.
/// </summary>
public class PromptEntity : BaseUserEntity
{
    /// <summary>
    /// Gets or sets the name of the prompt.
    /// </summary>
    [MaxLength(64)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the prompt.
    /// </summary>
    [MaxLength(256)]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the text of the prompt.
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Gets the usage count.
    /// </summary>
    public int UsageCount { get; init; }
}