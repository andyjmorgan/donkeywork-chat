// ------------------------------------------------------
// <copyright file="ResourceContentEntity.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.Common.Models.Prompt.Content;

/// <summary>
/// A resource content entity.
/// https://modelcontextprotocol.io/docs/concepts/prompts.
/// </summary>
public class ResourceContent : BaseContent
{
    /// <summary>
    /// Gets or sets the content of the resource content entity.
    /// </summary>
    required public ResourceItem Resource { get; set; }
}