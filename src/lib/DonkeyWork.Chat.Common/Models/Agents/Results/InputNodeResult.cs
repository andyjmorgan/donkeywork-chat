// ------------------------------------------------------
// <copyright file="InputNodeResult.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Contracts;
using DonkeyWork.Chat.Common.Models.Chat;

namespace DonkeyWork.Chat.Common.Models.Agents.Results;

/// <summary>
/// An input node result.
/// </summary>
public class InputNodeResult(IAgentNode node)
    : BaseAgentNodeResult(node)
{
    /// <summary>
    /// Gets or sets the input node result.
    /// </summary>
    required public GenericChatMessage Message { get; set; }

    /// <inheritdoc />
    public override string Text()
    {
        return this.Message.Content;
    }
}
