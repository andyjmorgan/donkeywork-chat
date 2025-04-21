// ------------------------------------------------------
// <copyright file="ResourceItem.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.Common.Models.Prompt.Content;

/// <summary>
/// A resource item entity.
/// </summary>
public class ResourceItem
{
    /// <summary>
    /// Gets or sets the uri value.
    /// </summary>
    public string Uri { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the text value.
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// gets or sets the mime type value.
    /// </summary>
    public string MimeType { get; set; } = string.Empty;
}