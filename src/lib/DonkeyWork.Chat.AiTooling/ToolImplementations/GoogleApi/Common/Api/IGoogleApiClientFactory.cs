// ------------------------------------------------------
// <copyright file="IGoogleApiClientFactory.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using Google.Apis.Drive.v3;
using Google.Apis.Gmail.v1;

namespace DonkeyWork.Chat.AiTooling.ToolImplementations.GoogleApi.Common.Api;

/// <summary>
/// Interface for Google API client factory that provides access to Google services.
/// </summary>
public interface IGoogleApiClientFactory
{
    /// <summary>
    /// Creates a Gmail service instance for interacting with the Gmail API.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a configured <see cref="GmailService"/> instance.</returns>
    Task<GmailService> CreateGmailServiceAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a Drive service instance for interacting with the Google Drive API.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a configured <see cref="DriveService"/> instance.</returns>
    Task<DriveService> CreateDriveServiceAsync(CancellationToken cancellationToken = default);
}