// ------------------------------------------------------
// <copyright file="InputNodeDataDto.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Models.Agents.Metadata;

namespace DonkeyWork.Chat.Api.Models.Agent.NodeData
{
    /// <summary>
    /// Input node data DTO.
    /// </summary>
    public class InputNodeDataDto : BaseNodeDataDto
    {
        /// <inheritdoc />
        internal override AgentNodeBaseMetadata GetMetadata()
        {
            return new AgentInputNodeMetadata();
        }
    }
}
