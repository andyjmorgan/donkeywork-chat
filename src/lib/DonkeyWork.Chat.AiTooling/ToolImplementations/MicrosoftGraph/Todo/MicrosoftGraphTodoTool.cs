// ------------------------------------------------------
// <copyright file="MicrosoftGraphTodoTool.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.ComponentModel;
using System.Text.Json;
using DonkeyWork.Chat.AiTooling.Attributes;
using DonkeyWork.Chat.AiTooling.ToolImplementations.MicrosoftGraph.Common;
using DonkeyWork.Chat.AiTooling.ToolImplementations.MicrosoftGraph.Common.Api;
using DonkeyWork.Chat.Common.Providers;
using Microsoft.Graph.Models;
using TaskStatus = Microsoft.Graph.Models.TaskStatus;

namespace DonkeyWork.Chat.AiTooling.ToolImplementations.MicrosoftGraph.Todo;

/// <summary>
/// A Microsoft Graph Todo tool.
/// </summary>
[ToolProvider(UserProviderType.Microsoft)]
public class MicrosoftGraphTodoTool(IMicrosoftGraphApiClientFactory microsoftGraphApiClientFactory)
    : Base.Tool, IMicrosoftGraphTodoTool
{
    /// <param name="cancellationToken"></param>
    /// <inheritdoc />
    [ToolFunction]
    [Description("Get all To Do lists for the signed-in user using the Microsoft Graph Api.")]
    [ToolProviderScopes(UserProviderScopeHandleType.Any,  "Tasks.Read", "Tasks.ReadWrite")]
    public async Task<JsonDocument> GetMicrosoftGraphTodoListsAsync(
        [ToolIgnoredParameter]
        CancellationToken cancellationToken = default)
    {
        var client = await microsoftGraphApiClientFactory.CreateGraphClientAsync(cancellationToken);
        var result = await client.Me.Todo.Lists
            .GetAsync(cancellationToken: cancellationToken);

        return JsonDocument.Parse(JsonSerializer.Serialize(result, MicrosoftGraphSerializationOptions.MicrosoftGraphJsonSerializerOptions));
    }

    /// <inheritdoc />
    [ToolFunction]
    [Description("Create a new To Do list using the Microsoft Graph Api.")]
    [ToolProviderScopes(UserProviderScopeHandleType.Any, "Tasks.ReadWrite")]
    public async Task<JsonDocument> CreateMicrosoftGraphTodoListAsync(
        [Description("The display name of the todo list. Keep it short")]
        string displayName,
        [ToolIgnoredParameter]
        CancellationToken cancellationToken = default)
    {
        var client = await microsoftGraphApiClientFactory.CreateGraphClientAsync(cancellationToken);
        var list = new TodoTaskList
        {
            DisplayName = displayName,
        };

        var result = await client.Me.Todo.Lists.PostAsync(list, cancellationToken: cancellationToken);
        return JsonDocument.Parse(JsonSerializer.Serialize(result, MicrosoftGraphSerializationOptions.MicrosoftGraphJsonSerializerOptions));
    }

    /// <inheritdoc />
    [ToolFunction]
    [Description("Get all tasks from a specific To Do list using the Microsoft Graph Api.")]
    [ToolProviderScopes(UserProviderScopeHandleType.Any,  "Tasks.Read", "Tasks.ReadWrite")]
    public async Task<JsonDocument> GetMicrosoftGraphTasksAsync(
        [Description("The ID of the To Do list.")]
        string listId,
        CancellationToken cancellationToken = default)
    {
        var client = await microsoftGraphApiClientFactory.CreateGraphClientAsync(cancellationToken);
        var result = await client.Me.Todo.Lists[listId].Tasks
            .GetAsync(cancellationToken: cancellationToken);

        return JsonDocument.Parse(JsonSerializer.Serialize(result, MicrosoftGraphSerializationOptions.MicrosoftGraphJsonSerializerOptions));
    }

    /// <inheritdoc />
    [ToolFunction]
    [Description("Deletes a task from a To Do list using the Microsoft Graph Api.")]
    public async Task<JsonDocument> DeleteMicrosoftGraphTaskAsync(
        [Description("The ID of the To Do list.")]
        string listId,
        [Description("The ID of the To Do task.")]
        string taskId,
        CancellationToken cancellationToken = default)
    {
        var client = await microsoftGraphApiClientFactory.CreateGraphClientAsync(cancellationToken);
        await client.Me.Todo.Lists[listId].Tasks[taskId].DeleteAsync(cancellationToken: cancellationToken);
        return JsonDocument.Parse(JsonSerializer.Serialize(new
        {
            success = true,
        }));
    }

    /// <inheritdoc />
    [ToolFunction]
    [Description("Create a new task in a specified To Do list using the Microsoft Graph Api.")]
    [ToolProviderScopes(UserProviderScopeHandleType.Any,  "Tasks.Read", "Tasks.ReadWrite")]
    public async Task<JsonDocument> CreateMicrosoftGraphTaskAsync(
        [Description("The ID of the To Do list.")]
        string listId,
        [Description("The title of the task.")]
        string title,
        [Description("The categories of the task.")]
        List<string>? categories = null,
        [Description("The body of the task.")]
        string? body = null,
        [Description("The importance of the task.")]
        Importance? importance = null,
        [Description("The status")]
        TaskStatus? status = null,
        [Description("The due date of the task. Optional. A single point of time in a combined date and time representation ({date}T{time}; for example, 2017-08-29T04:00:00.0000000).")]
        string? dueDate = null,
        CancellationToken cancellationToken = default)
    {
        var client = await microsoftGraphApiClientFactory.CreateGraphClientAsync(cancellationToken);
        var task = new TodoTask
        {
            Title = title,
            Importance = importance,
            Status = status,
            Body = string.IsNullOrWhiteSpace(body) ? null : new ItemBody
            {
                Content = body,
                ContentType = BodyType.Text,
            },
            Categories = categories?.Count > 0 ? categories : [],
            DueDateTime = !string.IsNullOrWhiteSpace(dueDate) ? new DateTimeTimeZone()
                {
                    DateTime = dueDate,
                    TimeZone = "UTC",
                }
                : null,
        };

        var result = await client.Me.Todo.Lists[listId].Tasks
            .PostAsync(task, cancellationToken: cancellationToken);
        return JsonDocument.Parse(JsonSerializer.Serialize(result, MicrosoftGraphSerializationOptions.MicrosoftGraphJsonSerializerOptions));
    }

    /// <inheritdoc />
    [ToolFunction]
    [Description("Updates an existing task in a specified To Do list using the Microsoft Graph Api.")]
    [ToolProviderScopes(UserProviderScopeHandleType.Any,  "Tasks.Read", "Tasks.ReadWrite")]
    public async Task<JsonDocument> UpdateMicrosoftGraphTaskAsync(
        [Description("The ID of the To Do list.")]
        string listId,
        [Description("The ID of the To Do task.")]
        string taskId,
        [Description("The title of the task.")]
        string? title = null,
        [Description("The categories of the task.")]
        List<string>? categories = null,
        [Description("The body of the task.")]
        string? body = null,
        [Description("The importance of the task.")]
        Importance? importance = null,
        [Description("The status")]
        TaskStatus? status = null,
        [Description("The due date of the task. Optional. A single point of time in a combined date and time representation ({date}T{time}; for example, 2017-08-29T04:00:00.0000000).")]
        string? dueDate = null,
        CancellationToken cancellationToken = default)
    {
        var client = await microsoftGraphApiClientFactory.CreateGraphClientAsync(cancellationToken);
        var task = new TodoTask
        {
            Title = title,
            Status = status,
            Categories = categories,
            Importance = importance,
            Body = string.IsNullOrWhiteSpace(body) ? null : new ItemBody
            {
                Content = body,
                ContentType = BodyType.Text,
            },
            DueDateTime = !string.IsNullOrWhiteSpace(dueDate) ? new DateTimeTimeZone()
            {
                DateTime = dueDate,
                TimeZone = "UTC",
            }
                : null,
        };

        var result = await client.Me.Todo.Lists[listId].Tasks[taskId].PatchAsync(task, cancellationToken: cancellationToken);
        return JsonDocument.Parse(JsonSerializer.Serialize(result, MicrosoftGraphSerializationOptions.MicrosoftGraphJsonSerializerOptions));
    }
}