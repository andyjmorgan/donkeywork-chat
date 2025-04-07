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
    public MicrosoftGraphApiClientFactory(IHttpClientFactory httpClientFactory, IUserPostureService userPostureService)
    {
        this.httpClient = httpClientFactory.CreateClient("MicrosoftGraph");
        this.userPostureService = userPostureService;
    }

    /// <inheritdoc />
    public async Task<GraphServiceClient> CreateGraphClientAsync(CancellationToken cancellationToken = default)
    {
        if (this.graphServiceClient == null)
        {
            var userPosture = await this.userPostureService.GetUserPosturesAsync(cancellationToken);
            var thisProvider = userPosture.FirstOrDefault(x => x.ProviderType == UserProviderType.Microsoft);
            ArgumentNullException.ThrowIfNull(thisProvider);
            this.graphServiceClient = new GraphServiceClient(this.httpClient, new UserOAuthTokenProvider(thisProvider.Keys[UserProviderDataKeyType.AccessToken]));
        }

        return this.graphServiceClient;
    }
}