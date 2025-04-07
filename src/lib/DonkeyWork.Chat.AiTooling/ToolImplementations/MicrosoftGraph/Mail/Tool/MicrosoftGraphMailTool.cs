// ------------------------------------------------------
// <copyright file="MicrosoftGraphMailTool.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.ComponentModel;
using System.Text.Json;
using DonkeyWork.Chat.AiTooling.Attributes;
using DonkeyWork.Chat.AiTooling.ToolImplementations.MicrosoftGraph.Common;
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
    /// <inheritdoc />
    [ToolFunction]
    [Description("Search the user's mailbox for emails matching a query using the Microsoft Graph api.")]
    [ToolProviderScopes(
        UserProviderScopeHandleType.Any,
        "Mail.Read",
        "Mail.ReadBasic",
        "Mail.ReadWrite")]
    public async Task<JsonDocument?> SearchEmailAsync(
        [Description("The search query string (e.g., \"report\"). Ensure to us US Formatted date strings (MM/DD/YYYY) for date searches.")]
        string search,
        [Description("The select query parameters (e.g., \"subject\",\"from\", \"to\") etc. Optional.")]
        List<string>? select,
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
            config.QueryParameters.Search = $"\"{search}\"";
            config.QueryParameters.Count = true;
            if (select?.Any() ?? false)
            {
                config.QueryParameters.Select = select.ToArray();
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

        return JsonDocument.Parse(JsonSerializer.Serialize(messages?.Value, MicrosoftGraphSerializationOptions.MicrosoftGraphJsonSerializerOptions));
    }

    /// <inheritdoc />
    [ToolFunction]
    [Description("A helper tool to describe the Microsoft Graph email search method and usage.")]
    public Task<JsonDocument> GetSearchQueryLanguageAsync()
    {
        var prompt = """
            # Microsoft Graph - search Query for Mail Messages
            
            The `search` query parameter allows full-text search over email messages in Microsoft Graph. You can target specific fields like `subject`, `from`, `to`, etc., and apply keyword searches.
            
            When using `search`, make sure to:
            - Use `$count=true` if needed
            - URL-encode special characters like spaces
            
            ## Searchable Email Properties
            
            | Property       | Description                                                                 | Example                                                                                         |
            |----------------|-----------------------------------------------------------------------------|-------------------------------------------------------------------------------------------------|
            | `attachment`   | The names of files attached to an email message.                            | "attachment:api-catalog.md"                                             |
            | `bcc`          | The Bcc field of an email message. Use SMTP address, display name, or alias.| `"bcc:samanthab@contoso.com"&$select=subject,bccRecipients`               |
            | `body`         | The body text of an email message.                                          | `"body:excitement"`                                                        |
            | `cc`           | The Cc field of an email message.                                           | `"cc:danas"&$select=subject,ccRecipients`                                  |
            | `from`         | The sender of the email. Use SMTP address, display name, or alias.          | `"from:randiw"&$select=subject,from`                                       |
            |                |                                                                             | `"from:adelev OR from:alexw OR from:allanD"&$select=subject,from`         |
            | `hasAttachment`| Whether an email contains attachments (excluding inline ones).              | `"hasAttachments:true"`                                                    |
            | `importance`   | Email importance: `low`, `medium`, or `high`.                               | `"importance:high"&$select=subject,importance`                             |
            | `kind`         | The type of message (e.g., `email`, `voicemail`, `docs`, etc.).             | `"kind:voicemail"`                                                         |
            | `participants` | Searches `from`, `to`, `cc`, and `bcc` fields for a match.                  | `"participants:danas"`                                                     |
            | `received`     | The date the email was received.                                            | `"received:07/23/2018"&$select=subject,receivedDateTime`                   |
            | `recipients`   | Searches `to`, `cc`, and `bcc` fields.                                      | `"recipients:randiq"&$select=subject,toRecipients,ccRecipients,bccRecipients` |
            | `sent`         | The date the email was sent.                                                | `"sent:07/23/2018"&$select=subject,sentDateTime`                           |
            | `size`         | The size of the email in bytes.                                             | `"size:1..500000"`                                                         |
            | `subject`      | The subject line text.                                                      | `"subject:has"&$select=subject`                                            |
            | `to`           | The To field of an email.                                                   | `"to:randiw"&$select=subject,toRecipients`                                 |
            
            ---
            
            ## General Notes
            - If you do a search on messages and specify only a value without specific message properties, the search is carried out on the default search properties of from, subject, and body.
            - You **cannot** combine `$search` with `$filter` in the same request.
            - search does not support advanced expressions like wildcards or fuzzy matches.
            - Use `select` to limit returned fields and improve performance.
            - Queries return up to 1000 results unless paginated.
            
            For full documentation, visit:  
            [https://learn.microsoft.com/en-us/graph/search-query-parameter?tabs=http#using-search-on-message-collections](https://learn.microsoft.com/en-us/graph/search-query-parameter?tabs=http#using-search-on-message-collections)
            """;
        return Task.FromResult(JsonDocument.Parse(JsonSerializer.Serialize(new
        {
            Documentation = prompt,
        })));
    }

    /// <inheritdoc />
    [ToolFunction]
    [Description("Send an email on behalf of the user using the Microsoft Graph api.")]
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
    [Description("Create a draft email message using the Microsoft Graph api.")]
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

        return JsonDocument.Parse(JsonSerializer.Serialize(result, MicrosoftGraphSerializationOptions.MicrosoftGraphJsonSerializerOptions));
    }

    /// <inheritdoc />
    [ToolFunction]
    [Description("Send a previously created draft email using the Microsoft Graph api.")]
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
    [Description("Get a specific email by message ID using the Microsoft Graph api.")]
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
        return JsonDocument.Parse(JsonSerializer.Serialize(message, MicrosoftGraphSerializationOptions.MicrosoftGraphJsonSerializerOptions));
    }

    /// <inheritdoc />
    [ToolFunction]
    [Description("Delete an email by message ID using the Microsoft Graph api.")]
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
    [Description("List the user's mail folders using the Microsoft Graph api.")]
    [ToolProviderScopes(
        UserProviderScopeHandleType.Any,
        "Mail.Read",
        "Mail.ReadWrite")]
    public async Task<JsonDocument?> ListMailFoldersAsync(
        [ToolIgnoredParameter] CancellationToken cancellationToken = default)
    {
        var client = await microsoftGraphApiClientFactory.CreateGraphClientAsync(cancellationToken);
        var folders = await client.Me.MailFolders.GetAsync(cancellationToken: cancellationToken);
        return JsonDocument.Parse(JsonSerializer.Serialize(folders?.Value, MicrosoftGraphSerializationOptions.MicrosoftGraphJsonSerializerOptions));
    }

    /// <inheritdoc />
    [ToolFunction]
    [Description("Forward an email to other recipients using the Microsoft Graph api.")]
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
    [Description("Reply to a specific email message using the Microsoft Graph api.")]
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
    [Description("Reply to all recipients of a message using the Microsoft Graph api.")]
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