// ------------------------------------------------------
// <copyright file="UserProviderScopeHandleType.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.Common.Models.Providers.Tools;

/// <summary>
/// A user provider scope handle type.
/// </summary>
public enum UserProviderScopeHandleType
{
    /// <summary>
    /// Any found scope is valid.
    /// </summary>
    Any,

    /// <summary>
    /// The user scopes must match the required scopes.
    /// </summary>
    All,
}