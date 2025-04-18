// ------------------------------------------------------
// <copyright file="SwarmpitClient.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Text.Json;
using System.Text.Json.Nodes;

namespace DonkeyWork.Chat.AiTooling.ToolImplementations.Swarmpit.Api
{
    /// <summary>
    /// Client for interacting with the Swarmpit API.
    /// </summary>
    public class SwarmpitClient : ISwarmpitClient
    {
        private readonly HttpClient httpClient;
        private readonly JsonDocumentOptions jsonOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="SwarmpitClient"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        public SwarmpitClient(HttpClient httpClient)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            this.jsonOptions = new JsonDocumentOptions
            {
                AllowTrailingCommas = true,
                CommentHandling = JsonCommentHandling.Skip,
            };
        }

        /// <inheritdoc/>
        public async Task<JsonNode> GetStatsAsync()
        {
            return await this.GetJsonAsync("/api/stats");
        }

        /// <inheritdoc/>
        public async Task<JsonNode> GetStacksAsync()
        {
            return await this.GetJsonAsync("/api/stacks");
        }

        /// <inheritdoc/>
        public async Task<JsonNode> GetStackAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Stack name cannot be empty", nameof(name));
            }

            return await this.GetJsonAsync($"/api/stacks/{name}");
        }

        /// <inheritdoc/>
        public async Task<JsonNode> GetServicesAsync()
        {
            return await this.GetJsonAsync("/api/services");
        }

        /// <inheritdoc/>
        public async Task<JsonNode> GetServiceAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Service ID cannot be empty", nameof(id));
            }

            return await this.GetJsonAsync($"/api/services/{id}");
        }

        /// <inheritdoc/>
        public async Task<JsonNode> GetNodesAsync()
        {
            return await this.GetJsonAsync("/api/nodes");
        }

        /// <inheritdoc/>
        public async Task<JsonNode> GetNodeAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Node ID cannot be empty", nameof(id));
            }

            return await this.GetJsonAsync($"/api/nodes/{id}");
        }

        /// <inheritdoc/>
        public async Task<JsonNode> GetNodeTasksAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Node ID cannot be empty", nameof(id));
            }

            return await this.GetJsonAsync($"/api/nodes/{id}/tasks");
        }

        /// <inheritdoc/>
        public async Task<JsonNode> GetNodeStatsAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Node ID cannot be empty", nameof(id));
            }

            return await this.GetJsonAsync($"/api/nodes/{id}/stats");
        }

        /// <inheritdoc/>
        public async Task<JsonNode> GetTasksAsync()
        {
            return await this.GetJsonAsync("/api/tasks");
        }

        /// <inheritdoc/>
        public async Task<JsonNode> GetTaskAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Task ID cannot be empty", nameof(id));
            }

            return await this.GetJsonAsync($"/api/tasks/{id}");
        }

        /// <inheritdoc/>
        public async Task<JsonNode> GetTaskTimeSeriesAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Task name cannot be empty", nameof(name));
            }

            return await this.GetJsonAsync($"/api/tasks/{name}/ts");
        }

        /// <summary>
        /// Helper method to perform GET requests and parse the response as JSON.
        /// </summary>
        /// <param name="endpoint">The API endpoint.</param>
        /// <returns>The JSON document.</returns>
        private async Task<JsonNode> GetJsonAsync(string endpoint)
        {
            var response = await this.httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            try
            {
                return JsonNode.Parse(content) ?? throw new InvalidOperationException($"Could not parse JSON response from {endpoint}");
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException($"Invalid JSON response from {endpoint}: {ex.Message}", ex);
            }
        }
    }
}