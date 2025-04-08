// ------------------------------------------------------
// <copyright file="MicrosoftGraphIdentityTool.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.ComponentModel;
using System.Text.Json;
using DonkeyWork.Chat.AiTooling.Attributes;
using DonkeyWork.Chat.AiTooling.ToolImplementations.MicrosoftGraph.Common;
using DonkeyWork.Chat.AiTooling.ToolImplementations.MicrosoftGraph.Common.Api;
using DonkeyWork.Chat.Common.Providers;

namespace DonkeyWork.Chat.AiTooling.ToolImplementations.MicrosoftGraph.Identity;

/// <summary>
/// A class that implements the <see cref="IMicrosoftGraphIdentityTool"/> interface.
/// </summary>
[ToolProvider(UserProviderType.Microsoft)]
public class MicrosoftGraphIdentityTool(IMicrosoftGraphApiClientFactory microsoftGraphApiClientFactory)
    : Base.Tool, IMicrosoftGraphIdentityTool
{
    /// <param name="cancellationToken"></param>
    /// <inheritdoc />
    [ToolProviderScopes(UserProviderScopeHandleType.Any, "User.Read")]
    [ToolFunction]
    [Description("A tool to get the current users information via the Microsoft Graph api.")]
    public async Task<JsonDocument> GetUserInformationAsync([ToolIgnoredParameter] CancellationToken cancellationToken = default)
    {
        var graphClient = await microsoftGraphApiClientFactory.CreateGraphClientAsync(cancellationToken);
        var searchResult = await graphClient.Me.GetAsync(cancellationToken: cancellationToken);
        return JsonDocument.Parse(JsonSerializer.Serialize(searchResult, MicrosoftGraphSerializationOptions.MicrosoftGraphJsonSerializerOptions));
    }
}