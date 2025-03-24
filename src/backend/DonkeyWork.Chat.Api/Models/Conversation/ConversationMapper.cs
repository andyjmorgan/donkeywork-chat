// ------------------------------------------------------
// <copyright file="ConversationMapper.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using AutoMapper;
using DonkeyWork.Chat.Persistence.Repository.Conversation.Models;

namespace DonkeyWork.Chat.Api.Models.Conversation;

/// <summary>
/// A Profile for mapping conversations.
/// </summary>
public class ConversationMapper : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConversationMapper"/> class.
    /// </summary>
    public ConversationMapper()
    {
        this.CreateMap<GetConversationsResponse, GetConversationsModel>();
        this.CreateMap<ConversationsItem, GetConversationsItemModel>();
        this.CreateMap<ConversationItem, GetConversationModel>();
        this.CreateMap<ConversationMessageItem, GetConversationMessageModel>();
    }
}