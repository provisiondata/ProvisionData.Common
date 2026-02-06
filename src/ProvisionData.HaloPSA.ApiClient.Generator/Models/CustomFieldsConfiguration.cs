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

using System.Text.Json.Serialization;

namespace ProvisionData.HaloPSA.ModelGenerator.Models;

/// <summary>
/// Root configuration for custom fields definitions.
/// </summary>
public class CustomFieldsConfiguration
{
    /// <summary>
    /// Gets or sets the custom field mappings for each model type.
    /// Key is the model name (e.g., "Asset"), value is a dictionary of field definitions.
    /// </summary>
    [JsonPropertyName("CustomFieldMappings")]
    public Dictionary<String, Dictionary<String, CustomFieldDefinition>> CustomFieldMappings { get; set; } = [];

    /// <summary>
    /// Checks if a specific model has custom field definitions.
    /// </summary>
    /// <param name="modelName">The name of the model.</param>
    /// <returns>True if the model has custom field definitions; otherwise, false.</returns>
    public Boolean HasCustomFields(String modelName)
    {
        return CustomFieldMappings.ContainsKey(modelName) && CustomFieldMappings[modelName].Count > 0;
    }

    /// <summary>
    /// Gets the custom field definitions for a specific model.
    /// </summary>
    /// <param name="modelName">The name of the model.</param>
    /// <returns>Dictionary of field definitions, or empty dictionary if not found.</returns>
    public Dictionary<String, CustomFieldDefinition> GetFieldsForModel(String modelName)
    {
        return CustomFieldMappings.TryGetValue(modelName, out var fields)
            ? fields
            : [];
    }
}
