// ------------------------------------------------------
// <copyright file="PagingParameters.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Persistence.Common.Common;

/// <summary>
/// Parameters for paging.
/// </summary>
public class PagingParameters
{
    /// <summary>
    /// Gets or sets the offset.
    /// </summary>
    public int Offset { get; set; } = 0;

    /// <summary>
    /// Gets or sets the limit.
    /// </summary>
    public int Limit { get; set; } = 10;
}