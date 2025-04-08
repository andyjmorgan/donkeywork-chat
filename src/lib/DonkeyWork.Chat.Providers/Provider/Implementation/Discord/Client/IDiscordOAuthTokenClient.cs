// ------------------------------------------------------
// <copyright file="IDiscordOAuthTokenClient.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Providers.Provider.Implementation.Discord.Models;

namespace DonkeyWork.Chat.Providers.Provider.Implementation.Discord.Client;

/// <summary>
/// An interface for Discord OAuth token client.
/// </summary>
public interface IDiscordOAuthTokenClient : IOAuthTokenClient<DiscordTokenResponse>
{
}