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

using System.Diagnostics.CodeAnalysis;

namespace ProvisionData.Dapper;

/// <summary>
/// Defines a contract for types that require custom Dapper column mapping configuration.
/// NOTE: You should not implement this interface directly. Instead, inherit
/// the <see cref="INeedColumnMapping{TSelf}"/> interface for automated mapping.
/// </summary>
public interface INeedColumnMapping
{
    /// <summary>
    /// Applies the column mapping configuration for this type to Dapper's type map.
    /// NOTE: You should rarely need to call this method directly. Instead, inherit
    /// the <see cref="INeedColumnMapping{TSelf}"/> interface for automated mapping.
    /// </summary>
    void ApplyMap();
}

/// <summary>
/// Provides a self-referencing generic interface for types that require custom Dapper column mapping.
/// </summary>
/// <typeparam name="TSelf">The type that implements this interface, enabling automatic mapping configuration.</typeparam>
/// <remarks>
/// <para>
/// This interface provides a default implementation of <see cref="ApplyMap"/> that automatically
/// configures Dapper to use <see cref="ColumnMapAttribute"/> decorations on the implementing type's properties.
/// </para>
/// <para>
/// Use this interface when your entity class can map itself, rather than requiring a separate mapping class.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// public class Customer : INeedColumnMapping&lt;Customer&gt;
/// {
///     [ColumnMap("customer_id")]
///     public Int32 Id { get; set; }
///
///     [ColumnMap("full_name")]
///     public String Name { get; set; }
/// }
/// </code>
/// </example>
public interface INeedColumnMapping<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] TSelf>
    where TSelf : INeedColumnMapping<TSelf>
{
    /// <summary>
    /// Applies the column mapping configuration for <typeparamref name="TSelf"/> to Dapper's type map.
    /// </summary>
    /// <remarks>
    /// The default implementation calls <see cref="ColumnMapping.ApplyMap{T}"/> to configure
    /// Dapper to recognize <see cref="ColumnMapAttribute"/> decorations on the type's properties.
    /// </remarks>
    void ApplyMap() => ColumnMapping.ApplyMap<TSelf>();
}
