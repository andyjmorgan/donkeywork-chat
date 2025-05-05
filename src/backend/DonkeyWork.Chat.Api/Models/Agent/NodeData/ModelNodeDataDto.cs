// ------------------------------------------------------
// <copyright file="ModelNodeDataDto.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Models.Actions;
using DonkeyWork.Chat.Common.Models.Agents.Metadata;
using DonkeyWork.Chat.Common.Models.Providers;
using DonkeyWork.Chat.Common.Models.Providers.Tools;

namespace DonkeyWork.Chat.Api.Models.Agent.NodeData
{
    /// <summary>
    /// Model node data DTO.
    /// </summary>
    public class ModelNodeDataDto : BaseNodeDataDto
    {
        /// <summary>
        /// Gets or sets a list of allowed tools for this model.
        /// </summary>
        public List<string> AllowedTools { get; set; } = [];

        /// <summary>
        /// Gets or sets provider type.
        /// </summary>
        public AiChatProvider ProviderType { get; set; }

        /// <summary>
        /// Gets or sets name of the model to use.
        /// </summary>
        required public string ModelName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use streaming responses.
        /// </summary>
        public bool Streaming { get; set; }

        /// <summary>
        /// Gets or sets iDs of system prompts to include.
        /// </summary>
        public List<Guid> SystemPromptIds { get; set; } = [];

        /// <summary>
        /// Gets or sets model-specific parameters.
        /// </summary>
        public Dictionary<string, object> ModelParameters { get; set; } = [];

        /// <summary>
        /// Gets or sets whether to use dynamic tools.
        /// </summary>
        public bool DynamicTools { get; set; }

        /// <inheritdoc />
        internal override AgentNodeBaseMetadata GetMetadata()
        {
            return new AgentModelNodeMetadata()
            {
                AllowedTools = this.AllowedTools.Select(x => Enum.Parse<ToolProviderApplicationType>(x, ignoreCase: true)).ToList(),
                SystemPrompts = this.SystemPromptIds,
                IsDynamicTooling = this.DynamicTools,
                ModelConfiguration = new ModelConfiguration()
                {
                    Metadata = this.ModelParameters,
                    ModelName = this.ModelName,
                    ProviderType = this.ProviderType,
                    Streaming = this.Streaming,
                },
            };
        }
    }
}
