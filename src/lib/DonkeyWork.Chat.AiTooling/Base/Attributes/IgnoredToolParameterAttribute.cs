// ------------------------------------------------------
// <copyright file="IgnoredToolParameterAttribute.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.AiTooling.Base.Attributes;

/// <summary>
/// An ignored tool parameter attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter, Inherited = false, AllowMultiple = false)]
public sealed class IgnoredToolParameterAttribute : Attribute
{
    // You can add properties or methods here if needed
}