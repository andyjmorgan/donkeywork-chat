// ------------------------------------------------------
// <copyright file="GenericMessageRole.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.AiServices.Clients.Models;

/// <summary>
/// A generic chat message role.
/// </summary>
public enum GenericMessageRole
{
    /// <summary>
    /// A system message.
    /// </summary>
    System,

    /// <summary>
    /// An assistant message.
    /// </summary>
    Assistant,

    /// <summary>
    /// A user message.
    /// </summary>
    User,
}