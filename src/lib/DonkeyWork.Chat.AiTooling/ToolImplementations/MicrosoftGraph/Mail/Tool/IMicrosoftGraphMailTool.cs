// ------------------------------------------------------
// <copyright file="IMicrosoftGraphMailTool.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Text.Json;
using Microsoft.Graph.Models;

namespace DonkeyWork.Chat.AiTooling.ToolImplementations.MicrosoftGraph.Mail.Tool;

/// <summary>
/// A tool interface for Microsoft Graph Mail operations.
/// </summary>
public interface IMicrosoftGraphMailTool
{
    /// <summary>
    /// Searches the user's mailbox for email messages that match a given query string.
    /// </summary>
    /// <param name="search">The search query.</param>
    /// <param name="select">The optional select statement.</param>
    /// <param name="maxCount">The maximum number of messages to return.</param>
    /// <param name="skip">The number of messages to skip (for pagination).</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A JSON document representing the matching messages.</returns>
    Task<JsonDocument?> SearchMicrosoftGraphEmailAsync(
        string search,
        List<string>? select,
        int? maxCount = null,
        int? skip = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the search query language supported by the Microsoft Graph API.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<JsonDocument> GetMicrosoftGraphSearchQueryLanguageAsync();

    /// <summary>
    /// Sends an email message on behalf of the authenticated user.
    /// </summary>
    /// <param name="subject">The subject of the email.</param>
    /// <param name="body">The plain text content of the email body.</param>
    /// <param name="toRecipients">A list of recipient email addresses.</param>
    /// <param name="bodyType">The body type.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A JSON document indicating success or failure.</returns>
    Task<JsonDocument?> SendMicrosoftGraphEmailAsync(
        string subject,
        string body,
        List<string> toRecipients,
        BodyType? bodyType = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a draft email message in the user's mailbox.
    /// </summary>
    /// <param name="subject">The subject of the draft email.</param>
    /// <param name="body">The plain text content of the email body.</param>
    /// <param name="toRecipients">A list of recipient email addresses.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A JSON document representing the created draft message.</returns>
    Task<JsonDocument?> CreateMicrosoftGraphDraftEmailAsync(
        string subject,
        string body,
        List<string> toRecipients,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends an existing draft email by its message ID.
    /// </summary>
    /// <param name="draftMessageId">The draft message id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<JsonDocument?> SendMicrosoftGraphDraftEmailAsync(
        string draftMessageId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a single email message by its ID.
    /// </summary>
    /// <param name="messageId">The message id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<JsonDocument?> GetMicrosoftGraphEmailByIdAsync(
        string messageId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an email message from the user's mailbox by its ID.
    /// </summary>
    /// <param name="messageId">The message id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<JsonDocument?> DeleteMicrosoftGraphEmailAsync(
        string messageId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists all mail folders in the user's mailbox.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<JsonDocument?> ListMicrosoftGraphMailFoldersAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Forwards an email message to a list of recipients with an optional comment.
    /// </summary>
    /// <param name="messageId">The message id.</param>
    /// <param name="toRecipients">The recipients.</param>
    /// <param name="comment">The message comment.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<JsonDocument?> ForwardMicrosoftGraphEmailAsync(
        string messageId,
        List<string> toRecipients,
        string comment,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Replies to a specific message.
    /// </summary>
    /// <param name="messageId">The message id.</param>
    /// <param name="comment">The message comment.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<JsonDocument?> ReplyToMicrosoftGraphEmailAsync(
        string messageId,
        string comment,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Replies to all recipients of a message.
    /// </summary>
    /// <param name="messageId">The message id.</param>
    /// <param name="comment">The comment.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<JsonDocument?> ReplyToAllMicrosoftGraphAsync(
        string messageId,
        string comment,
        CancellationToken cancellationToken = default);
}