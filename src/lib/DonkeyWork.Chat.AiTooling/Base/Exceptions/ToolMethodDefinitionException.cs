// ------------------------------------------------------
// <copyright file="ToolMethodDefinitionException.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.AiTooling.Base.Exceptions;

/// <summary>
/// An exception thrown when a tool method definition is unknown.
/// </summary>
public class ToolMethodDefinitionException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ToolMethodDefinitionException"/> class.
    /// </summary>
    /// <param name="argumentName">The argument name.</param>
    public ToolMethodDefinitionException(string argumentName)
        : base($"Tool method unknown or missing: {argumentName}")
    {
    }
}