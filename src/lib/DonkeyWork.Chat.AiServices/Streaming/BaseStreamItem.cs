// ------------------------------------------------------
// <copyright file="BaseStreamItem.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Text.Json.Serialization;
using DonkeyWork.Chat.AiServices.Streaming.Chat;
using DonkeyWork.Chat.AiServices.Streaming.Exceptions;
using DonkeyWork.Chat.AiServices.Streaming.Request;
using DonkeyWork.Chat.AiServices.Streaming.Tool;

namespace DonkeyWork.Chat.AiServices.Streaming;

/// <summary>
/// A base stream item.
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "MessageType")]
[JsonDerivedType(typeof(ChatFragment), typeDiscriminator: nameof(ChatFragment))]
[JsonDerivedType(typeof(ChatStartFragment), typeDiscriminator: nameof(ChatStartFragment))]
[JsonDerivedType(typeof(ChatEndFragment), typeDiscriminator: nameof(ChatEndFragment))]
[JsonDerivedType(typeof(RequestStart), typeDiscriminator: nameof(RequestStart))]
[JsonDerivedType(typeof(RequestEnd), typeDiscriminator: nameof(RequestEnd))]
[JsonDerivedType(typeof(ToolCall), typeDiscriminator: nameof(ToolCall))]
[JsonDerivedType(typeof(ToolResult), typeDiscriminator: nameof(ToolResult))]
[JsonDerivedType(typeof(ExceptionResult), typeDiscriminator: nameof(ExceptionResult))]
[JsonDerivedType(typeof(TokenUsage), typeDiscriminator: nameof(TokenUsage))]
public abstract record BaseStreamItem
{
    /// <summary>
    /// Gets the execution id.
    /// </summary>
    public Guid ExecutionId { get; init; }
}