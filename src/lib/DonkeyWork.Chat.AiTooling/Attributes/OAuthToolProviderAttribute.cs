// ------------------------------------------------------
// <copyright file="OAuthToolProviderAttribute.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Models.Providers.Tools;

namespace DonkeyWork.Chat.AiTooling.Attributes;

/// <summary>
/// An attribute that the provider of the tool.
/// </summary>
/// <para name="scopeIsAny">Determines if the required scope is Any or All. default is Any.</para>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class OAuthToolProviderAttribute
    : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OAuthToolProviderAttribute"/> class.
    /// </summary>
    /// <param name="providerType">The provider type.</param>
    public OAuthToolProviderAttribute(ToolProviderType providerType)
    {
        this.ProviderType = providerType;
    }

    /// <summary>
    /// Gets an attribute for the provider of the tool.
    /// </summary>
    public ToolProviderType ProviderType { get; }
}