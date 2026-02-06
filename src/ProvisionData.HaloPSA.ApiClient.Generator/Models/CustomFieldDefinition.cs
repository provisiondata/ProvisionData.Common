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
/// Defines a custom field configuration for code generation.
/// </summary>
public class CustomFieldDefinition
{
    /// <summary>
    /// Gets or sets the type of the custom field.
    /// Valid values: "string", "int32", "datetime", "boolean", "decimal"
    /// </summary>
    [JsonPropertyName("type")]
    public String Type { get; set; } = String.Empty;

    /// <summary>
    /// Gets or sets whether this field is required.
    /// Required fields will be validated during ApplyFieldMappings.
    /// </summary>
    [JsonPropertyName("required")]
    public Boolean Required { get; set; }

    /// <summary>
    /// Gets or sets the description for XML documentation.
    /// </summary>
    [JsonPropertyName("description")]
    public String? Description { get; set; }

    /// <summary>
    /// Validates that the field definition is valid.
    /// </summary>
    /// <param name="modelName">The model name for error messages.</param>
    /// <param name="fieldName">The field name for error messages.</param>
    /// <exception cref="InvalidOperationException">Thrown when the definition is invalid.</exception>
    public void Validate(String modelName, String fieldName)
    {
        if (String.IsNullOrWhiteSpace(Type))
        {
            throw new InvalidOperationException(
                $"Custom field definition missing required property 'type' for {modelName}.{fieldName}");
        }

        var validTypes = new[] { "string", "int32", "datetime", "boolean", "decimal" };
        if (!validTypes.Contains(Type.ToLowerInvariant()))
        {
            throw new InvalidOperationException(
                $"Invalid custom field type '{Type}' for {modelName}.{fieldName}. " +
                $"Valid types: {String.Join(", ", validTypes)}");
        }
    }

    /// <summary>
    /// Gets the C# type name for this field.
    /// </summary>
    /// <returns>The C# type as a string (e.g., "String?", "Int32?", "DateTime?").</returns>
    public String GetCSharpType()
    {
        return Type.ToLowerInvariant() switch
        {
            "string" => "String?",
            "int32" => "Int32?",
            "datetime" => "DateTime?",
            "boolean" => "Boolean?",
            "decimal" => "Decimal?",
            _ => throw new InvalidOperationException($"Unsupported type: {Type}")
        };
    }

    /// <summary>
    /// Gets the getter method name for this field type.
    /// </summary>
    /// <returns>The extension method name (e.g., "GetString", "GetInt32").</returns>
    public String GetGetterMethod()
    {
        return Type.ToLowerInvariant() switch
        {
            "string" => "GetString",
            "int32" => "GetInt32",
            "datetime" => "GetDateTime",
            "boolean" => "GetBoolean",
            "decimal" => "GetDecimal",
            _ => throw new InvalidOperationException($"Unsupported type: {Type}")
        };
    }

    /// <summary>
    /// Gets the setter method parameters for this field type.
    /// </summary>
    /// <param name="fieldIdVariable">The variable name holding the field ID.</param>
    /// <param name="valueParameter">The value parameter name.</param>
    /// <returns>The complete parameter string for the SetField call.</returns>
    public String GetSetterParameters(String fieldIdVariable, String valueParameter)
    {
        return Type.ToLowerInvariant() switch
        {
            "string" => $"{fieldIdVariable}, {valueParameter}",
            "int32" => $"{fieldIdVariable}, {valueParameter}?.ToString()",
            "datetime" => $"this, {fieldIdVariable}, {valueParameter}",
            "boolean" => $"{fieldIdVariable}, {valueParameter}?.ToString()",
            "decimal" => $"{fieldIdVariable}, {valueParameter}?.ToString(CultureInfo.InvariantCulture)",
            _ => throw new InvalidOperationException($"Unsupported type: {Type}")
        };
    }
}
