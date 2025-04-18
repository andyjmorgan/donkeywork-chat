// ------------------------------------------------------
// <copyright file="CookieOrApiKeyAuthAttribute.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Api.Core.AuthenticationSchemes;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace DonkeyWork.Chat.Api.Core.Attributes;

/// <summary>
/// Attribute to require either cookie or API key authentication.
/// </summary>
public class CookieOrApiKeyAuthAttribute : AuthorizeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CookieOrApiKeyAuthAttribute"/> class.
    /// </summary>
    public CookieOrApiKeyAuthAttribute()
    {
        this.AuthenticationSchemes = $"{CookieAuthenticationDefaults.AuthenticationScheme},{ApiKeyAuthenticationOptions.DefaultScheme}";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CookieOrApiKeyAuthAttribute"/> class with specified roles.
    /// </summary>
    /// <param name="roles">The roles that are authorized to access the resource.</param>
    public CookieOrApiKeyAuthAttribute(string roles)
        : this()
    {
        this.Roles = roles;
    }
}