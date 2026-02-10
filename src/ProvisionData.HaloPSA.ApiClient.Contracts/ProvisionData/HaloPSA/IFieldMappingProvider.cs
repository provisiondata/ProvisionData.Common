// Provision Data HaloPSA API Client
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

namespace ProvisionData.HaloPSA;

/// <summary>
/// Provides field ID mappings for HaloPSA custom fields.
/// </summary>
public interface IFieldMappingProvider
{
    /// <summary>
    /// Gets the field ID for a specific model type and field name.
    /// </summary>
    /// <param name="modelType">The type of the model (e.g., "Asset").</param>
    /// <param name="fieldName">The field name (e.g., "SerialNumber").</param>
    /// <returns>The field ID, or null if not configured.</returns>
    Int32 GetFieldId(String modelType, String fieldName);
}
