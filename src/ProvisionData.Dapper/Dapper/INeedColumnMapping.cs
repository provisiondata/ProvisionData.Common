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
/// Defines a contract for types that require custom Dapper column mapping configuration.
/// </summary>
public interface INeedColumnMapping
{
    /// <summary>
    /// Applies the column mapping configuration for the implementing type to Dapper's type map.
    /// </summary>
    /// <remarks>
    /// The default implementation calls <see cref="ColumnMapping.ApplyMap"/> to configure
    /// Dapper to recognize <see cref="ColumnMapAttribute"/> decorations on the type's properties.
    /// </remarks>
    void ApplyMap() => ColumnMapping.ApplyMap(GetType());
}
