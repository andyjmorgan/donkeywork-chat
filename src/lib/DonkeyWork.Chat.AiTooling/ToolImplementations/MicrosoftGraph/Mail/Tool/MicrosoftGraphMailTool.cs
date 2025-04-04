using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using DonkeyWork.Chat.AiTooling.Attributes;
using DonkeyWork.Chat.AiTooling.ToolImplementations.MicrosoftGraph.Common.Api;
using DonkeyWork.Chat.Common.Providers;
using Microsoft.Graph.Me.SendMail;
using Microsoft.Graph.Models;

namespace DonkeyWork.Chat.AiTooling.ToolImplementations.MicrosoftGraph.Mail.Tool;

/// <summary>
/// A microsoft graph mail tool implementation.
/// </summary>
[ToolProvider(UserProviderType.Microsoft)]
public class MicrosoftGraphMailTool(
    IMicrosoftGraphApiClientFactory microsoftGraphApiClientFactory)
    : Base.Tool, IMicrosoftGraphMailTool
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new ()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    /// <inheritdoc />
    [ToolFunction]
    [Description("Search the user's mailbox for emails matching a query.")]
    [ToolProviderScopes(
        UserProviderScopeHandleType.Any,
        "Mail.Read",
        "Mail.ReadBasic",
        "Mail.ReadWrite")]
    public async Task<JsonDocument?> SearchEmailAsync(
        [Description("The OData filter query string (e.g., \"contains(subject, 'report')\").")]
        string query,
        [Description("The maximum number of messages to return.")]
        int? maxCount = null,
        [Description("The number of messages to skip (for pagination).")]
        int? skip = null,
        [ToolIgnoredParameter] CancellationToken cancellationToken = default)
    {
        var client = await microsoftGraphApiClientFactory.CreateGraphClientAsync(cancellationToken);

        var messages = await client.Me.Messages.GetAsync(
            config =>
        {
            if (!string.IsNullOrWhiteSpace(query))
            {
                config.QueryParameters.Filter = query;
            }

            if (maxCount.HasValue)
            {
                config.QueryParameters.Top = maxCount.Value;
            }

            if (skip.HasValue)
            {
                config.QueryParameters.Skip = skip.Value;
            }
        },
            cancellationToken);

        return JsonDocument.Parse(JsonSerializer.Serialize(messages?.Value, JsonSerializerOptions));
    }

    /// <inheritdoc />
    [ToolFunction]
    [Description("Send an email on behalf of the user.")]
    [ToolProviderScopes(
        UserProviderScopeHandleType.Any,
        "Mail.Send")]
    public async Task<JsonDocument?> SendEmailAsync(
        [Description("The subject of the email.")]
        string subject,
        [Description("The plain text content of the email body.")]
        string body,
        [Description("A list of recipient email addresses.")]
        List<string> toRecipients,
        [ToolIgnoredParameter] CancellationToken cancellationToken = default)
    {
        var client = await microsoftGraphApiClientFactory.CreateGraphClientAsync(cancellationToken);

        var message = new Message
        {
            Subject = subject,
            Body = new ItemBody
            {
                ContentType = BodyType.Text,
                Content = body,
            },
            ToRecipients = toRecipients.Select(email => new Recipient
            {
                EmailAddress = new EmailAddress
                {
                    Address = email,
                },
            }).ToList(),
        };

        await client.Me.SendMail.PostAsync(
            new SendMailPostRequestBody()
        {
            Message = message,
            SaveToSentItems = true,
        }, cancellationToken: cancellationToken);

        return JsonDocument.Parse("{\"status\":\"Email sent successfully\"}");
    }

    /// <inheritdoc />
    [ToolFunction]
    [Description("Create a draft email message.")]
    [ToolProviderScopes(
        UserProviderScopeHandleType.Any,
        "Mail.ReadWrite")]
    public async Task<JsonDocument?> CreateDraftEmailAsync(
        [Description("The subject of the draft email.")]
        string subject,
        [Description("The plain text content of the email body.")]
        string body,
        [Description("A list of recipient email addresses.")]
        List<string> toRecipients,
        [ToolIgnoredParameter] CancellationToken cancellationToken = default)
    {
        var client = await microsoftGraphApiClientFactory.CreateGraphClientAsync(cancellationToken);

        var draft = new Message
        {
            Subject = subject,
            Body = new ItemBody
            {
                ContentType = BodyType.Text,
                Content = body,
            },
            ToRecipients = toRecipients.Select(
                email =>
                    new Recipient
            {
                EmailAddress = new EmailAddress { Address = email },
            }).ToList(),
        };

        var result = await client.Me.Messages.PostAsync(draft, cancellationToken: cancellationToken);

        return JsonDocument.Parse(JsonSerializer.Serialize(result, JsonSerializerOptions));
    }

    /// <inheritdoc />
    [ToolFunction]
    [Description("Send a previously created draft email.")]
    [ToolProviderScopes(
        UserProviderScopeHandleType.Any,
        "Mail.Send",
        "Mail.ReadWrite")]
    public async Task<JsonDocument?> SendDraftEmailAsync(
        [Description("The ID of the draft message to send.")]
        string draftMessageId,
        [ToolIgnoredParameter] CancellationToken cancellationToken = default)
    {
        var client = await microsoftGraphApiClientFactory.CreateGraphClientAsync(cancellationToken);

        await client.Me.Messages[draftMessageId].Send.PostAsync(cancellationToken: cancellationToken);
        return JsonDocument.Parse("{\"status\":\"Draft sent successfully\"}");
    }

    /// <inheritdoc />
    [ToolFunction]
    [Description("Get a specific email by message ID.")]
    [ToolProviderScopes(
        UserProviderScopeHandleType.Any,
        "Mail.Read",
        "Mail.ReadBasic",
        "Mail.ReadWrite")]
    public async Task<JsonDocument?> GetEmailByIdAsync(
        [Description("The ID of the email message.")]
        string messageId,
        [ToolIgnoredParameter] CancellationToken cancellationToken = default)
    {
        var client = await microsoftGraphApiClientFactory.CreateGraphClientAsync(cancellationToken);
        var message = await client.Me.Messages[messageId].GetAsync(cancellationToken: cancellationToken);
        return JsonDocument.Parse(JsonSerializer.Serialize(message, JsonSerializerOptions));
    }

    /// <inheritdoc />
    [ToolFunction]
    [Description("Delete an email by message ID.")]
    [ToolProviderScopes(
        UserProviderScopeHandleType.Any,
        "Mail.ReadWrite")]
    public async Task<JsonDocument?> DeleteEmailAsync(
        [Description("The ID of the email message to delete.")]
        string messageId,
        [ToolIgnoredParameter] CancellationToken cancellationToken = default)
    {
        var client = await microsoftGraphApiClientFactory.CreateGraphClientAsync(cancellationToken);
        await client.Me.Messages[messageId].DeleteAsync(cancellationToken: cancellationToken);
        return JsonDocument.Parse("{\"status\":\"Email deleted successfully\"}");
    }

    /// <inheritdoc />
    [ToolFunction]
    [Description("List the user's mail folders.")]
    [ToolProviderScopes(
        UserProviderScopeHandleType.Any,
        "Mail.Read",
        "Mail.ReadWrite")]
    public async Task<JsonDocument?> ListMailFoldersAsync(
        [ToolIgnoredParameter] CancellationToken cancellationToken = default)
    {
        var client = await microsoftGraphApiClientFactory.CreateGraphClientAsync(cancellationToken);
        var folders = await client.Me.MailFolders.GetAsync(cancellationToken: cancellationToken);
        return JsonDocument.Parse(JsonSerializer.Serialize(folders?.Value, JsonSerializerOptions));
    }

    /// <inheritdoc />
    [ToolFunction]
    [Description("Forward an email to other recipients.")]
    [ToolProviderScopes(
        UserProviderScopeHandleType.Any,
        "Mail.ReadWrite")]
    public async Task<JsonDocument?> ForwardEmailAsync(
        [Description("The ID of the email message to forward.")]
        string messageId,
        [Description("A list of recipient email addresses.")]
        List<string> toRecipients,
        [Description("A comment to include in the forwarded message.")]
        string comment,
        [ToolIgnoredParameter] CancellationToken cancellationToken = default)
    {
        var client = await microsoftGraphApiClientFactory.CreateGraphClientAsync(cancellationToken);
        await client.Me.Messages[messageId].Forward.PostAsync(
            new()
            {
                ToRecipients = toRecipients.Select(
                    address =>
                        new Recipient
                        {
                            EmailAddress = new EmailAddress
                            {
                                Address = address,
                            },
                        }).ToList(),
                Comment = comment,
            },
            cancellationToken: cancellationToken);

        return JsonDocument.Parse("{\"status\":\"Email forwarded successfully\"}");
    }

    /// <inheritdoc />
    [ToolFunction]
    [Description("Reply to a specific email message.")]
    [ToolProviderScopes(
        UserProviderScopeHandleType.Any,
        "Mail.ReadWrite")]
    public async Task<JsonDocument?> ReplyToEmailAsync(
        [Description("The ID of the message to reply to.")]
        string messageId,
        [Description("The reply body text.")] string comment,
        [ToolIgnoredParameter] CancellationToken cancellationToken = default)
    {
        var client = await microsoftGraphApiClientFactory.CreateGraphClientAsync(cancellationToken);
        await client.Me.Messages[messageId].Reply.PostAsync(
            new()
            {
                Comment = comment,
            }, cancellationToken: cancellationToken);

        return JsonDocument.Parse("{\"status\":\"Reply sent successfully\"}");
    }

    /// <inheritdoc />
    [ToolFunction]
    [Description("Reply to all recipients of a message.")]
    [ToolProviderScopes(
        UserProviderScopeHandleType.Any,
        "Mail.ReadWrite")]
    public async Task<JsonDocument?> ReplyToAllAsync(
        [Description("The ID of the message to reply to.")]
        string messageId,
        [Description("The reply body text.")] string comment,
        [ToolIgnoredParameter] CancellationToken cancellationToken = default)
    {
        var client = await microsoftGraphApiClientFactory.CreateGraphClientAsync(cancellationToken);
        await client.Me.Messages[messageId].ReplyAll.PostAsync(
            new()
            {
                Comment = comment,
            },
            cancellationToken: cancellationToken);

        return JsonDocument.Parse("{\"status\":\"Reply all sent successfully\"}");
    }
}