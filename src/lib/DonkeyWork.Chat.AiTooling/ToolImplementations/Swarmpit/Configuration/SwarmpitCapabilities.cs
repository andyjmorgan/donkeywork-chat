// ------------------------------------------------------
// <copyright file="SwarmpitCapabilities.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.ComponentModel;

namespace DonkeyWork.Chat.AiTooling.ToolImplementations.Swarmpit.Configuration;

/// <summary>
/// An enumeration representing the capabilities of a Swarmpit provider.
/// </summary>
public enum SwarmpitCapabilities
{
    /// <summary>
    /// The provider can list all services.
    /// </summary>
    [Description("Get performance statistics for all services")]
    Statistics,

    /// <summary>
    /// The provider can list all containers.
    /// </summary>
    [Description("Manage containers")]
    Containers,

    /// <summary>
    /// The provider can list all stacks.
    /// </summary>
    [Description("Manage stacks")]
    Stacks,
}