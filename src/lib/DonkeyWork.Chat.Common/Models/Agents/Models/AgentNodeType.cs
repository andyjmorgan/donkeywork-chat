// ------------------------------------------------------
// <copyright file="AgentNodeType.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.Common.Models.Agents.Models;

/// <summary>
/// An agent node type.
/// </summary>
public enum AgentNodeType
{
    /// <summary>
    /// Unspecified agent node type.
    /// </summary>
    None,

    /// <summary>
    /// Input.
    /// </summary>
    Input,

    /// <summary>
    /// Output.
    /// </summary>
    Output,

    /// <summary>
    /// Model.
    /// </summary>
    Model,

    /// <summary>
    /// Conditional.
    /// </summary>
    Conditional,

    /// <summary>
    /// String formatter.
    /// </summary>
    StringFormatter,
}
