// ------------------------------------------------------
// <copyright file="StringFormatterNodeDataDto.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Models.Agents.Metadata;

namespace DonkeyWork.Chat.Api.Models.Agent.NodeData
{
    /// <summary>
    /// String formatter node data DTO.
    /// </summary>
    public class StringFormatterNodeDataDto : BaseNodeDataDto
    {
        /// <summary>
        /// Gets or sets a template string for formatting.
        /// </summary>
        required public string Template { get; set; }

        /// <inheritdoc />
        internal override AgentNodeBaseMetadata GetMetadata()
        {
            return new AgentStringFormatterNodeMetadata()
            {
                Template = this.Template,
            };
        }
    }
}
