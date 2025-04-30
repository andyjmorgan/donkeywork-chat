// ------------------------------------------------------
// <copyright file="GenericProviderPropertyType.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.Common.Models.Providers.Tools.GenericProvider;

/// <summary>
/// An enumeration representing the types of properties that can be used in a generic provider.
/// </summary>
public enum GenericProviderPropertyType
{
    /// <summary>
    /// A string value.
    /// </summary>
    String,

    /// <summary>
    /// A secret value.
    /// </summary>
    Secret,

    /// <summary>
    /// A boolean value.
    /// </summary>
    Boolean,

    /// <summary>
    /// An integer type.
    /// </summary>
    Integer,

    /// <summary>
    /// A double type.
    /// </summary>
    Double,
}