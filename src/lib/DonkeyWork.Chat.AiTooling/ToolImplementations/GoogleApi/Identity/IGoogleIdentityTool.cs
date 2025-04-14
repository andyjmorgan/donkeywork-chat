// ------------------------------------------------------
// <copyright file="IGoogleIdentityTool.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Text.Json;
using DonkeyWork.Chat.AiTooling.Base;

namespace DonkeyWork.Chat.AiTooling.ToolImplementations.GoogleApi.Identity;

/// <summary>
/// A tool class for Google Identity, to get the current user's information.
/// </summary>
public interface IGoogleIdentityTool : ITool
{
    /// <summary>
    /// Gets the current user's Google identity information.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<JsonDocument?> GetGoogleIdentityAsync(
        CancellationToken cancellationToken = default);
}