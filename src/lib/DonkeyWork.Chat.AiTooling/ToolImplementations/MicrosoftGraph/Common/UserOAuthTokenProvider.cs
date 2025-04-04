// ------------------------------------------------------
// <copyright file="UserOAuthTokenProvider.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;

namespace DonkeyWork.Chat.AiTooling.ToolImplementations.MicrosoftGraph.Common;

/// <summary>
/// An oauth token provider for a user OAuth Token.
/// </summary>
public class UserOAuthTokenProvider : IAuthenticationProvider
{
    private readonly string accessToken;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserOAuthTokenProvider"/> class.
    /// </summary>
    /// <param name="accessToken">The access Token.</param>
    public UserOAuthTokenProvider(string accessToken)
    {
        this.accessToken = accessToken;
    }

    /// <inheritdoc />
    public Task AuthenticateRequestAsync(
        RequestInformation request,
        Dictionary<string, object>? additionalAuthenticationContext = null,
        CancellationToken cancellationToken = default)
    {
        request.Headers.Add("Authorization", $"Bearer {this.accessToken}");
        return Task.CompletedTask;
    }
}