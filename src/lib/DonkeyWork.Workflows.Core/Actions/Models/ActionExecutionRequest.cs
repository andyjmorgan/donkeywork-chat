// ------------------------------------------------------
// <copyright file="ActionExecutionRequest.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Workflows.Core.Actions.Models;

/// <summary>
/// A request to execute an action.
/// </summary>
public record ActionExecutionRequest
{
    /// <summary>
    /// Gets the Action id.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Gets the action name.
    /// </summary>
    public string ActionName { get; init; } = string.Empty;

    /// <summary>
    /// Gets the user id.
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// Gets the action input.
    /// </summary>
    public string ActionInput { get; init; } = string.Empty;

    /// <summary>
    /// Gets the action Execution id.
    /// </summary>
    public Guid ExecutionId { get; init; } = Guid.NewGuid();
}