// ------------------------------------------------------
// <copyright file="KnownMetaDataFields.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.AiServices.Clients;

/// <summary>
/// An enumeration of known metadata fields.
/// </summary>
public enum KnownMetaDataFields
{
    /// <summary>
    /// A temperature setting for the model.
    /// </summary>
    Temperature,

    /// <summary>
    /// A top-p setting for the model.
    /// </summary>
    TopP,

    /// <summary>
    /// A top-k setting for the model.
    /// </summary>
    TopK,

    /// <summary>
    /// A setting for the model.
    /// </summary>
    MaxTokens,

    /// <summary>
    /// A setting for the model to enable thinking.
    /// </summary>
    ThinkingEnabled,

    /// <summary>
    /// A setting for the model to budget thinking tokens.
    /// </summary>
    BudgetThinkingTokens,
}