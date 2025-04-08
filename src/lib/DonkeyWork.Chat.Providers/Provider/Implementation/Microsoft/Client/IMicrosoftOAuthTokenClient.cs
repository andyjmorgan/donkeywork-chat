// ------------------------------------------------------
// <copyright file="IMicrosoftOAuthTokenClient.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Providers.Provider.Implementation.Microsoft.Models;

namespace DonkeyWork.Chat.Providers.Provider.Implementation.Microsoft.Client;

/// <summary>
/// An interface for Microsoft OAuth token client.
/// </summary>
public interface IMicrosoftOAuthTokenClient : IOAuthTokenClient<MicrosoftTokenResponse>
{
}