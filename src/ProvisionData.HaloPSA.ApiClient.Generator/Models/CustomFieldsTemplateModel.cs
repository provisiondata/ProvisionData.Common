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

namespace ProvisionData.HaloPSA.ModelGenerator.Models;

/// <summary>
/// Template data model for generating custom fields code.
/// </summary>
public class CustomFieldsTemplateModel
{
    /// <summary>
    /// Gets or sets the name of the model/DTO (e.g., "Asset").
    /// </summary>
    public String ModelName { get; set; } = String.Empty;

    /// <summary>
    /// Gets or sets the list of custom field definitions.
    /// </summary>
    public List<CustomFieldModel> Fields { get; set; } = [];

    /// <summary>
    /// Gets the list of required field names.
    /// </summary>
    public List<String> RequiredFieldNames => Fields
        .Where(f => f.IsRequired)
        .Select(f => f.Name)
        .ToList();

    /// <summary>
    /// Gets whether this model has any required fields.
    /// </summary>
    public Boolean HasRequiredFields => RequiredFieldNames.Count > 0;

    /// <summary>
    /// Gets the required fields validation condition for code generation.
    /// </summary>
    /// <returns>A string like "NameFieldId == 0 || SerialNumberFieldId == 0"</returns>
    public String GetRequiredFieldsCondition()
    {
        var conditions = RequiredFieldNames.Select(name => $"{name}FieldId == 0");
        return String.Join(" || ", conditions);
    }

    /// <summary>
    /// Gets a comma-separated list of required field names for error messages.
    /// </summary>
    public String GetRequiredFieldsList()
    {
        return String.Join(", ", RequiredFieldNames);
    }
}

/// <summary>
/// Represents a single custom field for code generation.
/// </summary>
public class CustomFieldModel
{
    /// <summary>
    /// Gets or sets the field name (property name).
    /// </summary>
    public String Name { get; set; } = String.Empty;

    /// <summary>
    /// Gets or sets the C# type (e.g., "String?", "Int32?").
    /// </summary>
    public String CSharpType { get; set; } = String.Empty;

    /// <summary>
    /// Gets or sets the getter method name (e.g., "GetString", "GetDateTime").
    /// </summary>
    public String GetterMethod { get; set; } = String.Empty;

    /// <summary>
    /// Gets or sets the setter method parameters string.
    /// </summary>
    public String SetterParameters { get; set; } = String.Empty;

    /// <summary>
    /// Gets or sets the XML documentation description.
    /// </summary>
    public String? Description { get; set; }

    /// <summary>
    /// Gets or sets whether this field is required.
    /// </summary>
    public Boolean IsRequired { get; set; }

    /// <summary>
    /// Gets the field ID variable name (e.g., "NameFieldId").
    /// </summary>
    public String FieldIdVariable => $"{Name}FieldId";

    /// <summary>
    /// Creates a CustomFieldModel from a CustomFieldDefinition.
    /// </summary>
    /// <param name="fieldName">The name of the field.</param>
    /// <param name="definition">The field definition.</param>
    /// <returns>A new CustomFieldModel instance.</returns>
    public static CustomFieldModel FromDefinition(String fieldName, CustomFieldDefinition definition)
    {
        return new CustomFieldModel
        {
            Name = fieldName,
            CSharpType = definition.GetCSharpType(),
            GetterMethod = definition.GetGetterMethod(),
            SetterParameters = definition.GetSetterParameters($"{fieldName}FieldId", "value"),
            Description = definition.Description,
            IsRequired = definition.Required
        };
    }
}
