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
/// Specifies that a class defines column mappings for use with Dapper.
/// </summary>
/// <remarks>Apply this attribute to a class to indicate that it provides explicit mapping between its members and
/// columns in a database. The attribute does not enforce any behavior by itself; it serves as a marker that when
/// discovered at runtime will cause the mapping configuration to be applied.</remarks>
[AttributeUsage(AttributeTargets.Class)]
public class HasColumnMapsAttribute : Attribute
{
}
