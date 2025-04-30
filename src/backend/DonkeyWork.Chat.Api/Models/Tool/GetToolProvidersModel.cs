// ------------------------------------------------------
// <copyright file="GetToolProvidersModel.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.Api.Models.Tool;

/// <summary>
/// A model for getting tool providers.
/// </summary>
public record GetToolProvidersModel
{
    /// <summary>
    /// Gets the tool providers.
    /// </summary>
    public List<ToolProvidersModel> ToolProviders { get; init; } = [];
}