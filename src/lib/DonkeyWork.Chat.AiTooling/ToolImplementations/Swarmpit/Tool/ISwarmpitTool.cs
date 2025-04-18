// ------------------------------------------------------
// <copyright file="ISwarmpitTool.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Text.Json.Nodes;
using DonkeyWork.Chat.AiTooling.Base;

namespace DonkeyWork.Chat.AiTooling.ToolImplementations.Swarmpit.Tool
{
    /// <summary>
    /// Interface for the Swarmpit tool.
    /// </summary>
    public interface ISwarmpitTool : ITool
    {
        /// <summary>
        /// Gets cluster statistics.
        /// </summary>
        /// <returns>The cluster statistics as a JSON document.</returns>
        Task<JsonNode> SwarmPit_GetClusterStatsAsync();

        /// <summary>
        /// Gets a list of all stacks.
        /// </summary>
        /// <returns>A list of stacks as a JSON document.</returns>
        Task<JsonNode> SwarmPit_GetStacksAsync();

        /// <summary>
        /// Gets detailed information for a specific stack.
        /// </summary>
        /// <param name="name">The name of the stack.</param>
        /// <returns>The stack details as a JSON document.</returns>
        Task<JsonNode> SwarmPit_GetStackAsync(string name);

        /// <summary>
        /// Redeploys a stack with all of its services.
        /// </summary>
        /// <param name="name">The name of the stack to redeploy.</param>
        /// <returns>Response as a JSON document.</returns>
        Task<JsonNode> SwarmPit_RedeployStackAsync(string name);

        /// <summary>
        /// Deactivates (stops) a stack and all of its services.
        /// </summary>
        /// <param name="name">The name of the stack to deactivate.</param>
        /// <returns>Response as a JSON document.</returns>
        Task<JsonNode> SwarmPit_DeactivateStackAsync(string name);

        /// <summary>
        /// Gets a list of all services.
        /// </summary>
        /// <returns>A list of services as a JSON document.</returns>
        Task<JsonNode> SwarmPit_GetServicesAsync();

        /// <summary>
        /// Gets detailed information for a specific service.
        /// </summary>
        /// <param name="id">The ID of the service.</param>
        /// <returns>The service details as a JSON document.</returns>
        Task<JsonNode> SwarmPit_GetServiceAsync(string id);

        /// <summary>
        /// Redeploys a service with the same or new image tag.
        /// </summary>
        /// <param name="id">The ID of the service to redeploy.</param>
        /// <param name="tag">Optional tag to use for redeployment. If not specified, uses the current image tag.</param>
        /// <returns>Response as a JSON document.</returns>
        Task<JsonNode> SwarmPit_RedeployServiceAsync(string id, string? tag = null);

        /// <summary>
        /// Stops a running service.
        /// </summary>
        /// <param name="id">The ID of the service to stop.</param>
        /// <returns>Response as a JSON document.</returns>
        Task<JsonNode> SwarmPit_StopServiceAsync(string id);

        /// <summary>
        /// Gets a list of all nodes.
        /// </summary>
        /// <returns>A list of nodes as a JSON document.</returns>
        Task<JsonNode> SwarmPit_GetNodesAsync();

        /// <summary>
        /// Gets detailed information for a specific node.
        /// </summary>
        /// <param name="id">The ID of the node.</param>
        /// <returns>The node details as a JSON document.</returns>
        Task<JsonNode> SwarmPit_GetNodeAsync(string id);

        /// <summary>
        /// Gets tasks running on a specific node.
        /// </summary>
        /// <param name="id">The ID of the node.</param>
        /// <returns>A list of tasks running on the node as a JSON document.</returns>
        Task<JsonNode> SwarmPit_GetNodeTasksAsync(string id);

        /// <summary>
        /// Gets statistics for a specific node.
        /// </summary>
        /// <param name="id">The ID of the node.</param>
        /// <returns>The node statistics as a JSON document.</returns>
        Task<JsonNode> SwarmPit_GetNodeStatsAsync(string id);

        /// <summary>
        /// Gets a list of all tasks.
        /// </summary>
        /// <returns>A list of tasks as a JSON document.</returns>
        Task<JsonNode> SwarmPit_GetTasksAsync();

        /// <summary>
        /// Gets detailed information for a specific task.
        /// </summary>
        /// <param name="id">The ID of the task.</param>
        /// <returns>The task details as a JSON document.</returns>
        Task<JsonNode> SwarmPit_GetTaskAsync(string id);

        /// <summary>
        /// Gets time series data for a specific task.
        /// </summary>
        /// <param name="name">The name of the task.</param>
        /// <returns>The task time series data as a JSON document.</returns>
        Task<JsonNode> SwarmPit_GetTaskTimeSeriesAsync(string name);
    }
}