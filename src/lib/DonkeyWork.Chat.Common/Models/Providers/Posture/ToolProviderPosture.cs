// ------------------------------------------------------
// <copyright file="ToolProviderPosture.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.Common.Models.Providers.Posture;

/// <summary>
/// A tool provider posture.
/// </summary>
public class ToolProviderPosture
{
    /// <summary>
    /// Gets or sets the users postures with oauth tokens.
    /// </summary>
    public List<UserProviderPosture> UserTokens { get; set; } = [];

    /// <summary>
    /// Gets or sets the generic postures with providers.
    /// </summary>
    public List<GenericProviderPosture> GenericIntegrations { get; set; } = [];
}