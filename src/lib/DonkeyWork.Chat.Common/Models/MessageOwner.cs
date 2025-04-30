// ------------------------------------------------------
// <copyright file="MessageOwner.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.Common.Models;

/// <summary>
/// Gets or sets the message owner.
/// </summary>
public enum MessageOwner
{
    /// <summary>
    /// An assistant message.
    /// </summary>
    Assistant,

    /// <summary>
    /// A user message.
    /// </summary>
    User,
}