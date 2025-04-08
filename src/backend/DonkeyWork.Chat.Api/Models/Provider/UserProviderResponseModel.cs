// ------------------------------------------------------
// <copyright file="UserProviderResponseModel.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Providers;

namespace DonkeyWork.Chat.Api.Models.Provider;

/// <summary>
/// Gets the provider configuration for a user.
/// </summary>
public class UserProviderResponseModel
{
    /// <summary>
    /// Gets or sets the provider configuration for a user.
    /// </summary>
    public Dictionary<UserProviderType, List<string>> ProviderConfiguration { get; set; } = [];
}