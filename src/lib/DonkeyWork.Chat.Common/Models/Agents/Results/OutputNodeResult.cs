// ------------------------------------------------------
// <copyright file="OutputNodeResult.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Contracts;

namespace DonkeyWork.Chat.Common.Models.Agents.Results;

/// <summary>
/// An output node result.
/// </summary>
public class OutputNodeResult(IAgentNode node)
    : BaseAgentNodeResult(node)
{
    /// <summary>
    /// Gets or sets the output node result.
    /// </summary>
    required public List<BaseAgentNodeResult> Inputs { get; set; } = [];

    /// <inheritdoc />
    public override string Text()
    {
        return string.Join(Environment.NewLine, this.Inputs.Select(x => x.Text()));
    }
}
