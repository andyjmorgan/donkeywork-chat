// ------------------------------------------------------
// <copyright file="SwarmpitClientFactory.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Net.Http.Headers;
using DonkeyWork.Chat.Common.Contracts;
using DonkeyWork.Chat.Common.Providers.GenericProvider;
using DonkeyWork.Chat.Common.Providers.GenericProvider.Implementations;

namespace DonkeyWork.Chat.AiTooling.ToolImplementations.Swarmpit.Api
{
    /// <summary>
    /// Factory for creating Swarmpit API clients.
    /// </summary>
    public class SwarmpitClientFactory
        : ISwarmpitClientFactory
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IUserPostureService userPostureService;
        private ISwarmpitClient? swarmpitClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="SwarmpitClientFactory"/> class.
        /// </summary>
        /// <param name="httpClientFactory">The HTTP client factory.</param>
        /// <param name="userPostureService">The user posture service.</param>
        public SwarmpitClientFactory(IHttpClientFactory httpClientFactory, IUserPostureService userPostureService)
        {
            this.httpClientFactory = httpClientFactory;
            this.userPostureService = userPostureService;
        }

        /// <inheritdoc/>
        public async Task<ISwarmpitClient> CreateClient()
        {
            if (this.swarmpitClient is not null)
            {
                return this.swarmpitClient;
            }

            var userPosture = await this.userPostureService.GetUserGenericPostureAsync(GenericProviderType.Swarmpit);
            if (userPosture is not { Configuration: SwarmpitConfiguration swarmpitConfiguration })
            {
                throw new InvalidOperationException("User posture is not set.");
            }

            var httpClient = this.httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", swarmpitConfiguration.ApiKey);
            httpClient.BaseAddress = new Uri(swarmpitConfiguration.BaseUrl);
            this.swarmpitClient = new SwarmpitClient(httpClient);
            return new SwarmpitClient(httpClient);
        }
    }
}