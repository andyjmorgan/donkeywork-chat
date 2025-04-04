// ------------------------------------------------------
// <copyright file="ToolProviderAttribute.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Providers;

namespace DonkeyWork.Chat.AiTooling.Attributes;

/// <summary>
/// An attribute that the provider of the tool.
/// </summary>
/// <para name="scopeIsAny">Determines if the required scope is Any or All. default is Any.</para>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class ToolProviderAttribute
    : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ToolProviderAttribute"/> class.
    /// </summary>
    /// <param name="providerType">The provider type.</param>
    public ToolProviderAttribute(UserProviderType providerType)
    {
        this.ProviderType = providerType;
    }

    /// <summary>
    /// Gets an attribute for the provider of the tool.
    /// </summary>
    public UserProviderType ProviderType { get; }
}