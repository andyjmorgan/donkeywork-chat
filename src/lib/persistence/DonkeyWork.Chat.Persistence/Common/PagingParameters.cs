// ------------------------------------------------------
// <copyright file="PagingParameters.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.Persistence.Common;

/// <summary>
/// A class representing paging parameters.
/// </summary>
public record PagingParameters
{
    /// <summary>
    /// Gets the offset of the first record to return.
    /// </summary>
    public int Offset { get; init; } = 0;

    /// <summary>
    /// Gets the maximum number of records to return.
    /// </summary>
    public int Limit { get; init; } = 1000;
}