// ------------------------------------------------------
// <copyright file="SwarmpitTool.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.ComponentModel;
using System.Text.Json.Nodes;
using DonkeyWork.Chat.AiTooling.Attributes;
using DonkeyWork.Chat.AiTooling.ToolImplementations.Swarmpit.Api;
using DonkeyWork.Chat.Common.Providers.GenericProvider;

namespace DonkeyWork.Chat.AiTooling.ToolImplementations.Swarmpit.Tool
{
    /// <summary>
    /// Tool for interacting with Docker Swarm via Swarmpit API.
    /// </summary>
    [GenericToolProvider(GenericProviderType.Swarmpit)]
    public class SwarmpitTool : Base.Tool, ISwarmpitTool
    {
        private readonly ISwarmpitClientFactory clientFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="SwarmpitTool"/> class.
        /// </summary>
        /// <param name="clientFactory">The client factory.</param>
        public SwarmpitTool(ISwarmpitClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
        }

        /// <inheritdoc/>
        [ToolFunction]
        [Description("Get cluster statistics.")]
        public async Task<JsonNode> SwarmPit_GetClusterStatsAsync()
        {
            var client = await this.clientFactory.CreateClient();
            return await client.GetStatsAsync();
        }

        /// <inheritdoc/>
        [ToolFunction]
        [Description("Get a list of all stacks.")]
        public async Task<JsonNode> SwarmPit_GetStacksAsync()
        {
            var client = await this.clientFactory.CreateClient();
            return await client.GetStacksAsync();
        }

        /// <inheritdoc/>
        [ToolFunction]
        [Description("Get detailed information for a specific stack.")]
        public async Task<JsonNode> SwarmPit_GetStackAsync(
            [Description("The name of the stack.")]
            string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Stack name cannot be empty", nameof(name));
            }

            var client = await this.clientFactory.CreateClient();
            return await client.GetStackAsync(name);
        }

        /// <inheritdoc/>
        [ToolFunction]
        [Description("Get a list of all services.")]
        public async Task<JsonNode> SwarmPit_GetServicesAsync()
        {
            var client = await this.clientFactory.CreateClient();
            return await client.GetServicesAsync();
        }

        /// <inheritdoc/>
        [ToolFunction]
        [Description("Get detailed information for a specific service.")]
        public async Task<JsonNode> SwarmPit_GetServiceAsync(
            [Description("The ID of the service.")]
            string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Service ID cannot be empty", nameof(id));
            }

            var client = await this.clientFactory.CreateClient();
            return await client.GetServiceAsync(id);
        }

        /// <inheritdoc/>
        [ToolFunction]
        [Description("Get a list of all nodes.")]
        public async Task<JsonNode> SwarmPit_GetNodesAsync()
        {
            var client = await this.clientFactory.CreateClient();
            return await client.GetNodesAsync();
        }

        /// <inheritdoc/>
        [ToolFunction]
        [Description("Get detailed information for a specific node.")]
        public async Task<JsonNode> SwarmPit_GetNodeAsync(
            [Description("The ID of the node.")] string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Node ID cannot be empty", nameof(id));
            }

            var client = await this.clientFactory.CreateClient();
            return await client.GetNodeAsync(id);
        }

        /// <inheritdoc/>
        [ToolFunction]
        [Description("Get tasks running on a specific node.")]
        public async Task<JsonNode> SwarmPit_GetNodeTasksAsync(
            [Description("The ID of the node.")] string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Node ID cannot be empty", nameof(id));
            }

            var client = await this.clientFactory.CreateClient();
            return await client.GetNodeTasksAsync(id);
        }

        /// <inheritdoc/>
        [ToolFunction]
        [Description("Get statistics for a specific node.")]
        public async Task<JsonNode> SwarmPit_GetNodeStatsAsync(
            [Description("The ID of the node.")] string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Node ID cannot be empty", nameof(id));
            }

            var client = await this.clientFactory.CreateClient();
            return await client.GetNodeStatsAsync(id);
        }

        /// <inheritdoc/>
        [ToolFunction]
        [Description("Get a list of all tasks.")]
        public async Task<JsonNode> SwarmPit_GetTasksAsync()
        {
            var client = await this.clientFactory.CreateClient();
            return await client.GetTasksAsync();
        }

        /// <inheritdoc/>
        [ToolFunction]
        [Description("Get detailed information for a specific task.")]
        public async Task<JsonNode> SwarmPit_GetTaskAsync(
            [Description("The ID of the task.")] string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Task ID cannot be empty", nameof(id));
            }

            var client = await this.clientFactory.CreateClient();
            return await client.GetTaskAsync(id);
        }

        /// <inheritdoc/>
        [ToolFunction]
        [Description("Get time series data for a specific task.")]
        public async Task<JsonNode> SwarmPit_GetTaskTimeSeriesAsync(
            [Description("The name of the task.")] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Task name cannot be empty", nameof(name));
            }

            var client = await this.clientFactory.CreateClient();
            return await client.GetTaskTimeSeriesAsync(name);
        }
    }
}