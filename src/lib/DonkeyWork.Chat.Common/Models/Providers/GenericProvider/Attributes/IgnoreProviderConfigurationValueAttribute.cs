// ------------------------------------------------------
// <copyright file="IgnoreProviderConfigurationValueAttribute.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.Common.Models.Providers.GenericProvider.Attributes;

/// <summary>
/// An ignored tool parameter attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter, Inherited = false, AllowMultiple = false)]
public class IgnoreProviderConfigurationValueAttribute : Attribute
{
    
}
public sealed class ToolIgnoredParameterAttribute
{
}