// ------------------------------------------------------
// <copyright file="ApiUserMiddleware.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Security.Claims;
using DonkeyWork.Chat.Common.Services.UserContext;
using Microsoft.AspNetCore.Http;

namespace DonkeyWork.Chat.Api.Core.Middleware;

/// <summary>
/// A user middleware to capture the user id.
/// </summary>
public class ApiUserMiddleware
{
    /// <summary>
    /// The User ID key.
    /// </summary>
    public const string TenantIdKey = "TenantId";

    /// <summary>
    /// The next middleware in the pipeline.
    /// </summary>
    private readonly RequestDelegate next;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiUserMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware.</param>
    public ApiUserMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    /// <summary>
    /// Invokes the middleware.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="userContextProvider">The user context provider.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task InvokeAsync(HttpContext context, IUserContextProvider userContextProvider)
    {
        if (context.Request.Path.StartsWithSegments("/api"))
        {
            if (context.User.Identity is not null && context.User.Identity.IsAuthenticated)
            {
                var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim is not null)
                {
                    if (Guid.TryParse(userIdClaim.Value, out var userId))
                    {
                        userContextProvider.SetUserId(userId);
                    }
                }
            }
        }

        await this.next(context);
    }
}