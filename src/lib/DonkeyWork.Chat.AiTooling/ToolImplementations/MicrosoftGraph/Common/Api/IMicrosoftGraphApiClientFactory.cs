// ------------------------------------------------------
// <copyright file="IMicrosoftGraphDriveApi.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using Microsoft.Graph;

namespace DonkeyWork.Chat.AiTooling.ToolImplementations.MicrosoftGraph.Common.Api;

/// <summary>
/// A Microsoft Graph drive tool.
/// </summary>
public interface IMicrosoftGraphApiClientFactory
{
    /// <summary>
    /// Searches the drive for files and folders.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task<GraphServiceClient> CreateGraphClientAsync(CancellationToken cancellationToken = default);
}