// ------------------------------------------------------
// <copyright file="ToolCallEntity.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using DonkeyWork.Chat.Persistence.Entity.Base;

// ReSharper disable EntityFramework.ModelValidation.UnlimitedStringLength
namespace DonkeyWork.Chat.Persistence.Entity.Conversation;

/// <summary>
/// A tool call entity.
/// </summary>
public class ToolCallEntity : BaseUserEntity
{
    /// <summary>
    /// Gets the conversation Id.
    /// </summary>
    public Guid ConversationId { get; init; }

    /// <summary>
    /// Gets the message pair id.
    /// </summary>
    public Guid MessagePairId { get; init; }

    /// <summary>
    /// Gets the tool name.
    /// </summary>
    [MaxLength(128)]
    public string ToolName { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the json string tool request.
    /// </summary>
    public string Request { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the json string tool response.
    /// </summary>
    public string Response { get; set; } = string.Empty;

    /// <summary>
    /// Gets the conversation.
    /// </summary>
    required public virtual ConversationEntity Conversation { get; init; }
}