// ProvisionData.Common
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

using Dapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ProvisionData.Dapper;

/// <summary>
/// Provides methods to configure Dapper column-to-property mappings using <see cref="ColumnMapAttribute"/> decorations.
/// </summary>
/// <remarks>
/// This class enables mapping database column names to C# property names when they differ from each other.
/// Use <see cref="MapColumns(IServiceProvider)"/> or <see cref="MapColumns(IApplicationBuilder)"/> during
/// application startup to apply all registered column mappings.
/// </remarks>
public static class ColumnMapping
{
    /// <summary>
    /// Applies all registered column mappings from the dependency injection container.
    /// </summary>
    /// <param name="services">The service provider containing registered <see cref="INeedColumnMapping"/> implementations.</param>
    /// <remarks>
    /// This method resolves all <see cref="INeedColumnMapping"/> services from the container
    /// and calls <see cref="INeedColumnMapping.ApplyMap"/> on each one to configure Dapper's type mappings.
    /// </remarks>
    public static void MapColumns(this IServiceProvider services)
    {
        var columnMaps = services.GetServices<INeedColumnMapping>();
        foreach (var map in columnMaps)
        {
            map.ApplyMap();
        }
    }

    /// <summary>
    /// Applies all registered column mappings using the application's service provider.
    /// </summary>
    /// <param name="app">The application builder whose services contain registered <see cref="INeedColumnMapping"/> implementations.</param>
    /// <remarks>
    /// This extension method is typically called during application startup, after <code>builder.Build()</code> but before <code>app.Run()</code> or <code>await app.RunAsync()</code>
    /// </remarks>
    /// <example>
    /// <code>
    ///     builder.Services.AddColumnMapping();
    /// 
    ///     // ... other service registrations
    ///     
    ///     var app = builder.Build();
    /// 
    ///     app.MapColumns();
    ///     
    ///     // ... other middleware configuration
    ///     
    ///     app.Run();
    ///     // Or await app.RunAsync()
    /// </code>
    /// </example>
    public static void MapColumns(this IApplicationBuilder app)
    {
        MapColumns(app.ApplicationServices);
    }

    /// <summary>
    /// Configures Dapper to use <see cref="ColumnMapAttribute"/> decorations for the specified type.
    /// </summary>
    /// <typeparam name="T">The type to configure column mappings for.</typeparam>
    /// <remarks>
    /// After calling this method, Dapper will use <see cref="ColumnMapAttribute.Name"/> values
    /// to match database columns to properties on type <typeparamref name="T"/>.
    /// </remarks>
    public static void ApplyMap<T>()
    {
        Type type = typeof(T);

        CustomPropertyTypeMap map = new(type, (_, columnName) => type.GetProperties().FirstOrDefault(prop => prop.GetCustomAttribute<ColumnMapAttribute>(false)?.Name == columnName)!);

        SqlMapper.SetTypeMap(type, map);
    }

    /// <summary>
    /// Configures Dapper to use <see cref="ColumnMapAttribute"/> decorations for the specified type.
    /// </summary>
    /// <param name="type">The type to configure column mappings for.</param>
    /// <remarks>
    /// After calling this method, Dapper will use <see cref="ColumnMapAttribute.Name"/> values
    /// to match database columns to properties on the specified type.
    /// </remarks>
    public static void ApplyMap(this Type type)
    {
        SqlMapper.SetTypeMap(
            type,
            new CustomPropertyTypeMap(
                type,
                (_, columnName) => type.GetProperties()
                    .FirstOrDefault(prop =>
                        prop.GetCustomAttribute<ColumnMapAttribute>(false)?.Name == columnName)!
            )
        );
    }
}
