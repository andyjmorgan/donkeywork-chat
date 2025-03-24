// ------------------------------------------------------
// <copyright file="ToolParameterMissingException.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.AiTooling.Base.Exceptions;

/// <summary>
/// An exception thrown when a tool parameter is missing.
/// </summary>
public class ToolParameterMissingException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ToolParameterMissingException"/> class.
    /// </summary>
    /// <param name="argumentName">The argument name.</param>
    public ToolParameterMissingException(string argumentName)
        : base($"Tool parameter missing: {argumentName}")
    {
    }
}