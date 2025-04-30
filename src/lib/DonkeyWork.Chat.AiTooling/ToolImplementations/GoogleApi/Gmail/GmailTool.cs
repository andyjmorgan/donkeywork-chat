// ------------------------------------------------------
// <copyright file="GmailTool.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.ComponentModel;
using System.Text;
using System.Text.Json;
using DonkeyWork.Chat.AiTooling.Attributes;
using DonkeyWork.Chat.AiTooling.ToolImplementations.GoogleApi.Common.Api;
using DonkeyWork.Chat.Common.Models.Providers.Tools;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Microsoft.Extensions.Logging;

namespace DonkeyWork.Chat.AiTooling.ToolImplementations.GoogleApi.Gmail;

/// <inheritdoc cref="DonkeyWork.Chat.AiTooling.ToolImplementations.GoogleApi.Gmail.IGmailTool" />
[OAuthToolProvider(ToolProviderType.Google)]
[ToolProviderApplicationType(ToolProviderApplicationType.GoogleMail)]
public class GmailTool(
    IGoogleApiClientFactory googleApiClientFactory, ILogger<GmailTool> logger)
    : Base.Tool(logger), IGmailTool
{
    private readonly JsonSerializerOptions jsonSerializerOptions = new ()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    /// <inheritdoc />
    [ToolFunction]
    [Description("A tool to send an email using Gmail.")]
    [ToolProviderScopes(
        UserProviderScopeHandleType.Any,
        "https://www.googleapis.com/auth/gmail.modify")]
    public async Task<JsonDocument?> SendGmailAsync(
        [Description("The recipient email address(es), comma-separated for multiple recipients.")]
        string to,
        [Description("The email subject.")] string subject,
        [Description("The email body content.")]
        string body,
        [Description("The CC email address(es), comma-separated for multiple recipients (optional).")]
        string? cc = null,
        [Description("The BCC email address(es), comma-separated for multiple recipients (optional).")]
        string? bcc = null,
        [Description("Whether the body content is HTML (default is true).")]
        bool isHtml = true,
        [ToolIgnoredParameter] CancellationToken cancellationToken = default)
    {
        var gmailService = await googleApiClientFactory.CreateGmailServiceAsync(cancellationToken);

        // Create email message
        var emailMessage = new StringBuilder();
        emailMessage.AppendLine("To: " + to);

        if (!string.IsNullOrEmpty(cc))
        {
            emailMessage.AppendLine("Cc: " + cc);
        }

        if (!string.IsNullOrEmpty(bcc))
        {
            emailMessage.AppendLine("Bcc: " + bcc);
        }

        emailMessage.AppendLine("Subject: " + subject);
        emailMessage.AppendLine("Content-Type: " + (isHtml ? "text/html; charset=UTF-8" : "text/plain; charset=UTF-8"));
        emailMessage.AppendLine();
        emailMessage.AppendLine(body);

        // Convert the email message to base64url encoding
        var messageBytes = Encoding.UTF8.GetBytes(emailMessage.ToString());
        var base64UrlEncodedMessage = Convert.ToBase64String(messageBytes)
            .Replace('+', '-')
            .Replace('/', '_')
            .Replace("=", string.Empty);

        // Create the message object
        var message = new Message
        {
            Raw = base64UrlEncodedMessage,
        };

        // Send the message
        var request = gmailService.Users.Messages.Send(message, "me");
        var response = await request.ExecuteAsync(cancellationToken);

        var result = new
        {
            Success = true,
            MessageId = response.Id,
            response.ThreadId,
        };

        return JsonDocument.Parse(JsonSerializer.Serialize(result, this.jsonSerializerOptions));
    }

    /// <inheritdoc />
    [ToolFunction]
    [Description("A tool to list emails from Gmail inbox with filtering options.")]
    [ToolProviderScopes(
        UserProviderScopeHandleType.Any,
        "https://www.googleapis.com/auth/gmail.modify")]
    public async Task<JsonDocument?> ListGmailMessagesAsync(
        [Description("Maximum number of emails to return.")]
        int maxResults,
        [Description("Search query using Gmail search syntax (optional).")]
        string? query = null,
        [Description("Labels to filter by (comma-separated, optional).")]
        string? labelIds = null,
        [Description("Whether to include attachment information (optional).")]
        bool includeAttachments = false,
        [ToolIgnoredParameter] CancellationToken cancellationToken = default)
    {
        var gmailService = await googleApiClientFactory.CreateGmailServiceAsync(cancellationToken);

        // Create the list request
        var listRequest = gmailService.Users.Messages.List("me");
        listRequest.MaxResults = maxResults;

        if (!string.IsNullOrEmpty(query))
        {
            listRequest.Q = query;
        }

        if (!string.IsNullOrEmpty(labelIds))
        {
            listRequest.LabelIds = labelIds.Split(',').Select(l => l.Trim()).ToList();
        }

        // Execute the request to get message IDs
        var messagesResponse = await listRequest.ExecuteAsync(cancellationToken);

        if (messagesResponse.Messages == null || !messagesResponse.Messages.Any())
        {
            return JsonDocument.Parse("{\"messages\": [], \"resultSizeEstimate\": 0}");
        }

        // Process each message
        var messages = new List<object>();

        foreach (var messageRef in messagesResponse.Messages)
        {
            var messageRequest = gmailService.Users.Messages.Get("me", messageRef.Id);
            messageRequest.Format = UsersResource.MessagesResource.GetRequest.FormatEnum.Full;

            var message = await messageRequest.ExecuteAsync(cancellationToken);

            // Extract headers
            var headers = new Dictionary<string, string>();
            if (message.Payload?.Headers != null)
            {
                foreach (var header in message.Payload.Headers)
                {
                    headers[header.Name] = header.Value;
                }
            }

            // Process attachments if requested
            var attachments = new List<object>();
            if (includeAttachments && message.Payload?.Parts != null)
            {
                foreach (var part in message.Payload.Parts)
                {
                    if (!string.IsNullOrEmpty(part.Filename))
                    {
                        attachments.Add(new
                        {
                            FileName = part.Filename,
                            part.MimeType,
                            part.Body?.Size,
                            part.Body?.AttachmentId,
                        });
                    }
                }
            }

            // Add message to result list
            messages.Add(new
            {
                message.Id,
                message.ThreadId,
                message.LabelIds,
                message.Snippet,
                From = headers.GetValueOrDefault("From"),
                To = headers.GetValueOrDefault("To"),
                Subject = headers.GetValueOrDefault("Subject"),
                Date = headers.GetValueOrDefault("Date"),
                Attachments = includeAttachments ? attachments : null,
            });
        }

        var result = new
        {
            Messages = messages, messagesResponse.ResultSizeEstimate,
        };

        return JsonDocument.Parse(JsonSerializer.Serialize(result, this.jsonSerializerOptions));
    }

    /// <inheritdoc />
    [ToolFunction]
    [Description("A tool to get a specific email from Gmail.")]
    [ToolProviderScopes(
        UserProviderScopeHandleType.Any,
        "https://www.googleapis.com/auth/gmail.modify")]
    public async Task<JsonDocument?> GetGmailMessageAsync(
        [Description("The ID of the message to retrieve.")]
        string messageId,
        [Description("The format to return the message in (full, metadata, minimal, raw).")]
        UsersResource.MessagesResource.GetRequest.FormatEnum? format,
        [ToolIgnoredParameter] CancellationToken cancellationToken = default)
    {
        var gmailService = await googleApiClientFactory.CreateGmailServiceAsync(cancellationToken);

        // Create the get request
        var getRequest = gmailService.Users.Messages.Get("me", messageId);
        getRequest.Format = format ?? UsersResource.MessagesResource.GetRequest.FormatEnum.Full;

        // Execute the request
        var message = await getRequest.ExecuteAsync(cancellationToken);

        // Process the message to extract useful information
        var headers = new Dictionary<string, string>();
        if (message.Payload?.Headers != null)
        {
            foreach (var header in message.Payload.Headers)
            {
                headers[header.Name] = header.Value;
            }
        }

        // Extract message body
        string? messageBody = null;
        string? bodyMimeType = null;

        if (message.Payload != null)
        {
            if (message.Payload.Body?.Data != null)
            {
                // Direct body
                var bodyData = message.Payload.Body.Data.Replace('-', '+').Replace('_', '/');
                var bodyBytes = Convert.FromBase64String(bodyData);
                messageBody = Encoding.UTF8.GetString(bodyBytes);
                bodyMimeType = message.Payload.MimeType;
            }
            else if (message.Payload.Parts != null)
            {
                // Multipart message
                foreach (var part in message.Payload.Parts)
                {
                    if (part.MimeType == "text/plain" || part.MimeType == "text/html")
                    {
                        if (part.Body?.Data != null)
                        {
                            var bodyData = part.Body.Data.Replace('-', '+').Replace('_', '/');
                            var bodyBytes = Convert.FromBase64String(bodyData);
                            messageBody = Encoding.UTF8.GetString(bodyBytes);
                            bodyMimeType = part.MimeType;

                            // Prefer HTML if available
                            if (part.MimeType == "text/html")
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }

        // Extract attachments
        var attachments = new List<object>();
        if (message.Payload?.Parts != null)
        {
            foreach (var part in message.Payload.Parts)
            {
                if (!string.IsNullOrEmpty(part.Filename))
                {
                    attachments.Add(new
                    {
                        FileName = part.Filename,
                        part.MimeType,
                        part.Body?.Size,
                        part.Body?.AttachmentId,
                    });
                }
            }
        }

        // Create result object
        var result = new
        {
            message.Id,
            message.ThreadId,
            message.LabelIds,
            message.Snippet,
            message.HistoryId,
            message.InternalDate,
            message.SizeEstimate,
            From = headers.GetValueOrDefault("From"),
            To = headers.GetValueOrDefault("To"),
            Subject = headers.GetValueOrDefault("Subject"),
            Date = headers.GetValueOrDefault("Date"),
            Cc = headers.GetValueOrDefault("Cc"),
            Body = new
            {
                Content = messageBody,
                MimeType = bodyMimeType,
            },
            Attachments = attachments.Count > 0 ? attachments : null,
        };

        return JsonDocument.Parse(JsonSerializer.Serialize(result, this.jsonSerializerOptions));
    }
}