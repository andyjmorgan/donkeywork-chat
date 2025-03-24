// ------------------------------------------------------
// <copyright file="PromptMapper.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using AutoMapper;
using DonkeyWork.Chat.Persistence.Repository.Prompt.Models;

namespace DonkeyWork.Chat.Api.Models.Prompt;

/// <summary>
/// A prompt mapper.
/// </summary>
public class PromptMapper : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PromptMapper"/> class.
    /// </summary>
    public PromptMapper()
    {
        this.CreateMap<GetPromptsResponseItem, GetPromptsModel>();
        this.CreateMap<PromptItem, GetPromptsItemModel>();
        this.CreateMap<UpsertPromptModel, UpsertPromptItem>();
    }
}