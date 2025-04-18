// ------------------------------------------------------
// <copyright file="ConversationEntity.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using DonkeyWork.Chat.Persistence.Entity.Base;

namespace DonkeyWork.Chat.Persistence.Entity.Conversation;

/// <summary>
/// A conversation entity.
/// </summary>
public class ConversationEntity : BaseUserEntity
{
    /// <summary>
    /// Gets or sets the conversation title.
    /// </summary>
    [MaxLength(64)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the message entities.
    /// </summary>
    public virtual ICollection<ConversationMessageEntity> MessageEntities { get; set; } = [];

    /// <summary>
    /// Gets or sets the tool call entities.
    /// </summary>
    public virtual ICollection<ToolCallEntity> ToolCallEntities { get; set; } = [];
}