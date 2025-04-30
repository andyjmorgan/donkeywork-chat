// ------------------------------------------------------
// <copyright file="SystemPromptEntity.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.ComponentModel.DataAnnotations.Schema;

// ReSharper disable EntityFramework.ModelValidation.UnlimitedStringLength
namespace DonkeyWork.Persistence.Agent.Entity.Prompt;

/// <summary>
/// Gets the prompt entity.
/// </summary>
public class SystemPromptEntity : BasePromptEntity
{
    /// <summary>
    /// Gets or sets the text of the prompt.
    /// </summary>
    [Column(TypeName = "jsonb")]
    public List<string> Content { get; set; } = [];
}