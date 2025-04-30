// ------------------------------------------------------
// <copyright file="IToolService.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.AiTooling.Base;
using DonkeyWork.Chat.AiTooling.Base.Models;
using DonkeyWork.Chat.Common.Models.Providers.Posture;

namespace DonkeyWork.Chat.AiTooling.Services;

/// <summary>
/// A tool service interface.
/// </summary>
public interface IToolService
{
    /// <summary>
    /// Gets public tools available to users.
    /// </summary>
    /// <returns>A list of <see cref="ITool"/>.</returns>
    public List<ITool> GetPublicTools();

    /// <summary>
    /// Gets a list of user scoped tools.
    /// </summary>
    /// <param name="toolPosture">The users tool posture.</param>
    /// <returns>A list of <see cref="ToolDefinition"/>.</returns>
    public List<ToolDefinition> GetUserScopedTools(ToolProviderPosture toolPosture);
}