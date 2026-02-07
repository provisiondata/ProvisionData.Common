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

using ProvisionData;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// 
/// </summary>
public static class ProvisionDataConfigurationExtensions
{
    /// <summary>
    /// Adds services required for using the Result pattern with Provision Data's Error and ErrorCode
    /// types, including JSON converters for serializing and deserializing these types.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to which the provision data services are added.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance so that additional calls can be chained.</returns>
    [RequiresUnreferencedCode("The ErrorJsonConverter requires reflection to serialize/deserialize Error objects and their descendants, which may not be preserved in trimmed applications.")]
    public static IServiceCollection AddResultPattern(this IServiceCollection services)
    {
        // Register JsonTypeConverter for ErrorJsonConverter
        JsonSerializerOptions.Default.Converters.Add(new ErrorJsonConverter());
        JsonSerializerOptions.Default.Converters.Add(new ErrorCodeJsonConverter());

        return services;
    }
}
