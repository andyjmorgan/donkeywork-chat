// ------------------------------------------------------
// <copyright file="ToolFunctionCallHandlerAttribute.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.AiTooling.Base.Attributes;

/// <summary>
/// A tool function attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public sealed class ToolFunctionCallHandlerAttribute : Attribute
{
}