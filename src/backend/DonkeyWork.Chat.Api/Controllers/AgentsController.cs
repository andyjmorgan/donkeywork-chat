// ------------------------------------------------------
// <copyright file="AgentsController.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Api.Models.Agent;
using DonkeyWork.Persistence.Agent.Repository.Agent;
using DonkeyWork.Persistence.Common.Common;
using Microsoft.AspNetCore.Mvc;

namespace DonkeyWork.Chat.Api.Controllers;

/// <summary>
/// Handles agents.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AgentsController(IAgentRepository agentRepository)
    : ControllerBase
{
    /// <summary>
    /// Gets a paged list of agents.
    /// </summary>
    /// <param name="pagingParameters">The paging parameters.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="GetAgentsModel"/>.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetAgentsModel))]
    public async Task<IActionResult> GetAgentsAsync([FromQuery] PagingParameters pagingParameters, CancellationToken cancellationToken)
    {
        var result = await agentRepository.GetAgentsAsync(pagingParameters, cancellationToken);
        return this.Ok(new GetAgentsModel()
        {
            TotalCount = result.TotalCount,
            Agents = result.Agents.Select(GetAgentsItemModel.CreateFromAgent).ToList(),
        });
    }

    /// <summary>
    /// Gets an agent by id.
    /// </summary>
    /// <param name="id">The agent id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="GetAgentModel"/>.</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetAgentModel))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAgentByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var agent = await agentRepository.GetAgentByIdAsync(id, cancellationToken);
        if (agent == null)
        {
            return this.NotFound();
        }

        return this.Ok(GetAgentModel.FromAgentItem(agent));
    }

    /// <summary>
    /// Gets an agent by name.
    /// </summary>
    /// <param name="name">The agent name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="GetAgentModel"/>.</returns>
    [HttpGet("name/{name}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetAgentModel))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAgentByNameAsync(string name, CancellationToken cancellationToken)
    {
        var agent = await agentRepository.GetAgentByNameAsync(name, cancellationToken);
        if (agent == null)
        {
            return this.NotFound();
        }

        return this.Ok(GetAgentModel.FromAgentItem(agent));
    }

    /// <summary>
    /// Deletes an agent.
    /// </summary>
    /// <param name="id">The agent id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAgentAsync(Guid id, CancellationToken cancellationToken)
    {
        return await agentRepository.DeleteAgentAsync(id, cancellationToken)
            ? this.NoContent()
            : this.NotFound();
    }

    /// <summary>
    /// Creates a new agent.
    /// </summary>
    /// <param name="agent">The agent.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The ID of the newly created agent.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Guid))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddAgentAsync([FromBody] UpsertAgentModel agent, CancellationToken cancellationToken)
    {
        try
        {
            var agentId = await agentRepository.UpdateAgentAsync(agent.Id, agent.ToAgentItem(), cancellationToken);
            return this.Created($"/api/agents/{agentId}", agentId);
        }
        catch (Exception ex)
        {
            return this.BadRequest();
        }
    }
}
