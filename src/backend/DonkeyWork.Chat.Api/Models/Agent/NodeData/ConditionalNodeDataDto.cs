// ------------------------------------------------------
// <copyright file="ConditionalNodeDataDto.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Models.Agents.Metadata;
using DonkeyWork.Chat.Common.Models.Agents.Models;

namespace DonkeyWork.Chat.Api.Models.Agent.NodeData
{
    /// <summary>
    /// Conditional node data DTO.
    /// </summary>
    public class ConditionalNodeDataDto : BaseNodeDataDto
    {
        /// <summary>
        /// Gets or sets list of conditional expressions.
        /// </summary>
        public List<ConditionItem> Conditions { get; set; } = [];

        /// <inheritdoc />
        internal override AgentNodeBaseMetadata GetMetadata()
        {
            return new AgentConditionNodeMetadata() { Conditions = this.Conditions, };
        }
    }
}
