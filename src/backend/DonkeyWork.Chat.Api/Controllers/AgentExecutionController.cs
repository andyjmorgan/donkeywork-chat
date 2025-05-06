// ------------------------------------------------------
// <copyright file="AgentExecutionController.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Text.Json;
using DonkeyWork.Chat.Api.Models.Agent;
using DonkeyWork.Workflows.Core.Agents.Orchestrator;
using DonkeyWork.Workflows.Core.Agents.Stream;
using Microsoft.AspNetCore.Mvc;

namespace DonkeyWork.Chat.Api.Controllers;

/// <summary>
/// A controller for executing agents.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AgentExecutionController(IAgentOrchestrator agentOrchestrator, IStreamService streamService)
    : ControllerBase
{
    /// <summary>
    /// Posts an agent id to execute.
    /// </summary>
    /// <param name="id">The agent id.</param>
    /// <param name="request">The request.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [HttpPost("{id}")]
    public async Task ExecuteAgentAsync(
        [FromRoute] Guid id,
        [FromBody] ExecuteAgentRequestModel request)
    {
        // Set up SSE response
        this.HttpContext.Response.Headers.Append("Content-Type", "text/event-stream");
        this.HttpContext.Response.Headers.Append("Cache-Control", "no-cache");
        this.HttpContext.Response.Headers.Append("Connection", "keep-alive");

        var executionJob = agentOrchestrator.ExecuteAsync(
            id,
            request.Messages,
            this.HttpContext.RequestAborted);

        await foreach (var streamItem in streamService.StreamAsync(this.HttpContext.RequestAborted))
        {
            await this.HttpContext.Response.WriteAsync(
                $"event: {streamItem.GetType().Name}{Environment.NewLine}",
                this.HttpContext.RequestAborted);

            await this.HttpContext.Response.WriteAsync(
                $"data: {JsonSerializer.Serialize(streamItem)}",
                this.HttpContext.RequestAborted);

            await this.HttpContext.Response.WriteAsync(
                $"{Environment.NewLine}{Environment.NewLine}",
                this.HttpContext.RequestAborted);

            await this.HttpContext.Response.Body.FlushAsync(this.HttpContext.RequestAborted);
        }
    }
}
