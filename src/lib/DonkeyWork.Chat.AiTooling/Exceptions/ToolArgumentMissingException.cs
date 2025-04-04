// ------------------------------------------------------
// <copyright file="ToolArgumentMissingException.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.AiTooling.Exceptions;

/// <summary>
/// An exception thrown when a tool argument is missing.
/// </summary>
public class ToolArgumentMissingException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ToolArgumentMissingException"/> class.
    /// </summary>
    /// <param name="argumentName">The argument name.</param>
    public ToolArgumentMissingException(string argumentName)
        : base($"Tool argument missing: {argumentName}")
    {
    }
}