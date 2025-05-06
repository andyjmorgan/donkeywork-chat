// ------------------------------------------------------
// <copyright file="AgentInputDetails.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.AiServices.Clients.Models;
using DonkeyWork.Chat.Common.Models.Chat;

namespace DonkeyWork.Workflows.Core.Agents.Execution;

/// <summary>
/// Gets or sets the agent input details.
/// </summary>
public class AgentInputDetails
{
    /// <summary>
    /// Gets or sets the request headers.
    /// </summary>
    public Dictionary<string, string> Headers { get; set; } = [];

    /// <summary>
    /// Gets or sets the request messages.
    /// </summary>
    public List<GenericChatMessage> Messages { get; set; } = [];

    /// <summary>
    /// Gets the request message history.
    /// </summary>
    public List<GenericChatMessage> MessageHistory => this.Messages.Except([this.LastMessage]).ToList();

    /// <summary>
    /// Gets the most recent request message.
    /// </summary>
    public GenericChatMessage LastMessage =>
        this.Messages.Last(x => x.Role == GenericMessageRole.User);
}
