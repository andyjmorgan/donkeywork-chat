// ------------------------------------------------------
// <copyright file="IGoogleDriveTool.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Text.Json;
using DonkeyWork.Chat.AiTooling.Base;

namespace DonkeyWork.Chat.AiTooling.ToolImplementations.GoogleApi.Drive;

/// <summary>
/// An interface for the Google Drive tool.
/// </summary>
public interface IGoogleDriveTool : ITool
{
    /// <summary>
    /// Lists files in Google Drive with optional query parameters.
    /// </summary>
    /// <param name="query">Search query using Google Drive query syntax.</param>
    /// <param name="maxResults">Maximum number of files to return.</param>
    /// <param name="orderBy">Field to order results by.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task<JsonDocument?> ListGoogleDriveFilesAsync(
        string? query = null,
        int? maxResults = null,
        string? orderBy = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets information about a specific file.
    /// </summary>
    /// <param name="fileId">The file ID.</param>
    /// <param name="fields">The fields to include in the response.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task<JsonDocument?> GetGoogleDriveFileAsync(
        string fileId,
        string? fields = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a folder in Google Drive.
    /// </summary>
    /// <param name="folderName">The name for the folder.</param>
    /// <param name="parentId">The parent folder ID (optional).</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task<JsonDocument?> CreateGoogleDriveFolderAsync(
        string folderName,
        string? parentId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a file or folder from Google Drive.
    /// </summary>
    /// <param name="fileId">The ID of the file or folder to delete.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task<JsonDocument?> DeleteGoogleDriveFileAsync(
        string fileId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Copies a file in Google Drive.
    /// </summary>
    /// <param name="fileId">The ID of the file to copy.</param>
    /// <param name="newName">The new name for the copied file (optional).</param>
    /// <param name="parentId">The parent folder ID for the copied file (optional).</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task<JsonDocument?> CopyGoogleDriveFileAsync(
        string fileId,
        string? newName = null,
        string? parentId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Moves a file to a different folder in Google Drive.
    /// </summary>
    /// <param name="fileId">The ID of the file to move.</param>
    /// <param name="newParentId">The ID of the new parent folder.</param>
    /// <param name="currentParentId">The ID of the current parent folder (optional).</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task<JsonDocument?> MoveGoogleDriveFileAsync(
        string fileId,
        string newParentId,
        string? currentParentId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Renames a file in Google Drive.
    /// </summary>
    /// <param name="fileId">The ID of the file to rename.</param>
    /// <param name="newName">The new name for the file.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task<JsonDocument?> RenameGoogleDriveFileAsync(
        string fileId,
        string newName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a sharing link for a file.
    /// </summary>
    /// <param name="fileId">The ID of the file to share.</param>
    /// <param name="role">The role to grant (reader, writer, commenter).</param>
    /// <param name="type">The type of access (user, group, domain, anyone).</param>
    /// <param name="emailAddress">The email address to share with (optional).</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task<JsonDocument?> CreateGoogleDriveSharingLinkAsync(
        string fileId,
        string role,
        string type,
        string? emailAddress = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the permissions for a file.
    /// </summary>
    /// <param name="fileId">The ID of the file.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task<JsonDocument?> GetGoogleDriveFilePermissionsAsync(
        string fileId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists the contents of a specific folder.
    /// </summary>
    /// <param name="folderId">The ID of the folder.</param>
    /// <param name="maxResults">Maximum number of files to return.</param>
    /// <param name="orderBy">Field to order results by.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task<JsonDocument?> ListGoogleDriveFolderContentsAsync(
        string folderId,
        int? maxResults = null,
        string? orderBy = null,
        CancellationToken cancellationToken = default);
}