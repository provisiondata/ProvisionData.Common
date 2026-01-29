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
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace ProvisionData.Dapper;

public static class ColumnMapping
{
    public static void MapColumns(IServiceProvider services)
    {
        var columnMaps = services.GetServices<INeedColumnMapping>();
        foreach (var map in columnMaps)
        {
            map.ApplyMap();
        }
    }

    public static void MapColumns(this IApplicationBuilder app)
    {
        MapColumns(app.ApplicationServices);
    }

    public static void ApplyMap<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>()
    {
        SqlMapper.SetTypeMap(
            typeof(T),
            new CustomPropertyTypeMap(
                typeof(T),
                (_, columnName) => typeof(T).GetProperties()
                    .FirstOrDefault(prop =>
                        prop.GetCustomAttribute<ColumnMapAttribute>(false)?.Name == columnName)!
            )
        );
    }

    public static void ApplyMap([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] this Type type)
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
