// ------------------------------------------------------
// <copyright file="ActionMapper.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using AutoMapper;
using DonkeyWork.Persistence.Agent.Repository.Action.Models;

namespace DonkeyWork.Chat.Api.Models.Action;

/// <summary>
/// Mapper for action models.
/// </summary>
public class ActionMapper : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ActionMapper"/> class.
    /// </summary>
    public ActionMapper()
    {
        this.CreateMap<GetActionsResponseItem, GetActionsModel>()
            .ForMember(dest => dest.Actions, opt => opt.MapFrom(src => src.Actions));

        this.CreateMap<ActionItem, GetActionsItemModel>();

        this.CreateMap<UpsertActionModel, UpsertActionItem>();
    }
}