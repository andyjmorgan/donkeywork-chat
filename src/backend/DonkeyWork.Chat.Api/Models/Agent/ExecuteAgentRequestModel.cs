// ------------------------------------------------------
// <copyright file="ExecuteAgentRequestModel.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.AiServices.Clients.Models;
using DonkeyWork.Chat.Common.Models.Chat;

namespace DonkeyWork.Chat.Api.Models.Agent;

/// <summary>
/// A request model for executing an agent.
/// </summary>
public class ExecuteAgentRequestModel
{
    /// <summary>
    /// Gets the messages.
    /// </summary>
    public List<GenericChatMessage> Messages { get; init; } = [];

    /// <summary>
    /// Gets a value indicating whether the agent should stream.
    /// </summary>
    public bool Stream { get; init; } = false;
}
