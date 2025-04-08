// ------------------------------------------------------
// <copyright file="MicrosoftGraphApiClientFactory.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Contracts;
using DonkeyWork.Chat.Common.Providers;
using Microsoft.Graph;

namespace DonkeyWork.Chat.AiTooling.ToolImplementations.MicrosoftGraph.Common.Api;

/// <summary>
/// The Microsoft Graph drive tool.
/// </summary>
public class MicrosoftGraphApiClientFactory : IMicrosoftGraphApiClientFactory
{
    private readonly HttpClient httpClient;
    private readonly IUserPostureService userPostureService;
    private GraphServiceClient? graphServiceClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="MicrosoftGraphApiClientFactory"/> class.
    /// </summary>
    /// <param name="httpClientFactory">The http client factory.</param>
    /// <param name="userPostureService">The user posture service.</param>
    public MicrosoftGraphApiClientFactory(IHttpClientFactory httpClientFactory, IUserPostureService userPostureService)
    {
        this.httpClient = httpClientFactory.CreateClient(nameof(UserProviderType.Microsoft));
        this.userPostureService = userPostureService;
    }

    /// <inheritdoc />
    public async Task<GraphServiceClient> CreateGraphClientAsync(CancellationToken cancellationToken = default)
    {
        if (this.graphServiceClient != null)
        {
            return this.graphServiceClient;
        }

        var userPosture = await this.userPostureService.GetUserPostureAsync(UserProviderType.Microsoft, cancellationToken);
        ArgumentNullException.ThrowIfNull(userPosture);
        this.graphServiceClient = new GraphServiceClient(
            this.httpClient,
            new UserOAuthTokenProvider(userPosture.Keys[UserProviderDataKeyType.AccessToken]));
        return this.graphServiceClient;
    }
}