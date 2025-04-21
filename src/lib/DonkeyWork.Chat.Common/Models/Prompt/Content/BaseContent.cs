// ------------------------------------------------------
// <copyright file="BaseContent.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Text.Json.Serialization;

namespace DonkeyWork.Chat.Common.Models.Prompt.Content;

/// <summary>
/// A base class for prompt content entities.
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = ContentTypeConstants.DiscriminatorPropertyName)]
[JsonDerivedType(typeof(ResourceContent), typeDiscriminator: ContentTypeConstants.ResourceType)]
[JsonDerivedType(typeof(TextContent), typeDiscriminator: ContentTypeConstants.TextType)]
public abstract class BaseContent
{
}