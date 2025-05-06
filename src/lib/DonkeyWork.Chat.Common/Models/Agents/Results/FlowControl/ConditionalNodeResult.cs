// ------------------------------------------------------
// <copyright file="ConditionalNodeResult.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Contracts;

namespace DonkeyWork.Chat.Common.Models.Agents.Results.FlowControl;

/// <summary>
/// A conditional node result.
/// </summary>
public class ConditionalNodeResult(IAgentNode node)
    : BaseFlowControlResult(node)
{
    /// <summary>
    /// Gets or sets the string formatter node inputs.
    /// </summary>
    required public List<BaseAgentNodeResult> Inputs { get; set; } = [];

    /// <summary>
    /// Gets or sets the string formatter node result.
    /// </summary>
    public List<Guid> NextNodeIds { get; set; } = [];

    /// <inheritdoc />
    public override string Text()
    {
        return string.Join(Environment.NewLine, this.Inputs.Select(x => x.Text()));
    }
}
