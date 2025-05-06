// ------------------------------------------------------
// <copyright file="UserPosture.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Models.Providers.Posture;

namespace DonkeyWork.Workflows.Core.Agents.Execution;

/// <summary>
/// The users posture.
/// </summary>
public class UserPosture
{
    /// <summary>
    /// Gets or sets the user provider posture.
    /// </summary>
    public List<UserProviderPosture> UserProviderPosture { get; set; } = [];

    /// <summary>
    /// Gets or sets the provider posture.
    /// </summary>
    public List<GenericProviderPosture> ProviderPosture { get; set; } = [];
}
