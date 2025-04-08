// ------------------------------------------------------
// <copyright file="IGmailTool.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Text.Json;
using DonkeyWork.Chat.AiTooling.Base;
using Google.Apis.Gmail.v1;

namespace DonkeyWork.Chat.AiTooling.ToolImplementations.GoogleApi.Gmail;

/// <summary>
/// An interface for the Gmail tool.
/// </summary>
public interface IGmailTool : ITool
{
    /// <summary>
    /// Sends an email using Gmail.
    /// </summary>
    /// <param name="to">The recipient email address(es), comma-separated for multiple recipients.</param>
    /// <param name="subject">The email subject.</param>
    /// <param name="body">The email body content.</param>
    /// <param name="cc">The CC email address(es), comma-separated for multiple recipients (optional).</param>
    /// <param name="bcc">The BCC email address(es), comma-separated for multiple recipients (optional).</param>
    /// <param name="isHtml">Whether the body content is HTML (default is true).</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task<JsonDocument?> SendGmailAsync(
        string to,
        string subject,
        string body,
        string? cc = null,
        string? bcc = null,
        bool isHtml = true,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists emails from Gmail inbox with filtering options.
    /// </summary>
    /// <param name="maxResults">Maximum number of emails to return.</param>
    /// <param name="query">Search query using Gmail search syntax (optional).</param>
    /// <param name="labelIds">Labels to filter by (comma-separated, optional).</param>
    /// <param name="includeAttachments">Whether to include attachment information (optional).</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task<JsonDocument?> ListGmailMessagesAsync(
        int maxResults,
        string? query = null,
        string? labelIds = null,
        bool includeAttachments = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a specific email from Gmail.
    /// </summary>
    /// <param name="messageId">The ID of the message to retrieve.</param>
    /// <param name="format">The format to return the message in (full, metadata, minimal, raw).</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task<JsonDocument?> GetGmailMessageAsync(
        string messageId,
        UsersResource.MessagesResource.GetRequest.FormatEnum? format,
        CancellationToken cancellationToken = default);
}