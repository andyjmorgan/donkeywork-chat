// ------------------------------------------------------
// <copyright file="GetActionsModel.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.Api.Models.Action;

/// <summary>
/// Model for getting a list of actions.
/// </summary>
public class GetActionsModel
{
    /// <summary>
    /// Gets or sets the total count.
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Gets or sets the actions.
    /// </summary>
    public List<GetActionsItemModel> Actions { get; set; } = [];
}