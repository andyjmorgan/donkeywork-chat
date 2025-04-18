// ------------------------------------------------------
// <copyright file="ApiKeyAuthAttribute.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Api.Core.AuthenticationSchemes;
using Microsoft.AspNetCore.Authorization;

namespace DonkeyWork.Chat.Api.Core.Attributes;

/// <summary>
/// Attribute to require API key authentication.
/// </summary>
public class ApiKeyAuthAttribute : AuthorizeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ApiKeyAuthAttribute"/> class.
    /// </summary>
    public ApiKeyAuthAttribute()
    {
        this.AuthenticationSchemes = ApiKeyAuthenticationOptions.DefaultScheme;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiKeyAuthAttribute"/> class with specified roles.
    /// </summary>
    /// <param name="roles">The roles that are authorized to access the resource.</param>
    public ApiKeyAuthAttribute(string roles)
        : this()
    {
        this.Roles = roles;
    }
}