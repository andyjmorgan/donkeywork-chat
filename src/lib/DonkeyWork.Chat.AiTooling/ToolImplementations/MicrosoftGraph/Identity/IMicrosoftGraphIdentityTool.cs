// ------------------------------------------------------
// <copyright file="IMicrosoftGraphIdentityTool.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Text.Json;

namespace DonkeyWork.Chat.AiTooling.ToolImplementations.MicrosoftGraph.Identity;

/// <summary>
/// A tool class for microsoft graph, to get the current users information.
/// </summary>
public interface IMicrosoftGraphIdentityTool
{
    /// <summary>
    /// Gets the current user.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task<JsonDocument> GetUserInformationAsync(CancellationToken cancellationToken = default);
}