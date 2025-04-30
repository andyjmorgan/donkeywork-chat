// ------------------------------------------------------
// <copyright file="ToolProviderApplicationTypeAttribute.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Models.Providers.Tools;

namespace DonkeyWork.Chat.AiTooling.Attributes;

/// <summary>
/// An attribute that the provider of the tool.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class ToolProviderApplicationTypeAttribute
    : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ToolProviderApplicationTypeAttribute"/> class.
    /// </summary>
    /// <param name="providerType">The provider type.</param>
    public ToolProviderApplicationTypeAttribute(ToolProviderApplicationType providerType)
    {
        this.ProviderApplicationType = providerType;
    }

    /// <summary>
    /// Gets an attribute for the provider of the tool.
    /// </summary>
    public ToolProviderApplicationType ProviderApplicationType { get; }
}