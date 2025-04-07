// ------------------------------------------------------
// <copyright file="IMicrosoftGraphDriveTool.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Text.Json;
using DonkeyWork.Chat.AiTooling.Base;

namespace DonkeyWork.Chat.AiTooling.ToolImplementations.MicrosoftGraph.Drive.Tool;

/// <summary>
/// An interface for the Microsoft Graph drive tool.
/// </summary>
public interface IMicrosoftGraphDriveTool : ITool
{
    /// <summary>
    /// Search a drive for a file.
    /// </summary>
    /// <param name="driveId">The drive id.</param>
    /// <param name="query">The query.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task<JsonDocument?> SearchSpecificDriveAsync(string driveId, string query, CancellationToken cancellationToken = default);

    /// <summary>
    /// Search my drive for a file.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task<JsonDocument?> SearchMyDriveAsync(
        string query,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists the Microsoft drives the user has access to.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task<JsonDocument?> ListDrivesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a file preview.
    /// </summary>
    /// <param name="driveId">The drive id.</param>
    /// <param name="fileId">The file id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task<JsonDocument?> GetFilePreviewAsync(
        string driveId,
        string fileId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a public file sharing link.
    /// </summary>
    /// <param name="driveId">The drive id.</param>
    /// <param name="itemId">The file id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task<JsonDocument?> GetPublicFileSharingLinkAsync(
        string driveId,
        string itemId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a folder in the users drive.
    /// </summary>
    /// <param name="driveId">The drive id.</param>
    /// <param name="parentId">The parent id.</param>
    /// <param name="folderName">The folder name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task<JsonDocument?> CreateFolderInUserDriveAsync(
        string driveId,
        string parentId,
        string folderName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Copys a file in the drive.
    /// </summary>
    /// <param name="driveId">The drive id.</param>
    /// <param name="itemId">The file id.</param>
    /// <param name="newName">The new file name.</param>
    /// <param name="destinationParentId">The destination parent id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<JsonDocument?> CopyItemInDriveAsync(
        string driveId,
        string itemId,
        string newName,
        string destinationParentId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Moves a file in the users drive.
    /// </summary>
    /// <param name="driveId">The drive id.</param>
    /// <param name="itemId">The file id.</param>
    /// <param name="newName">The new file name.</param>
    /// <param name="destinationParentId">The destination parent id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<JsonDocument?> MoveItemInDriveAsync(
        string driveId,
        string itemId,
        string newName,
        string destinationParentId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a file in the users drive.
    /// </summary>
    /// <param name="driveId">The drive id.</param>
    /// <param name="itemId">The file id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<JsonDocument?> DeleteItemInDriveAsync(
        string driveId,
        string itemId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists files in a folder with pagination.
    /// </summary>
    /// <param name="driveId">The drive id.</param>
    /// <param name="folderId">The folder id.</param>
    /// <param name="maxCount">The max count.</param>
    /// <param name="skip">The skip count.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<JsonDocument?> ListFolderContentsAsync(
        string driveId,
        string folderId,
        int? maxCount = null,
        int? skip = null,
        CancellationToken cancellationToken = default);
}