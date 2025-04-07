// ------------------------------------------------------
// <copyright file="Tool.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using DonkeyWork.Chat.AiTooling.Attributes;
using DonkeyWork.Chat.AiTooling.Base.Models;
using DonkeyWork.Chat.AiTooling.Exceptions;
using DonkeyWork.Chat.Common.Providers;

namespace DonkeyWork.Chat.AiTooling.Base;

/// <summary>
/// A base tool implementation.
/// </summary>
public class Tool : ITool
{
    /// <inheritdoc />
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1101:Prefix local calls with this", Justification = "Record")]
    public List<ToolDefinition> GetToolDefinitions(List<UserProviderPosture> userProviderPosture)
    {
        List<ToolDefinition> toolDefinitions = new ();
        Type toolType = this.GetType();

        // Check if tool requires a specific provider
        var providerAttribute = toolType.GetCustomAttribute<ToolProviderAttribute>();
        if (providerAttribute is not null && !HasRequiredProvider(providerAttribute, userProviderPosture))
        {
            return [];
        }

        var methods = toolType.GetMethods().Where(x => x.GetCustomAttribute<ToolFunctionAttribute>() is not null);
        foreach (var method in methods)
        {
            // Check method-level scope requirements
            if (providerAttribute is not null && !HasRequiredScopes(method, providerAttribute, userProviderPosture))
            {
                continue;
            }

            ToolDefinition toolDefinition = new ToolDefinition
            {
                Name = method.Name,
                Description = method.GetCustomAttribute<DescriptionAttribute>()?.Description ?? string.Empty,
                Tool = this,
            };

            var parameters = method.GetParameters()
                .Where(x =>
                    x.GetCustomAttribute<ToolIgnoredParameterAttribute>() is null)
                .ToList();

            var functionParameters = new ToolFunctionDefinition
            {
                Type = "object",
                Properties = parameters
                    .Where(p => p.Name is not null)
                    .ToDictionary(
                        p => p.Name!,
                        p =>
                        {
                            var schema = GetJsonSchema(p.ParameterType);

                            // Add description if present
                            var descriptionAttr = p.GetCustomAttribute<DescriptionAttribute>();
                            if (descriptionAttr is not null)
                            {
                                schema = schema with
                                {
                                    Description = descriptionAttr.Description,
                                };
                            }

                            return schema;
                        }),

                Required = parameters
                    .Where(p => !p.IsOptional && p.Name is not null)
                    .Select(p => p.Name!)
                    .ToList(),
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

        try
        {
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
        }
        catch (Exception ex)
        {
            // Return this error to the llm and attempt to allow it to self correct.
            return JsonDocument.Parse(JsonSerializer.Serialize(new
            {
                FunctionName = functionName,
                Success = false,
                arguments,
                Exception = ex.Message,
            }));
        }

        try
        {
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
        catch (Exception ex)
        {
            return JsonDocument.Parse(JsonSerializer.Serialize(
                new
                {
                    methodName = functionName,
                    sucess = false,
                    exception = ex.Message,
                }));
        }
    }

    private static ToolFunctionParameterDefinition GetJsonSchema(Type type)
    {
        Type actualType = Nullable.GetUnderlyingType(type) ?? type;

        // Handle enums at the schema creation level
        if (actualType.IsEnum)
        {
            return new ToolFunctionParameterDefinition
            {
                Type = "string", // I know this looks wrong, it's not.
                Enum = Enum.GetNames(actualType).ToList(),
            };
        }

        // Don't treat string as IEnumerable<char>
        if (actualType != typeof(string) && IsEnumerableType(actualType, out var elementType))
        {
            return new ToolFunctionParameterDefinition
            {
                Type = "array",
                Items = elementType is not null ? GetJsonSchema(elementType) : null,
            };
        }

        string typeString = actualType switch
        {
            { } t when t == typeof(int) || t == typeof(long) || t == typeof(short) || t == typeof(byte) => "integer",
            { } t when t == typeof(float) || t == typeof(double) || t == typeof(decimal) => "number",
            { } t when t == typeof(bool) => "boolean",
            { } t when t == typeof(string) || t == typeof(DateTime) || t == typeof(DateTimeOffset) => "string",
            _ => "object"
        };

        return new ToolFunctionParameterDefinition
        {
            Type = typeString,
        };
    }

    private static bool IsEnumerableType(Type type, out Type? elementType)
    {
        elementType = null;

        if (type.IsArray)
        {
            elementType = type.GetElementType();
            return true;
        }

        if (type.IsGenericType &&
            type.GetInterfaces().Any(i =>
                i.IsGenericType &&
                i.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
        {
            elementType = type.GetGenericArguments()[0];
            return true;
        }

        return false;
    }

    private static object? DeserializeParameter(JsonElement element, Type targetType)
    {
        Type underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;
        if (underlyingType == typeof(string))
        {
            return element.GetString() ?? string.Empty;
        }

        if (underlyingType == typeof(int))
        {
            return element.GetInt32();
        }

        if (underlyingType == typeof(bool))
        {
            return element.GetBoolean();
        }

        if (underlyingType == typeof(double))
        {
            return element.GetDouble();
        }

        if (underlyingType.IsEnum)
        {
            return Enum.Parse(underlyingType, element.GetString() ?? string.Empty, true);
        }

        return JsonSerializer.Deserialize(
            element.GetRawText(),
            targetType,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    private bool HasRequiredProvider(ToolProviderAttribute providerAttribute, List<UserProviderPosture>? userProviderPostures)
    {
        var requiredProviderType = providerAttribute.ProviderType;
        var requiredPosture = userProviderPostures?.FirstOrDefault(p => p.ProviderType == requiredProviderType);
        return requiredPosture is not null;
    }

    private bool HasRequiredScopes(MethodInfo method, ToolProviderAttribute providerAttribute, List<UserProviderPosture>? userProviderPostures)
    {
        var scopeAttribute = method.GetCustomAttribute<ToolProviderScopesAttribute>();
        if (scopeAttribute is null)
        {
            return true; // No scope requirements
        }

        var requiredProviderType = providerAttribute.ProviderType;
        var requiredPosture = userProviderPostures?.FirstOrDefault(p => p.ProviderType == requiredProviderType);

        if (requiredPosture is null)
        {
            return false;
        }

        if (scopeAttribute.ScopeHandleType == UserProviderScopeHandleType.Any)
        {
            return scopeAttribute.Scopes.Any(scope => requiredPosture.Scopes.Contains(scope));
        }

        return true; // Default case, can be expanded for other ScopeHandleTypes
    }
}