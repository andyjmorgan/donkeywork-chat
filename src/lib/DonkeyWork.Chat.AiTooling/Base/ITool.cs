// ------------------------------------------------------
// <copyright file="ITool.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Text.Json;
using DonkeyWork.Chat.AiTooling.Base.Models;
using DonkeyWork.Chat.Common.Providers;

namespace DonkeyWork.Chat.AiTooling.Base;

/// <summary>
/// A tool interface.
/// </summary>
public interface ITool
{
    /// <summary>
    /// Gets the tool definitions.
    /// </summary>
    /// <param name="toolProviderPosture">The user tool posture.</param>
    /// <returns>A list of <see cref="ToolDefinition"/>.</returns>
    public List<ToolDefinition> GetToolDefinitions(ToolProviderPosture toolProviderPosture);

    /// <summary>
    /// A function to invoke a function on the tool.
    /// </summary>
    /// <param name="functionName">The function name.</param>
    /// <param name="arguments">The arguments.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A response.</returns>
    public Task<JsonDocument?> InvokeFunctionAsync(string functionName, JsonDocument arguments, CancellationToken cancellationToken = default);

    /// <summary>
    /// A function to invoke a function on the tool.
    /// </summary>
    /// <param name="functionName">The function name.</param>
    /// <param name="arguments">The arguments.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A response.</returns>
    public Task<JsonDocument?> InvokeFunctionAsync(
        string functionName,
        IReadOnlyDictionary<string, JsonElement>? arguments,
        CancellationToken cancellationToken = default);
}