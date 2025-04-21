// ------------------------------------------------------
// <copyright file="TextContent.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.Common.Models.Prompt.Content;

/// <summary>
/// A text content entity.
/// </summary>
public class TextContent : BaseContent
{
    /// <summary>
    /// Gets or sets the content of the text content entity.
    /// </summary>
    public string Text { get; set; } = string.Empty;
}