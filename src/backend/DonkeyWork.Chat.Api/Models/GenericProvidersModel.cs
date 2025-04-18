// ------------------------------------------------------
// <copyright file="GenericProvidersModel.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Providers.GenericProvider;

namespace DonkeyWork.Chat.Api.Models;

/// <summary>
/// A Model for generic providers.
/// </summary>
public class GenericProvidersModel
{
    /// <summary>
    /// A list of known providers.
    /// </summary>
    public List<GenericProviderDefinition> Providers { get; set; } = [];
}