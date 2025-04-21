// ------------------------------------------------------
// <copyright file="ContentTypeConstants.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.Common.Models.Prompt;

/// <summary>
/// A static class containing constants for content types.
/// </summary>
public static class ContentTypeConstants
{
    /// <summary>
    /// A static string representing the discriminator property name.
    /// </summary>
    public const string DiscriminatorPropertyName = "type";

    /// <summary>
    /// A static string representing the resource type.
    /// </summary>
    public const string ResourceType = "resource";

    /// <summary>
    /// A static string representing the text type.
    /// </summary>
    public const string TextType = "text";
}