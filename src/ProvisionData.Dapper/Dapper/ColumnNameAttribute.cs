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

namespace ProvisionData.Dapper;

/// <summary>
/// Specifies the database column name that a property should be mapped to when using Dapper.
/// </summary>
/// <remarks>
/// Apply this attribute to properties in your model classes to define custom mappings between
/// database column names and property names. This is useful when database column names differ
/// from your C# property naming conventions.
/// </remarks>
/// <example>
/// <code>
/// public class Customer
/// {
///     [ColumnMap("customer_id")]
///     public Int32 Id { get; set; }
///
///     [ColumnMap("full_name")]
///     public String Name { get; set; }
/// }
/// </code>
/// </example>
/// <param name="columnName">The name of the database column to map to this property.</param>
[AttributeUsage(AttributeTargets.Property)]
public class ColumnNameAttribute(String columnName) : Attribute
{
    /// <summary>
    /// Gets the database column name that this property maps to.
    /// </summary>
    public String Name { get; } = columnName;
}
