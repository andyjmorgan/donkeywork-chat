// ------------------------------------------------------
// <copyright file="ActionPromptMessageModel.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Models;

namespace DonkeyWork.Chat.Api.Models.Prompt;

/// <summary>
/// A model representing a prompt message for an action.
/// </summary>
public class ActionPromptMessageModel
{
    /// <summary>
    /// Gets or sets the unique identifier of the prompt message.
    /// </summary>
    public MessageOwner Role { get; set; }

    /// <summary>
    /// Gets or sets the content of the prompt message.
    /// </summary>
    public string Content { get; set; } = string.Empty;
}
