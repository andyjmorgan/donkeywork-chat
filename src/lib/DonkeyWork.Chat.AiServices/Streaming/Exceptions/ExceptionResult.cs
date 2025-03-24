// ------------------------------------------------------
// <copyright file="ExceptionResult.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.AiServices.Streaming.Exceptions;

/// <summary>
/// An exception result.
/// </summary>
public record ExceptionResult : BaseStreamItem
{
    /// <summary>
    /// Gets the exception.
    /// </summary>
    required public Exception Exception { get; init; }
}