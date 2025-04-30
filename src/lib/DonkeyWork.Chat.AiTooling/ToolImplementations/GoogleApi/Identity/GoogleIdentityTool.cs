// ------------------------------------------------------
// <copyright file="GoogleIdentityTool.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.ComponentModel;
using System.Text.Json;
using DonkeyWork.Chat.AiTooling.Attributes;
using DonkeyWork.Chat.AiTooling.ToolImplementations.GoogleApi.Common.Api;
using DonkeyWork.Chat.Common.Models.Providers.Tools;
using Microsoft.Extensions.Logging;

namespace DonkeyWork.Chat.AiTooling.ToolImplementations.GoogleApi.Identity;

/// <inheritdoc cref="IGoogleIdentityTool"/>
[OAuthToolProvider(ToolProviderType.Google)]
[ToolProviderApplicationType(ToolProviderApplicationType.GoogleIdentity)]
public class GoogleIdentityTool(IGoogleApiClientFactory googleApiClientFactory, ILogger<GoogleIdentityTool> logger)
    : Base.Tool(logger), IGoogleIdentityTool
{
    private readonly JsonSerializerOptions jsonSerializerOptions = new ()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    /// <inheritdoc />
    [ToolFunction]
    [Description("A tool to retrieve the users google identity.")]
    [ToolProviderScopes(
        UserProviderScopeHandleType.Any,
        "openid",
        "email",
        "profile")]
    public async Task<JsonDocument?> GetGoogleIdentityAsync([ToolIgnoredParameter] CancellationToken cancellationToken = default)
    {
        var authService = await googleApiClientFactory.CreateOauth2ServiceAsync(cancellationToken);
        var request = authService.Userinfo.V2.Me.Get();
        var result = await request.ExecuteAsync(cancellationToken);
        return JsonDocument.Parse(JsonSerializer.Serialize(result, this.jsonSerializerOptions));
    }
}