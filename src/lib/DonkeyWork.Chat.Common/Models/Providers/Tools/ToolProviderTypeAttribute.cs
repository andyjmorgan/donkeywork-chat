// ------------------------------------------------------
// <copyright file="ToolProviderTypeAttribute.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.Common.Models.Providers.Tools;

/// <summary>
/// Used to mark a field as a tool provider.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class ToolProviderTypeAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ToolProviderTypeAttribute"/> class.
    /// </summary>
    /// <param name="provider">The provider.</param>
    public ToolProviderTypeAttribute(ToolProviderType provider)
    {
        this.Provider = provider;
    }

    /// <summary>
    /// Gets the tool provider.
    /// </summary>
    public ToolProviderType Provider { get; }
}
