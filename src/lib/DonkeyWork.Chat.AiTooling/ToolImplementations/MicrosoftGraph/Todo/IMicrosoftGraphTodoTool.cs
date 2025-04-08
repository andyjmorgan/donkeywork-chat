// ------------------------------------------------------
// <copyright file="IMicrosoftGraphTodoTool.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Text.Json;
using Microsoft.Graph.Models;
using TaskStatus = Microsoft.Graph.Models.TaskStatus;

namespace DonkeyWork.Chat.AiTooling.ToolImplementations.MicrosoftGraph.Todo;

/// <summary>
/// A Microsoft Graph Todo tool.
/// </summary>
public interface IMicrosoftGraphTodoTool
{
    /// <summary>
    /// Retrieves all Microsoft To Do lists for the current user.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task<JsonDocument> GetMicrosoftGraphTodoListsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new Microsoft To Do list with the specified name.
    /// </summary>
    /// <param name="displayName">The name of the list to create.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task<JsonDocument> CreateMicrosoftGraphTodoListAsync(string displayName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all tasks in the specified Microsoft To Do list.
    /// </summary>
    /// <param name="listId">The ID of the To Do list.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task<JsonDocument> GetMicrosoftGraphTasksAsync(string listId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a task from the specified Microsoft To Do list.
    /// </summary>
    /// <param name="listId">The id of the To Do list.</param>
    /// <param name="taskId">The id of the task.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task<JsonDocument> DeleteMicrosoftGraphTaskAsync(
        string listId,
        string taskId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a task in the specified Microsoft To Do list.
    /// </summary>
    /// <param name="listId">The ID of the To Do list.</param>
    /// <param name="title">The title of the task.</param>
    /// <param name="categories">The categories.</param>
    /// <param name="body">Optional body text for the task.</param>
    /// <param name="importance">The importance.</param>
    /// <param name="status">The status.</param>
    /// <param name="dueDate">Optional due date for the task.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task<JsonDocument> CreateMicrosoftGraphTaskAsync(string listId, string title, List<string>? categories, string? body = null, Importance? importance = null, TaskStatus? status = null, string? dueDate = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a task in the specified Microsoft To Do list.
    /// </summary>
    /// <param name="listId">The ID of the To Do list.</param>
    /// <param name="taskId">THe task id.</param>
    /// <param name="title">The title of the task.</param>
    /// <param name="categories">The categories.</param>
    /// <param name="body">Optional body text for the task.</param>
    /// <param name="importance">The importance.</param>
    /// <param name="status">The status.</param>
    /// <param name="dueDate">Optional due date for the task.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task<JsonDocument> UpdateMicrosoftGraphTaskAsync(string listId, string taskId, string title, List<string>? categories, string? body = null, Importance? importance = null, TaskStatus? status = null, string? dueDate = null, CancellationToken cancellationToken = default);
}