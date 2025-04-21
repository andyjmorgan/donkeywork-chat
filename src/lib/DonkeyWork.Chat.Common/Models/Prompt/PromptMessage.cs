// ------------------------------------------------------
// <copyright file="PromptMessage.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Models.Prompt.Content;

namespace DonkeyWork.Chat.Common.Models.Prompt;

/// <summary>
/// A prompt message entity.
/// </summary>
public class PromptMessage
{
    /// <summary>
    /// Gets or sets the unique identifier of the prompt message.
    /// </summary>
    public MessageOwner Role { get; set; }

    /// <summary>
    /// Gets or sets the content of the prompt message.
    /// </summary>
    required public BaseContent Content { get; set; }
}