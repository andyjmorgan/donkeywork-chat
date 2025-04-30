// ------------------------------------------------------
// <copyright file="GoogleDriveTool.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.ComponentModel;
using System.Text.Json;
using DonkeyWork.Chat.AiTooling.Attributes;
using DonkeyWork.Chat.AiTooling.ToolImplementations.GoogleApi.Common.Api;
using DonkeyWork.Chat.Common.Models.Providers.Tools;
using Google.Apis.Drive.v3.Data;
using Microsoft.Extensions.Logging;

namespace DonkeyWork.Chat.AiTooling.ToolImplementations.GoogleApi.Drive;

/// <inheritdoc cref="IGoogleDriveTool" />
[OAuthToolProvider(ToolProviderType.Google)]
[ToolProviderApplicationType(ToolProviderApplicationType.GoogleDrive)]
public class GoogleDriveTool(
    IGoogleApiClientFactory googleApiClientFactory,
    ILogger<GoogleDriveTool> logger)
    : Base.Tool(logger), IGoogleDriveTool
{
    private readonly JsonSerializerOptions jsonSerializerOptions = new ()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    /// <inheritdoc />
    [ToolFunction]
    [Description("A tool to list files in Google Drive with optional query parameters.")]
    [ToolProviderScopes(
        UserProviderScopeHandleType.Any,
        "https://www.googleapis.com/auth/drive.readonly",
        "https://www.googleapis.com/auth/drive")]
    public async Task<JsonDocument?> ListGoogleDriveFilesAsync(
        [Description("Search query using Google Drive query syntax.")]
        string? query = null,
        [Description("Maximum number of files to return.")]
        int? maxResults = null,
        [Description("Field to order results by (e.g., 'modifiedTime desc').")]
        string? orderBy = null,
        [ToolIgnoredParameter] CancellationToken cancellationToken = default)
    {
        var driveService = await googleApiClientFactory.CreateDriveServiceAsync(cancellationToken);
        var request = driveService.Files.List();

        request.PageSize = maxResults ?? 100;
        request.Fields = "files(id, name, mimeType, size, webViewLink, modifiedTime, createdTime, parents, shared)";

        if (!string.IsNullOrEmpty(query))
        {
            request.Q = query;
        }

        if (!string.IsNullOrEmpty(orderBy))
        {
            request.OrderBy = orderBy;
        }

        var result = await request.ExecuteAsync(cancellationToken);
        return JsonDocument.Parse(JsonSerializer.Serialize(result, this.jsonSerializerOptions));
    }

    /// <inheritdoc />
    [ToolFunction]
    [Description("A tool to get information about a specific file in Google Drive.")]
    [ToolProviderScopes(
        UserProviderScopeHandleType.Any,
        "https://www.googleapis.com/auth/drive.readonly",
        "https://www.googleapis.com/auth/drive")]
    public async Task<JsonDocument?> GetGoogleDriveFileAsync(
        [Description("The ID of the file to get information about.")]
        string fileId,
        [Description("The fields to include in the response (comma-separated list or * for all).")]
        string? fields = null,
        [ToolIgnoredParameter] CancellationToken cancellationToken = default)
    {
        var driveService = await googleApiClientFactory.CreateDriveServiceAsync(cancellationToken);
        var request = driveService.Files.Get(fileId);

        request.Fields = fields ??
                         "id, name, mimeType, size, webViewLink, modifiedTime, createdTime, parents, shared, description";

        var result = await request.ExecuteAsync(cancellationToken);
        return JsonDocument.Parse(JsonSerializer.Serialize(result, this.jsonSerializerOptions));
    }

    /// <inheritdoc />
    [ToolFunction]
    [Description("A tool to create a folder in Google Drive.")]
    [ToolProviderScopes(
        UserProviderScopeHandleType.Any,
        "https://www.googleapis.com/auth/drive")]
    public async Task<JsonDocument?> CreateGoogleDriveFolderAsync(
        [Description("The name for the new folder.")]
        string folderName,
        [Description("The ID of the parent folder (optional).")]
        string? parentId = null,
        [ToolIgnoredParameter] CancellationToken cancellationToken = default)
    {
        var driveService = await googleApiClientFactory.CreateDriveServiceAsync(cancellationToken);

        var folderMetadata = new Google.Apis.Drive.v3.Data.File
        {
            Name = folderName,
            MimeType = "application/vnd.google-apps.folder",
        };

        if (!string.IsNullOrEmpty(parentId))
        {
            folderMetadata.Parents = new List<string> { parentId };
        }

        var request = driveService.Files.Create(folderMetadata);
        request.Fields = "id, name, mimeType, webViewLink";

        var result = await request.ExecuteAsync(cancellationToken);
        return JsonDocument.Parse(JsonSerializer.Serialize(result, this.jsonSerializerOptions));
    }

    /// <inheritdoc />
    [ToolFunction]
    [Description("A tool to delete a file or folder from Google Drive.")]
    [ToolProviderScopes(
        UserProviderScopeHandleType.Any,
        "https://www.googleapis.com/auth/drive")]
    public async Task<JsonDocument?> DeleteGoogleDriveFileAsync(
        [Description("The ID of the file or folder to delete.")]
        string fileId,
        [ToolIgnoredParameter] CancellationToken cancellationToken = default)
    {
        var driveService = await googleApiClientFactory.CreateDriveServiceAsync(cancellationToken);
        await driveService.Files.Delete(fileId).ExecuteAsync(cancellationToken);

        return JsonDocument.Parse("{\"status\":\"File deleted successfully\"}");
    }

    /// <inheritdoc />
    [ToolFunction]
    [Description("A tool to copy a file in Google Drive.")]
    [ToolProviderScopes(
        UserProviderScopeHandleType.Any,
        "https://www.googleapis.com/auth/drive")]
    public async Task<JsonDocument?> CopyGoogleDriveFileAsync(
        [Description("The ID of the file to copy.")]
        string fileId,
        [Description("The new name for the copied file (optional).")]
        string? newName = null,
        [Description("The parent folder ID for the copied file (optional).")]
        string? parentId = null,
        [ToolIgnoredParameter] CancellationToken cancellationToken = default)
    {
        var driveService = await googleApiClientFactory.CreateDriveServiceAsync(cancellationToken);

        var copyMetadata = new Google.Apis.Drive.v3.Data.File();

        if (!string.IsNullOrEmpty(newName))
        {
            copyMetadata.Name = newName;
        }

        if (!string.IsNullOrEmpty(parentId))
        {
            copyMetadata.Parents = new List<string> { parentId };
        }

        var request = driveService.Files.Copy(copyMetadata, fileId);
        request.Fields = "id, name, mimeType, webViewLink";

        var result = await request.ExecuteAsync(cancellationToken);
        return JsonDocument.Parse(JsonSerializer.Serialize(result, this.jsonSerializerOptions));
    }

    /// <inheritdoc />
    [ToolFunction]
    [Description("A tool to move a file to a different folder in Google Drive.")]
    [ToolProviderScopes(
        UserProviderScopeHandleType.Any,
        "https://www.googleapis.com/auth/drive")]
    public async Task<JsonDocument?> MoveGoogleDriveFileAsync(
        [Description("The ID of the file to move.")]
        string fileId,
        [Description("The ID of the new parent folder.")]
        string newParentId,
        [Description("The ID of the current parent folder (optional).")]
        string? currentParentId = null,
        [ToolIgnoredParameter] CancellationToken cancellationToken = default)
    {
        var driveService = await googleApiClientFactory.CreateDriveServiceAsync(cancellationToken);

        // Get the file to determine the current parent if not provided
        if (string.IsNullOrEmpty(currentParentId))
        {
            var getRequest = driveService.Files.Get(fileId);
            getRequest.Fields = "parents";
            var fileMetadata = await getRequest.ExecuteAsync(cancellationToken);

            if (fileMetadata.Parents is { Count: > 0 })
            {
                currentParentId = fileMetadata.Parents[0];
            }
        }

        // Execute the update
        var updateRequest = driveService.Files.Update(new Google.Apis.Drive.v3.Data.File(), fileId);
        updateRequest.Fields = "id, name, parents";

        // Remove from previous parent and add to new parent
        if (!string.IsNullOrEmpty(currentParentId))
        {
            updateRequest.RemoveParents = currentParentId;
        }

        updateRequest.AddParents = newParentId;

        var result = await updateRequest.ExecuteAsync(cancellationToken);
        return JsonDocument.Parse(JsonSerializer.Serialize(result, this.jsonSerializerOptions));
    }

    /// <inheritdoc />
    [ToolFunction]
    [Description("A tool to rename a file in Google Drive.")]
    [ToolProviderScopes(
        UserProviderScopeHandleType.Any,
        "https://www.googleapis.com/auth/drive")]
    public async Task<JsonDocument?> RenameGoogleDriveFileAsync(
        [Description("The ID of the file to rename.")]
        string fileId,
        [Description("The new name for the file.")]
        string newName,
        [ToolIgnoredParameter] CancellationToken cancellationToken = default)
    {
        var driveService = await googleApiClientFactory.CreateDriveServiceAsync(cancellationToken);

        var fileMetadata = new Google.Apis.Drive.v3.Data.File
        {
            Name = newName,
        };

        var updateRequest = driveService.Files.Update(fileMetadata, fileId);
        updateRequest.Fields = "id, name";

        var result = await updateRequest.ExecuteAsync(cancellationToken);
        return JsonDocument.Parse(JsonSerializer.Serialize(result, this.jsonSerializerOptions));
    }

    /// <inheritdoc />
    [ToolFunction]
    [Description("A tool to create a sharing link for a file in Google Drive.")]
    [ToolProviderScopes(
        UserProviderScopeHandleType.Any,
        "https://www.googleapis.com/auth/drive")]
    public async Task<JsonDocument?> CreateGoogleDriveSharingLinkAsync(
        [Description("The ID of the file to share.")]
        string fileId,
        [Description("The role to grant (reader, writer, commenter).")]
        string role,
        [Description("The type of access (user, group, domain, anyone).")]
        string type,
        [Description("The email address to share with (optional, required for user or group).")]
        string? emailAddress = null,
        [ToolIgnoredParameter] CancellationToken cancellationToken = default)
    {
        var driveService = await googleApiClientFactory.CreateDriveServiceAsync(cancellationToken);

        var permission = new Permission
        {
            Type = type,
            Role = role,
        };

        if (type == "user" || type == "group")
        {
            if (string.IsNullOrEmpty(emailAddress))
            {
                throw new ArgumentException("Email address is required for user or group sharing");
            }

            permission.EmailAddress = emailAddress;
        }

        var createRequest = driveService.Permissions.Create(permission, fileId);
        createRequest.Fields = "id, type, role, emailAddress";

        var result = await createRequest.ExecuteAsync(cancellationToken);

        // Get the file to include sharing information
        var fileRequest = driveService.Files.Get(fileId);
        fileRequest.Fields = "id, name, webViewLink, shared";
        var fileInfo = await fileRequest.ExecuteAsync(cancellationToken);

        // Combine results
        var combinedResult = new
        {
            Permission = result,
            File = fileInfo,
        };

        return JsonDocument.Parse(JsonSerializer.Serialize(combinedResult, this.jsonSerializerOptions));
    }

    /// <inheritdoc />
    [ToolFunction]
    [Description("A tool to get the permissions for a file in Google Drive.")]
    [ToolProviderScopes(
        UserProviderScopeHandleType.Any,
        "https://www.googleapis.com/auth/drive.readonly",
        "https://www.googleapis.com/auth/drive")]
    public async Task<JsonDocument?> GetGoogleDriveFilePermissionsAsync(
        [Description("The ID of the file.")] string fileId,
        [ToolIgnoredParameter] CancellationToken cancellationToken = default)
    {
        var driveService = await googleApiClientFactory.CreateDriveServiceAsync(cancellationToken);

        var request = driveService.Permissions.List(fileId);
        request.Fields = "permissions(id, type, role, emailAddress, displayName)";

        var result = await request.ExecuteAsync(cancellationToken);
        return JsonDocument.Parse(JsonSerializer.Serialize(result, this.jsonSerializerOptions));
    }

    /// <inheritdoc />
    [ToolFunction]
    [Description("A tool to list the contents of a specific folder in Google Drive.")]
    [ToolProviderScopes(
        UserProviderScopeHandleType.Any,
        "https://www.googleapis.com/auth/drive.readonly",
        "https://www.googleapis.com/auth/drive")]
    public async Task<JsonDocument?> ListGoogleDriveFolderContentsAsync(
        [Description("The ID of the folder.")] string folderId,
        [Description("Maximum number of files to return.")]
        int? maxResults = null,
        [Description("Field to order results by (e.g., 'name', 'modifiedTime desc').")]
        string? orderBy = null,
        [ToolIgnoredParameter] CancellationToken cancellationToken = default)
    {
        var driveService = await googleApiClientFactory.CreateDriveServiceAsync(cancellationToken);

        var request = driveService.Files.List();
        request.Q = $"'{folderId}' in parents and trashed=false";
        request.PageSize = maxResults ?? 100;
        request.Fields = "files(id, name, mimeType, size, webViewLink, modifiedTime, createdTime)";

        if (!string.IsNullOrEmpty(orderBy))
        {
            request.OrderBy = orderBy;
        }

        var result = await request.ExecuteAsync(cancellationToken);
        return JsonDocument.Parse(JsonSerializer.Serialize(result, this.jsonSerializerOptions));
    }
}