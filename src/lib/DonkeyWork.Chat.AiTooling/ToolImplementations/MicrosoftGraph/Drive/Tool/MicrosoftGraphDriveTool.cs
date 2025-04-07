// ------------------------------------------------------
// <copyright file="MicrosoftGraphDriveTool.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.ComponentModel;
using System.Text.Json;
using DonkeyWork.Chat.AiTooling.Attributes;
using DonkeyWork.Chat.AiTooling.ToolImplementations.MicrosoftGraph.Common;
using DonkeyWork.Chat.AiTooling.ToolImplementations.MicrosoftGraph.Common.Api;
using DonkeyWork.Chat.Common.Providers;
using Microsoft.Graph.Drives.Item.Items.Item.Copy;
using Microsoft.Graph.Drives.Item.Items.Item.CreateLink;
using Microsoft.Graph.Models;

namespace DonkeyWork.Chat.AiTooling.ToolImplementations.MicrosoftGraph.Drive.Tool;

/// <inheritdoc cref="DonkeyWork.Chat.AiTooling.ToolImplementations.MicrosoftGraph.Drive.Tool.IMicrosoftGraphDriveTool" />
[ToolProvider(UserProviderType.Microsoft)]
public class MicrosoftGraphDriveTool(
    IMicrosoftGraphApiClientFactory microsoftGraphApiClientFactory)
    : Base.Tool, IMicrosoftGraphDriveTool
{
    /// <inheritdoc />
    [ToolFunction]
    [Description("A tool to search specific Microsoft Graph drives.")]
    [ToolProviderScopes(
        UserProviderScopeHandleType.Any,
        "Files.Read",
        "Files.Read.All",
        "Files.ReadWrite",
        "Files.ReadWrite.All")]
    public async Task<JsonDocument?> SearchSpecificDriveAsync(
        [Description("The drive id that you wish to search. Consider listing the drives first.")]
        string driveId,
        [Description("Search terms that you wish to use the Microsoft Graph drives.")]
        string query,
        [ToolIgnoredParameter] CancellationToken cancellationToken = default)
    {
        var graphClient = await microsoftGraphApiClientFactory.CreateGraphClientAsync(cancellationToken);
        var searchResult = await graphClient.Drives[driveId].SearchWithQ(query)
            .GetAsSearchWithQGetResponseAsync(cancellationToken: cancellationToken);
        return JsonDocument.Parse(JsonSerializer.Serialize(searchResult, MicrosoftGraphSerializationOptions.MicrosoftGraphJsonSerializerOptions));
    }

    /// <inheritdoc />
    [ToolFunction]
    [Description("A tool to search the users Microsoft Graph drives.")]
    [ToolProviderScopes(
        UserProviderScopeHandleType.Any,
        "Files.Read",
        "Files.Read.All",
        "Files.ReadWrite",
        "Files.ReadWrite.All")]
    public async Task<JsonDocument?> SearchMyDriveAsync(
        [Description("Search terms that you wish to use the Microsoft Graph drives.")]
        string query,
        [ToolIgnoredParameter] CancellationToken cancellationToken = default)
    {
        var graphClient = await microsoftGraphApiClientFactory.CreateGraphClientAsync(cancellationToken);
        var myDrive = await graphClient.Me.Drive.GetAsync(cancellationToken: cancellationToken);
        var searchResult = await graphClient.Drives[myDrive?.Id]
            .SearchWithQ(query)
            .GetAsSearchWithQGetResponseAsync(cancellationToken: cancellationToken);
        return JsonDocument.Parse(JsonSerializer.Serialize(searchResult, MicrosoftGraphSerializationOptions.MicrosoftGraphJsonSerializerOptions));
    }

    /// <inheritdoc />
    [ToolFunction]
    [Description("A tool to list the Microsoft Graph drives the user has access to.")]
    [ToolProviderScopes(
        UserProviderScopeHandleType.Any,
        "Files.Read",
        "Files.Read.All",
        "Files.ReadWrite",
        "Files.ReadWrite.All")]
    public async Task<JsonDocument?> ListDrivesAsync(
        [ToolIgnoredParameter] CancellationToken cancellationToken = default)
    {
        var graphClient = await microsoftGraphApiClientFactory.CreateGraphClientAsync(cancellationToken);
        var result = await graphClient.Me.Drives.GetAsync(cancellationToken: cancellationToken);
        return JsonDocument.Parse(JsonSerializer.Serialize(result, MicrosoftGraphSerializationOptions.MicrosoftGraphJsonSerializerOptions));
    }

    /// <inheritdoc />
    [ToolFunction]
    [Description("A tool to Get a file thumbnails preview from the Microsoft Graph api.")]
    [ToolProviderScopes(
        UserProviderScopeHandleType.Any,
        "Files.Read",
        "Files.Read.All",
        "Files.ReadWrite",
        "Files.ReadWrite.All")]
    public async Task<JsonDocument?> GetFilePreviewAsync(
        [Description("The drive id from which you wish to get the file preview.")]
        string driveId,
        [Description("The file id that you wish to get the preview for.")]
        string fileId,
        [ToolIgnoredParameter] CancellationToken cancellationToken = default)
    {
        var graphClient = await microsoftGraphApiClientFactory.CreateGraphClientAsync(cancellationToken);
        var searchResult = await graphClient.Drives[driveId].Items[fileId]
            .Thumbnails.GetAsync(cancellationToken: cancellationToken);
        return JsonDocument.Parse(JsonSerializer.Serialize(searchResult?.Value, MicrosoftGraphSerializationOptions.MicrosoftGraphJsonSerializerOptions));
    }

    /// <inheritdoc />
    [ToolFunction]
    [ToolProviderScopes(
        UserProviderScopeHandleType.Any,
        "Files.ReadWrite",
        "Files.ReadWrite.All")]
    [Description("A tool to Get a public file sharing link from a Microsoft Graph drive file or folder.")]
    public async Task<JsonDocument?> GetPublicFileSharingLinkAsync(
        [Description("The drive id from which you wish to get the file preview.")]
        string driveId,
        [Description("The file id that you wish to get the preview for.")]
        string itemId,
        [ToolIgnoredParameter] CancellationToken cancellationToken = default)
    {
        var requestBody = new CreateLinkPostRequestBody
        {
            Type = "view",
            Scope = "anonymous",
            ExpirationDateTime = DateTimeOffset.UtcNow.AddDays(1),
            RetainInheritedPermissions = false,
        };

        var graphClient = await microsoftGraphApiClientFactory.CreateGraphClientAsync(cancellationToken);
        var result = await graphClient.Drives[driveId]
            .Items[itemId]
            .CreateLink.PostAsync(requestBody, cancellationToken: cancellationToken);
        return JsonDocument.Parse(JsonSerializer.Serialize(result, MicrosoftGraphSerializationOptions.MicrosoftGraphJsonSerializerOptions));
    }

    /// <inheritdoc />
    [ToolFunction]
    [ToolProviderScopes(
        UserProviderScopeHandleType.Any,
        "Files.ReadWrite",
        "Files.ReadWrite.All")]
    [Description("A tool to Create a folder in a Microsoft Graph drive.")]
    public async Task<JsonDocument?> CreateFolderInUserDriveAsync(
        [Description("The drive id from which you wish to get the file preview.")]
        string driveId,
        [Description("The parent id you wish to add the folder to.")]
        string parentId,
        [Description("The folder name that you wish to create.")]
        string folderName,
        [ToolIgnoredParameter] CancellationToken cancellationToken = default)
    {
        var graphClient = await microsoftGraphApiClientFactory.CreateGraphClientAsync(cancellationToken);
        var requestBody = new DriveItem()
        {
            Name = folderName,
            Folder = new Folder(),
            AdditionalData = new Dictionary<string, object>()
            {
                ["@microsoft.graph.conflictBehavior"] = "rename",
            },
        };
        var result = await graphClient.Drives[driveId].Items[parentId].Children
            .PostAsync(requestBody, cancellationToken: cancellationToken);
        return JsonDocument.Parse(JsonSerializer.Serialize(result, MicrosoftGraphSerializationOptions.MicrosoftGraphJsonSerializerOptions));
    }

    /// <inheritdoc />
    [ToolFunction]
    [ToolProviderScopes(
        UserProviderScopeHandleType.Any,
        "Files.ReadWrite",
        "Files.ReadWrite.All")]
    [Description("A tool to copy a file in a user's Microsoft Graph drive.")]
    public async Task<JsonDocument?> CopyItemInDriveAsync(
        [Description("The drive ID where the file exists.")]
        string driveId,
        [Description("The ID of the file you want to copy.")]
        string itemId,
        [Description("The new file name after copying.")]
        string newName,
        [Description("The ID of the destination folder.")]
        string destinationParentId,
        [ToolIgnoredParameter] CancellationToken cancellationToken = default)
    {
        var graphClient = await microsoftGraphApiClientFactory.CreateGraphClientAsync(cancellationToken);
        var requestBody = new CopyPostRequestBody()
        {
            Name = newName,
            ParentReference = new ItemReference()
            {
                Id = destinationParentId,
            },
        };

        await graphClient.Drives[driveId].Items[itemId]
            .Copy
            .PostAsync(requestBody, cancellationToken: cancellationToken);

        return JsonDocument.Parse("{\"status\":\"Copy initiated\"}");
    }

    /// <inheritdoc />
    [ToolFunction]
    [ToolProviderScopes(
        UserProviderScopeHandleType.Any,
        "Files.ReadWrite",
        "Files.ReadWrite.All")]
    [Description("A tool to move a file in a user's Microsoft Graph drive.")]
    public async Task<JsonDocument?> MoveItemInDriveAsync(
        [Description("The drive ID where the file exists.")]
        string driveId,
        [Description("The ID of the file to move.")]
        string itemId,
        [Description("The new file name (optional, pass same name to keep unchanged).")]
        string newName,
        [Description("The ID of the destination folder.")]
        string destinationParentId,
        [ToolIgnoredParameter] CancellationToken cancellationToken = default)
    {
        var graphClient = await microsoftGraphApiClientFactory.CreateGraphClientAsync(cancellationToken);

        var requestBody = new DriveItem
        {
            Name = newName,
            ParentReference = new ItemReference
            {
                Id = destinationParentId,
            },
        };

        var result = await graphClient.Drives[driveId].Items[itemId]
            .PatchAsync(requestBody, cancellationToken: cancellationToken);

        return JsonDocument.Parse(JsonSerializer.Serialize(result, MicrosoftGraphSerializationOptions.MicrosoftGraphJsonSerializerOptions));
    }

    /// <inheritdoc />
    [ToolFunction]
    [ToolProviderScopes(
        UserProviderScopeHandleType.Any,
        "Files.ReadWrite",
        "Files.ReadWrite.All")]
    [Description("A tool to delete a file in a user's Microsoft Graph drive.")]
    public async Task<JsonDocument?> DeleteItemInDriveAsync(
        [Description("The drive ID where the file exists.")]
        string driveId,
        [Description("The ID of the file to delete.")]
        string itemId,
        [ToolIgnoredParameter] CancellationToken cancellationToken = default)
    {
        var graphClient = await microsoftGraphApiClientFactory.CreateGraphClientAsync(cancellationToken);

        await graphClient.Drives[driveId].Items[itemId].DeleteAsync(cancellationToken: cancellationToken);

        return JsonDocument.Parse("{\"status\":\"File deleted successfully\"}");
    }

    /// <inheritdoc />
    [ToolFunction]
    [ToolProviderScopes(
        UserProviderScopeHandleType.Any,
        "Files.Read",
        "Files.Read.All",
        "Files.ReadWrite",
        "Files.ReadWrite.All")]
    [Description("A tool to list the contents of a folder in a user's Microsoft Graph drive, with optional pagination.")]
    public async Task<JsonDocument?> ListFolderContentsAsync(
        [Description("The drive ID containing the folder.")]
        string driveId,
        [Description("The ID of the folder to list contents from.")]
        string folderId,
        [Description("The maximum number of items to return (optional).")]
        int? maxCount = null,
        [Description("The number of items to skip before starting to return results (optional).")]
        int? skip = null,
        [ToolIgnoredParameter] CancellationToken cancellationToken = default)
    {
        var graphClient = await microsoftGraphApiClientFactory.CreateGraphClientAsync(cancellationToken);

        var result = await graphClient.Drives[driveId]
            .Items[folderId]
            .Children
            .GetAsync(
                requestConfig =>
            {
                if (maxCount.HasValue)
                {
                    requestConfig.QueryParameters.Top = maxCount.Value;
                }

                if (skip.HasValue)
                {
                    requestConfig.QueryParameters.Skip = skip.Value;
                }
            }, cancellationToken: cancellationToken);

        return JsonDocument.Parse(JsonSerializer.Serialize(result?.Value, MicrosoftGraphSerializationOptions.MicrosoftGraphJsonSerializerOptions));
    }
}