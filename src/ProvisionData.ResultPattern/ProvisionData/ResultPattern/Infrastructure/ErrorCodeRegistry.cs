// Provision Data Application Framework
// Copyright (C) 2026 Provision Data Systems Inc.
//
// This program is free software: you can redistribute it and/or modify it under the terms of
// the GNU Affero General Public License as published by the Free Software Foundation, either
// version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with this
// program. If not, see <https://www.gnu.org/licenses/>.

using ProvisionData.ResultPattern.Generators;
using System.Collections.Concurrent;
using System.Reflection;

namespace ProvisionData.ResultPattern.Infrastructure;

/// <summary>
/// Serves as a registry for all <see cref="Error"/> types and their corresponding error code types in the
/// system. This allows for dynamic discovery and handling of errors based on their types
/// and codes.
/// </summary>
/// <remarks>
/// Error types are automatically registered via source-generated ModuleInitializers.
/// Each assembly containing partial Error classes will have its errors registered
/// automatically when the assembly is loaded.
/// </remarks>
internal static class ErrorCodeRegistry
{
    internal static readonly ConcurrentDictionary<Type, ErrorCode> LookupTable = new();

    public static ErrorCode GetFor<TError>()
        where TError : Error => GetFor(typeof(TError));

    public static ErrorCode GetFor(Type type)
        => LookupTable.GetOrAdd(type, CreateInstance);

    private static ErrorCode CreateInstance(Type errorType)
    {
        // 1. Find the Error.Code property
        var codeProp = errorType.GetProperty(GeneratorConsts.ErrorCodeProperty, BindingFlags.Public | BindingFlags.Instance)
            ?? throw new InvalidOperationException($"Type '{errorType.FullName}' does not contain a '{GeneratorConsts.ErrorCodeProperty}' property.");

        var codeType = codeProp.PropertyType;

        // 2. Find the static Instance property on the ErrorCode type
        var instanceProp = codeType.GetProperty(GeneratorConsts.ErrorCodeInstance, BindingFlags.Public | BindingFlags.Static);
        if (instanceProp is not null && typeof(ErrorCode).IsAssignableFrom(instanceProp.PropertyType))
        {
            // Use the singleton instance
            return (ErrorCode)instanceProp.GetValue(null)!;
        }

        // 3. Fallback: create a new instance
        return (ErrorCode)Activator.CreateInstance(codeType)!;
    }
}
