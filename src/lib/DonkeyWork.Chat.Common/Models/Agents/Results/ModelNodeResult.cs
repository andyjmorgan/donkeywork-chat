// ------------------------------------------------------
// <copyright file="ModelNodeResult.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Contracts;

namespace DonkeyWork.Chat.Common.Models.Agents.Results;

/// <summary>
/// A model node result.
/// </summary>
public class ModelNodeResult(IAgentNode node)
    : BaseAgentNodeResult(node)
{
    /// <summary>
    /// Gets or sets the message.
    /// </summary>
    required public string Message { get; set; }

    /// <summary>
    /// Gets or sets the model node result.
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = [];

    /// <summary>
    ///  Gets or sets a value indicating whether the result was streamed.
    /// </summary>
    public bool WasStreamed { get; set; }

    /// <inheritdoc />
    public override string Text()
    {
        return this.Message;
    }
}
