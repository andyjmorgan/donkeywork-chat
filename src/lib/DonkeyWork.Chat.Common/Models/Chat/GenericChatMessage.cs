// ------------------------------------------------------
// <copyright file="GenericChatMessage.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Text.Json.Serialization;

namespace DonkeyWork.Chat.Common.Models.Chat;

/// <summary>
/// A generic chat completion request.
/// </summary>
public class GenericChatMessage
{
    /// <summary>
    /// Gets the role.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public GenericMessageRole Role { get; init; }

    /// <summary>
    /// Gets the message content.
    /// </summary>
    required public string Content { get; init; }
}
