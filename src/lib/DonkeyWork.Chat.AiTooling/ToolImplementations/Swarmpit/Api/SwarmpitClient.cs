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
        public async Task<JsonNode> RedeployStackAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Stack name cannot be empty", nameof(name));
            }

            return await this.PostJsonAsync($"/api/stacks/{name}/redeploy", null);
        }

        /// <inheritdoc/>
        public async Task<JsonNode> DeactivateStackAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Stack name cannot be empty", nameof(name));
            }

            return await this.PostJsonAsync($"/api/stacks/{name}/deactivate", null);
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
        public async Task<JsonNode> RedeployServiceAsync(string id, string? tag = null)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Service ID cannot be empty", nameof(id));
            }

            var endpoint = $"/api/services/{id}/redeploy";
            if (!string.IsNullOrWhiteSpace(tag))
            {
                endpoint += $"?tag={Uri.EscapeDataString(tag)}";
            }

            return await this.PostJsonAsync(endpoint, null);
        }

        /// <inheritdoc/>
        public async Task<JsonNode> StopServiceAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Service ID cannot be empty", nameof(id));
            }

            return await this.PostJsonAsync($"/api/services/{id}/stop", null);
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

        /// <summary>
        /// Helper method to perform POST requests and parse the response as JSON.
        /// </summary>
        /// <param name="endpoint">The API endpoint.</param>
        /// <param name="data">The data to include in the request body, or null if no data.</param>
        /// <returns>The JSON document.</returns>
        private async Task<JsonNode> PostJsonAsync(string endpoint, object? data)
        {
            HttpResponseMessage response;
            
            if (data != null)
            {
                var jsonContent = JsonSerializer.Serialize(data);
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
                response = await this.httpClient.PostAsync(endpoint, content);
            }
            else
            {
                response = await this.httpClient.PostAsync(endpoint, null);
            }

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            
            // Check if the response is empty or whitespace
            if (string.IsNullOrWhiteSpace(responseContent))
            {
                // Return an empty JSON object for empty responses
                return JsonNode.Parse("{}") ?? throw new InvalidOperationException("Could not create empty JSON object");
            }
            
            try
            {
                return JsonNode.Parse(responseContent) ?? throw new InvalidOperationException($"Could not parse JSON response from {endpoint}");
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException($"Invalid JSON response from {endpoint}: {ex.Message}", ex);
            }
        }
    }
}