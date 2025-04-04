// ------------------------------------------------------
// <copyright file="UnknownToolDefinitionException.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.AiTooling.Exceptions;

/// <summary>
/// An exception thrown when a tool definition is unknown.
/// </summary>
public class UnknownToolDefinitionException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnknownToolDefinitionException"/> class.
    /// </summary>
    /// <param name="argumentName">The argument name.</param>
    public UnknownToolDefinitionException(string argumentName)
        : base($"Tool unknown or missing: {argumentName}")
    {
    }
}