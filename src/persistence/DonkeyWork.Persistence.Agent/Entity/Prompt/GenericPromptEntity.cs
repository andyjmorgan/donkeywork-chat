// ------------------------------------------------------
// <copyright file="GenericPromptEntity.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DonkeyWork.Chat.Common.Models.Prompt;
using DonkeyWork.Persistence.Common.Entity.Base;

namespace DonkeyWork.Persistence.Agents.Entity.Prompt;

/// <summary>
/// A generic prompt entity.
/// </summary>
public class GenericPromptEntity : BasePromptEntity
{
    /// <summary>
    /// Gets or sets the prompt variables.
    /// </summary>
    [Column(TypeName = "jsonb")]
    public List<PromptVariable> Variables { get; set; } = [];

    /// <summary>
    /// Gets or sets the prompt messages.
    /// </summary>
    [Column(TypeName = "jsonb")]
    public List<PromptMessage> Messages { get; set; } = [];
}