// ------------------------------------------------------
// <copyright file="ITool.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Text.Json;
using DonkeyWork.Chat.AiTooling.Base.Models;
using DonkeyWork.Chat.Common.Models.Providers.Posture;

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

    /// <summary>
    /// Creates a successful task response.
    /// </summary>
    /// <param name="functionName">The function name.</param>
    /// <param name="successful">A boolean value to determine if the task was successful.</param>
    /// <param name="metadata">A metadata dictionary.</param>
    /// <param name="errorMessage">An error Message.</param>
    /// <returns>A task response.</returns>
    protected object CreateTaskResponse(
        string functionName,
        bool successful,
        Dictionary<string, string>? metadata = null,
        string? errorMessage = null);
}