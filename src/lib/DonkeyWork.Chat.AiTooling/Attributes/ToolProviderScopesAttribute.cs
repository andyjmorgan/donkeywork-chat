// ------------------------------------------------------
// <copyright file="ToolProviderScopesAttribute.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Models.Providers.Tools;

namespace DonkeyWork.Chat.AiTooling.Attributes;

/// <summary>
/// An attribute that the provider of the tool.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public sealed class ToolProviderScopesAttribute
    : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ToolProviderScopesAttribute"/> class.
    /// </summary>
    /// <param name="handleType">The handle type.</param>
    /// <param name="scopes">The scopes.</param>
    public ToolProviderScopesAttribute(UserProviderScopeHandleType handleType, params string[] scopes)
    {
        this.ScopeHandleType = handleType;
        this.Scopes = scopes;
    }

    /// <summary>
    /// Gets the scopes required to use the tool.
    /// </summary>
    public string[] Scopes { get; }

    /// <summary>
    /// Gets a value indicating whether the required scope is Any or All.
    /// </summary>
    public UserProviderScopeHandleType ScopeHandleType { get; }
}