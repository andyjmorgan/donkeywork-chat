// ------------------------------------------------------
// <copyright file="ActionPromptEntity.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.ComponentModel.DataAnnotations.Schema;
using DonkeyWork.Chat.Common.Models.Prompt;

namespace DonkeyWork.Persistence.Agent.Entity.Prompt;

/// <summary>
/// A generic prompt entity.
/// </summary>
public class ActionPromptEntity : BasePromptEntity
{
    /// <summary>
    /// Gets or sets the prompt variables.
    /// </summary>
    [Column(TypeName = "jsonb")]
    public Dictionary<string, PromptVariable> Variables { get; set; } = [];

    /// <summary>
    /// Gets or sets the prompt messages.
    /// </summary>
    [Column(TypeName = "jsonb")]
    public List<PromptMessage> Messages { get; set; } = [];
}
