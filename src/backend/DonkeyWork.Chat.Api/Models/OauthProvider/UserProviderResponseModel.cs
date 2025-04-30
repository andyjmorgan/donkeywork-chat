// ------------------------------------------------------
// <copyright file="UserProviderResponseModel.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Models.Providers.Tools;

namespace DonkeyWork.Chat.Api.Models.OauthProvider;

/// <summary>
/// Gets the provider configuration for a user.
/// </summary>
public class UserProviderResponseModel
{
    /// <summary>
    /// Gets or sets the provider configuration for a user.
    /// </summary>
    public Dictionary<ToolProviderType, List<string>> ProviderConfiguration { get; set; } = [];
}