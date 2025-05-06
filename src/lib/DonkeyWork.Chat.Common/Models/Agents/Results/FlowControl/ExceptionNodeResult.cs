// ------------------------------------------------------
// <copyright file="ExceptionNodeResult.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Text.Json.Serialization;
using DonkeyWork.Chat.Common.Contracts;

namespace DonkeyWork.Chat.Common.Models.Agents.Results.FlowControl;

/// <summary>
/// An exception node result.
/// </summary>
public class ExceptionNodeResult(IAgentNode node)
    : BaseAgentNodeResult(node)
{
    /// <summary>
    /// Gets or sets the exception node result.
    /// </summary>
    [JsonIgnore]
    public Exception? Exception { get; set; }

    /// <summary>
    /// Gets or sets the exception message.
    /// </summary>
    required public string Message { get; set; }

    /// <inheritdoc />
    public override string Text()
    {
        return this.Message;
    }
}
