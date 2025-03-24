// ------------------------------------------------------
// <copyright file="Tool.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.ComponentModel;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using DonkeyWork.Chat.AiTooling.Base.Attributes;
using DonkeyWork.Chat.AiTooling.Base.Exceptions;
using DonkeyWork.Chat.AiTooling.Base.Models;

namespace DonkeyWork.Chat.AiTooling.Base;

/// <summary>
/// A base tool implementation.
/// </summary>
public class Tool : ITool
{
    /// <inheritdoc />
    public List<ToolDefinition> GetToolDefinitions()
    {
        List<ToolDefinition> toolDefinitions = new ();
        Type toolType = this.GetType();
        var methods = toolType.GetMethods().Where(x => x.GetCustomAttribute<ToolFunctionAttribute>() is not null);

        foreach (var method in methods)
        {
            ToolDefinition toolDefinition = new ToolDefinition
            {
                Name = method.Name,
                Description = method.GetCustomAttribute<DescriptionAttribute>()?.Description ?? string.Empty,
                Tool = this,
            };

            var parameters = method.GetParameters()
                .Where(x =>
                    x.GetCustomAttribute<IgnoredToolParameterAttribute>() is null
                && x.HasDefaultValue == false);

            var parameterDescriptions = parameters
                .Select(p => new
            {
                Name = p.Name ?? string.Empty,
                Type = p.ParameterType.Name,
                p.GetCustomAttribute<DescriptionAttribute>()?.Description,
            }).ToList();

            var functionParameters = new ToolFunctionDefinition()
            {
                Type = "object",
                Properties = parameterDescriptions.Select(x => new
                    {
                        x.Name,
                        ToolFunctionParameterDefinition = new ToolFunctionParameterDefinition
                        {
                            // todo: handle enum
                            Type = x.Type.ToLower(),
                            Description = x.Description ?? string.Empty,
                        },
                    })
                    .ToDictionary(x => x.Name, x => x.ToolFunctionParameterDefinition),

                Required = method.GetParameters().Where(p => !p.IsOptional && p.Name != null).Select(p => p.Name ?? string.Empty).ToList(),
            };

            var functionParametersJson = JsonSerializer.Serialize(
                functionParameters,
                new JsonSerializerOptions
                {
                    WriteIndented = true,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                });

            toolDefinition.Parameters = functionParametersJson;
            toolDefinition.ToolFunctionDefinition = functionParameters;
            toolDefinitions.Add(toolDefinition);
        }

        return toolDefinitions;
    }

    /// <inheritdoc />
    public virtual async Task<object?> InvokeFunctionAsync(string functionName, JsonDocument arguments, CancellationToken cancellationToken = default)
    {
        var method = this.GetType().GetMethods()
            .FirstOrDefault(x => x.GetCustomAttribute<ToolFunctionAttribute>() != null
                                 && x.Name == functionName);

        if (method == null)
        {
            throw new InvalidOperationException($"Function {functionName} not found.");
        }

        var parameters = method.GetParameters();
        var parameterValues = new object?[parameters.Length];

        for (int i = 0; i < parameters.Length; i++)
        {
            var param = parameters[i];
            if (param.Name is null)
            {
                throw new ToolParameterMissingException(param.Name ?? "Unknown Name");
            }

            if (param.ParameterType == typeof(CancellationToken))
            {
                parameterValues[i] = cancellationToken;
                continue;
            }

            if (arguments.RootElement.TryGetProperty(param.Name, out var element))
            {
                parameterValues[i] = DeserializeParameter(element, param.ParameterType);
            }
            else if (!param.IsOptional)
            {
                throw new ToolArgumentMissingException(param.Name ?? "Unknown Name");
            }
            else
            {
                parameterValues[i] = param.DefaultValue;
            }
        }

        var result = method.Invoke(this, parameterValues);

        if (result is Task task)
        {
            await task.WaitAsync(cancellationToken);
            if (method.ReturnType.IsGenericType)
            {
                return ((dynamic)task).Result;
            }

            return null;
        }

        return result;
    }

    private static object? DeserializeParameter(JsonElement element, Type targetType)
    {
        if (targetType == typeof(string))
        {
            return element.GetString() ?? string.Empty;
        }

        if (targetType == typeof(int))
        {
            return element.GetInt32();
        }

        if (targetType == typeof(bool))
        {
            return element.GetBoolean();
        }

        if (targetType == typeof(double))
        {
            return element.GetDouble();
        }

        if (targetType.IsEnum)
        {
            return Enum.Parse(targetType, element.GetString() ?? string.Empty, true);
        }

        return JsonSerializer.Deserialize(
            element.GetRawText(),
            targetType,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }
}