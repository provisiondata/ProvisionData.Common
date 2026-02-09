// Provision Data Libraries
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

namespace ProvisionData.ResultPattern.Infrastructure;

/// <summary>
/// Serves as a registry for all error types and their corresponding error code types in the
/// system. This allows for dynamic discovery and handling of errors based on their types
/// and codes.
/// </summary>
/// <remarks>
/// Error types are automatically registered via source-generated ModuleInitializers.
/// Each assembly containing partial Error classes will have its errors registered
/// automatically when the assembly is loaded.
/// </remarks>
public static class ErrorTypeRegistry
{
    private static readonly List<Type> ErrorTypesList = [];
    private static readonly List<Type> ErrorCodeTypesList = [];

    /// <summary>
    /// Registers an error type and its corresponding error code type in the registry. This method
    /// is called automatically by source-generated ModuleInitializers to ensure all error types are registered.
    /// </summary>
    /// <typeparam name="TError">The error type to register.</typeparam>
    public static void Register<TError>() where TError : Error
    {
        var errorType = typeof(TError);
        var codeType = errorType.GetProperty("Code")!.PropertyType;

        ErrorTypesList.Add(errorType);
        ErrorCodeTypesList.Add(codeType);
    }

    /// <summary>
    /// Gets the collection of Error types registered in the system. This can be used for
    /// dynamic discovery and handling of strongly typed errors. The order of types in the
    /// collection is not guaranteed.
    /// </summary>
    public static IEnumerable<Type> ErrorTypes => ErrorTypesList;

    /// <summary>
    /// Gets the collection of types that represent error codes supported by the application.
    /// </summary>
    /// <remarks>
    /// The returned collection includes all error code types that can be used with <see cref="Error"/>.
    /// The order of types in the collection is not guaranteed.
    /// </remarks>
    public static IEnumerable<Type> ErrorCodeTypes => ErrorCodeTypesList;
}
