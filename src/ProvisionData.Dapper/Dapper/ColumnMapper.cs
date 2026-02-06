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
using System.Reflection;

namespace ProvisionData.Dapper;

/// <summary>
/// Provides methods to configure Dapper column-to-property mappings using <see cref="ColumnNameAttribute"/> decorations.
/// </summary>
public static class ColumnMapper
{
    /// <summary>
    /// Searches for all types decorated with <see cref="HasColumnMapsAttribute"/> in the calling assembly
    /// and maps the properties that are decorated with <see cref="ColumnNameAttribute"/>.
    /// </summary>
    /// <remarks>The search does not include inherited attributes; only attributes applied directly to the
    /// discovered types are matched. If an assembly cannot load all of its types, only the successfully
    /// loaded types are mapped.</remarks>
    public static void MapTypesFromCallingAssembly()
    {
        MapTypesFromAssemblies(Assembly.GetCallingAssembly());
    }

    /// <summary>
    /// Searches for all types decorated with <see cref="HasColumnMapsAttribute"/> in the executing assembly
    /// and maps the properties that are decorated with <see cref="ColumnNameAttribute"/>.
    /// </summary>
    public static void MapTypesFromExecutingAssembly()
    {
        MapTypesFromAssemblies(Assembly.GetExecutingAssembly());
    }

    /// <summary>
    /// Searches for all types decorated with <see cref="HasColumnMapsAttribute"/> in the assembly that contains the specified <typeparamref name="T"/>
    /// and maps the properties that are decorated with <see cref="ColumnNameAttribute"/>.
    /// </summary>
    /// <remarks>The search does not include inherited attributes; only attributes applied directly to the
    /// discovered types are matched. If an assembly cannot load all of its types, only the successfully
    /// loaded types are mapped.</remarks>
    /// <typeparam name="T">The type whose containing assembly will be scanned for types to map.</typeparam>
    public static void MapTypesFromAssemblyContaining<T>()
    {
        MapTypesFromAssemblies(typeof(T).Assembly);
    }

    /// <summary>
    /// Searches for all types decorated with <see cref="HasColumnMapsAttribute"/> in the provided <paramref name="assemblies"/>
    /// and maps the properties that are decorated with <see cref="ColumnNameAttribute"/>.
    /// </summary>
    /// <remarks>The search does not include inherited attributes; only attributes applied directly to the
    /// discovered types are matched. If an assembly cannot load all of its types, only the successfully
    /// loaded types are mapped.</remarks>
    /// <param name="assemblies">The assemblies to search for types with the specified attribute. At least one assembly must be provided.</param>
    public static void MapTypesFromAssemblies(params Assembly[] assemblies)
    {
        ArgumentNullException.ThrowIfNull(assemblies);

        var list = assemblies.SelectMany(a =>
        {
            try
            {
                return a.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                return ex.Types.Where(static t => t != null);
            }
        }).Where(t => t?.IsDefined(typeof(HasColumnMapsAttribute), inherit: false) == true)
          .ToArray();

        foreach (var type in list)
        {
            if (type == null)
                continue;

            MapColumns(type);
        }
    }

    /// <summary>
    /// Maps the properties of <typeparamref name="T"/> that are decorated with <see cref="ColumnNameAttribute"/>.
    /// </summary>
    /// <typeparam name="T">The type to configure column mappings for.</typeparam>    
    public static void Map<T>()
    {
        Type type = typeof(T);

        CustomPropertyTypeMap map = new(type, (_, columnName) => type.GetProperties().FirstOrDefault(prop => prop.GetCustomAttribute<ColumnNameAttribute>(false)?.Name == columnName)!);

        SqlMapper.SetTypeMap(type, map);
    }

    /// <summary>
    /// Maps the properties <paramref name="type"/> that are decorated with <see cref="ColumnNameAttribute"/>.
    /// </summary>
    /// <param name="type">The type to configure column mappings for.</param>
    public static void MapColumns(this Type type)
    {
        SqlMapper.SetTypeMap(
            type,
            new CustomPropertyTypeMap(
                type,
                (_, columnName) => type.GetProperties()
                    .FirstOrDefault(prop =>
                        prop.GetCustomAttribute<ColumnNameAttribute>(false)?.Name == columnName)!
            )
        );
    }
}
