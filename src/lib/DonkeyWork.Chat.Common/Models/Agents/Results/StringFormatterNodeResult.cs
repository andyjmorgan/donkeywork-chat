// ------------------------------------------------------
// <copyright file="StringFormatterNodeResult.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Contracts;

namespace DonkeyWork.Chat.Common.Models.Agents.Results;

/// <summary>
/// A string formatter node result.
/// </summary>
public class StringFormatterNodeResult(IAgentNode agentNode)
    : BaseAgentNodeResult(agentNode)
{
    /// <summary>
    /// Gets or sets the string formatter node inputs.
    /// </summary>
    required public List<BaseAgentNodeResult> Inputs { get; set; } = [];

    /// <summary>
    /// Gets or sets the string formatter node result.
    /// </summary>
    required public string FormattedText { get; set; }

    /// <inheritdoc />
    public override string Text()
    {
        return this.FormattedText;
    }
}
