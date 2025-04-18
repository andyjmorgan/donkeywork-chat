// ------------------------------------------------------
// <copyright file="ISwarmpitClientFactory.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.AiTooling.ToolImplementations.Swarmpit.Api
{
    /// <summary>
    /// Factory for creating Swarmpit API clients.
    /// </summary>
    public interface ISwarmpitClientFactory
    {
        /// <summary>
        /// Creates a Swarmpit API client.
        /// </summary>
        /// <returns>A client for accessing the Swarmpit API.</returns>
        public Task<ISwarmpitClient> CreateClient();
    }
}