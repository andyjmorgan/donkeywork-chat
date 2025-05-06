// ------------------------------------------------------
// <copyright file="AgentVariableContext.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Models.Agents.Results;
using DonkeyWork.Workflows.Core.Agents.Execution;

namespace DonkeyWork.Workflows.Core.Agents.Nodes;

/// <summary>
/// An agent execution context.
/// </summary>
public class AgentVariableContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AgentVariableContext"/> class.
    /// </summary>
    /// <param name="inputs">The step inputs.</param>
    /// <param name="agentContext">The agent context.</param>
    public AgentVariableContext(List<BaseAgentNodeResult> inputs, IAgentContext agentContext)
    {
        this.Input = new AgentInputContext(inputs.ToDictionary(x => x.Name));
        this.AgentInputDetails = agentContext.InputDetails;
        this.AgentExecutionDetails = agentContext.ExecutionDetails;
        this.AgentResults = this.AgentResults.Select(x => x.Value).ToDictionary(x => x.Name);
    }

    /// <summary>
    /// Gets or sets the agent node input results.
    /// </summary>
    public AgentInputContext Input { get; set; }

    /// <summary>
    /// Gets or sets the agent request details.
    /// </summary>
    public AgentInputDetails AgentInputDetails { get; set; }

    /// <summary>
    /// Gets or sets the agent results from all executed steps.
    /// </summary>
    public Dictionary<string, BaseAgentNodeResult> AgentResults { get; set; } = [];

    /// <summary>
    /// Gets or sets the agent execution details.
    /// </summary>
    public AgentExecutionDetails AgentExecutionDetails { get; set; }
}


/// <summary>
/// An agent input context.
/// </summary>
public class AgentInputContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AgentInputContext"/> class.
    /// </summary>
    /// <param name="items">The input.</param>
    public AgentInputContext(Dictionary<string, BaseAgentNodeResult> items)
    {
        this.Items = items;
    }

    /// <summary>
    /// The input dictionary.
    /// </summary>
    public Dictionary<string, BaseAgentNodeResult> Items { get; }

    /// <summary>
    /// Gets all the input values as a string.
    /// </summary>
    public string Text => string.Join(Environment.NewLine, this.Items.Values.Select(x => x.Text()));
}
