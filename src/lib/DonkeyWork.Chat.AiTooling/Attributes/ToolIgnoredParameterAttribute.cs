// ------------------------------------------------------
// <copyright file="ToolIgnoredParameterAttribute.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.AiTooling.Attributes;

/// <summary>
/// An ignored tool parameter attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter, Inherited = false, AllowMultiple = false)]
public sealed class ToolIgnoredParameterAttribute : Attribute
{
}