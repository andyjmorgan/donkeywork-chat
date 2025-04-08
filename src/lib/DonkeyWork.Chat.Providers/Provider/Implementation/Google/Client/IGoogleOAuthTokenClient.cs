// ------------------------------------------------------
// <copyright file="IGoogleOAuthTokenClient.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Providers.Provider.Implementation.Google.Models;

namespace DonkeyWork.Chat.Providers.Provider.Implementation.Google.Client;

/// <summary>
/// An interface for Google OAuth token client.
/// </summary>
public interface IGoogleOAuthTokenClient : IOAuthTokenClient<GoogleTokenResponse>
{
}