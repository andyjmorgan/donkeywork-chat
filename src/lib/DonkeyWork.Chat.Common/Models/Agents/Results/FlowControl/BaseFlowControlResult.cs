// ------------------------------------------------------
// <copyright file="BaseFlowControlResult.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Contracts;

namespace DonkeyWork.Chat.Common.Models.Agents.Results.FlowControl;

/// <inheritdoc />
public abstract class BaseFlowControlResult(IAgentNode node)
    : BaseAgentNodeResult(node)
{
}
