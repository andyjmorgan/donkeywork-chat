// ------------------------------------------------------
// <copyright file="ISwarmpitClient.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Text.Json.Nodes;

namespace DonkeyWork.Chat.AiTooling.ToolImplementations.Swarmpit.Api
{
    /// <summary>
    /// Interface for the Swarmpit API client.
    /// </summary>
    public interface ISwarmpitClient
    {
        /// <summary>
        /// Gets cluster statistics.
        /// </summary>
        /// <returns>The cluster statistics as a JSON document.</returns>
        Task<JsonNode> GetStatsAsync();

        /// <summary>
        /// Gets a list of all stacks.
        /// </summary>
        /// <returns>A list of stacks as a JSON document.</returns>
        Task<JsonNode> GetStacksAsync();

        /// <summary>
        /// Gets detailed information for a specific stack.
        /// </summary>
        /// <param name="name">The name of the stack.</param>
        /// <returns>The stack details as a JSON document.</returns>
        Task<JsonNode> GetStackAsync(string name);

        /// <summary>
        /// Redeploys a stack.
        /// </summary>
        /// <param name="name">The name of the stack.</param>
        /// <returns>The response as a JSON document.</returns>
        Task<JsonNode> RedeployStackAsync(string name);

        /// <summary>
        /// Deactivates a stack.
        /// </summary>
        /// <param name="name">The name of the stack.</param>
        /// <returns>The response as a JSON document.</returns>
        Task<JsonNode> DeactivateStackAsync(string name);

        /// <summary>
        /// Gets a list of all services.
        /// </summary>
        /// <returns>A list of services as a JSON document.</returns>
        Task<JsonNode> GetServicesAsync();

        /// <summary>
        /// Gets detailed information for a specific service.
        /// </summary>
        /// <param name="id">The ID of the service.</param>
        /// <returns>The service details as a JSON document.</returns>
        Task<JsonNode> GetServiceAsync(string id);

        /// <summary>
        /// Redeploys a service.
        /// </summary>
        /// <param name="id">The ID of the service.</param>
        /// <param name="tag">Optional tag to specify for the redeployment.</param>
        /// <returns>The response as a JSON document.</returns>
        Task<JsonNode> RedeployServiceAsync(string id, string? tag = null);

        /// <summary>
        /// Stops a service.
        /// </summary>
        /// <param name="id">The ID of the service.</param>
        /// <returns>The response as a JSON document.</returns>
        Task<JsonNode> StopServiceAsync(string id);

        /// <summary>
        /// Gets a list of all nodes.
        /// </summary>
        /// <returns>A list of nodes as a JSON document.</returns>
        Task<JsonNode> GetNodesAsync();

        /// <summary>
        /// Gets detailed information for a specific node.
        /// </summary>
        /// <param name="id">The ID of the node.</param>
        /// <returns>The node details as a JSON document.</returns>
        Task<JsonNode> GetNodeAsync(string id);

        /// <summary>
        /// Gets tasks running on a specific node.
        /// </summary>
        /// <param name="id">The ID of the node.</param>
        /// <returns>A list of tasks running on the node as a JSON document.</returns>
        Task<JsonNode> GetNodeTasksAsync(string id);

        /// <summary>
        /// Gets statistics for a specific node.
        /// </summary>
        /// <param name="id">The ID of the node.</param>
        /// <returns>The node statistics as a JSON document.</returns>
        Task<JsonNode> GetNodeStatsAsync(string id);

        /// <summary>
        /// Gets a list of all tasks.
        /// </summary>
        /// <returns>A list of tasks as a JSON document.</returns>
        Task<JsonNode> GetTasksAsync();

        /// <summary>
        /// Gets detailed information for a specific task.
        /// </summary>
        /// <param name="id">The ID of the task.</param>
        /// <returns>The task details as a JSON document.</returns>
        Task<JsonNode> GetTaskAsync(string id);

        /// <summary>
        /// Gets time series data for a specific task.
        /// </summary>
        /// <param name="name">The name of the task.</param>
        /// <returns>The task time series data as a JSON document.</returns>
        Task<JsonNode> GetTaskTimeSeriesAsync(string name);
    }
}