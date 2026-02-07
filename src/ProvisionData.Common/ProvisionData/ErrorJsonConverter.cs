// Provision Data Libraries
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
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ProvisionData;

/// <summary>
/// JSON converter for Error that supports polymorphic serialization/deserialization
/// without requiring derived types to be registered via attributes.
/// </summary>
/// <remarks>
/// This converter allows external assemblies to create custom Error types that will
/// automatically serialize and deserialize correctly without modifying the base Error class.
/// The converter handles additional properties on derived Error types by using reflection
/// to read all public properties and invoke the appropriate constructor.
/// </remarks>
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
internal sealed class ErrorJsonConverter : JsonConverter<Error>
{
    [SuppressMessage("Trimming", "IL2057:Unrecognized value passed to the parameter of method. It's not possible to guarantee the availability of the target type.", Justification = "Type name is provided from JSON payload ($type property). The trimmer cannot statically analyze the type name at compile time, but callers must ensure they only deserialize types that have been preserved via IlDescriptors.xml or other trimming configuration.")]
    [SuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Method performs intentional polymorphic deserialization using reflection to reconstruct Error types from JSON. All Error types and their constructors must be preserved via IlDescriptors.xml for this to work correctly in trimmed environments.")]
    public override Error Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        // Get the type discriminator
        if (!root.TryGetProperty("$type", out var typeElement))
        {
            throw new JsonException("Error serialization requires $type property");
        }

        var typeName = typeElement.GetString();
        if (String.IsNullOrEmpty(typeName))
        {
            throw new JsonException("Error $type cannot be null or empty");
        }

        // Load the type
        var type = Type.GetType(typeName);
        if (type is null || !typeof(Error).IsAssignableFrom(type))
        {
            throw new JsonException($"Unknown or invalid Error type: '{typeName}'");
        }

        // Get all constructors and find the one with the most parameters (primary constructor for records)
        var constructors = type.GetConstructors()
            .OrderByDescending(c => c.GetParameters().Length)
            .ToArray();

        if (constructors.Length == 0)
        {
            throw new JsonException($"Error type '{type.FullName}' has no public constructors");
        }

        // Try each constructor until one works
        foreach (var constructor in constructors)
        {
            var parameters = constructor.GetParameters();
            var args = new Object?[parameters.Length];
            var success = true;

            for (var i = 0; i < parameters.Length; i++)
            {
                var param = parameters[i];
                var paramName = param.Name!;

                // Look for a JSON property that matches the parameter name (case-insensitive)
                var jsonProperty = root.EnumerateObject()
                    .FirstOrDefault(p => String.Equals(p.Name, paramName, StringComparison.OrdinalIgnoreCase));

                if (jsonProperty.Value.ValueKind == JsonValueKind.Undefined)
                {
                    // Parameter not found in JSON
                    if (param.HasDefaultValue)
                    {
                        args[i] = param.DefaultValue;
                    }
                    else
                    {
                        success = false;
                        break;
                    }
                }
                else
                {
                    // Deserialize the parameter value
                    try
                    {
                        args[i] = JsonSerializer.Deserialize(jsonProperty.Value.GetRawText(), param.ParameterType, options);
                    }
                    catch
                    {
                        success = false;
                        break;
                    }
                }
            }

            if (success)
            {
                try
                {
                    return (Error)constructor.Invoke(args);
                }
                catch
                {
                    // Constructor invocation failed, try next one
                }
            }
        }

        throw new JsonException($"Could not find a compatible constructor for Error type '{type.FullName}'");
    }

    [SuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Method performs intentional polymorphic serialization using reflection to discover all public properties on Error types. The [DynamicallyAccessedMembers(All)] attribute on Error class ensures all properties are preserved in trimmed environments.")]
    public override void Write(Utf8JsonWriter writer, Error value, JsonSerializerOptions options)
    {
        var type = value.GetType();

        writer.WriteStartObject();

        // Write type discriminator
        writer.WriteString("$type", type.AssemblyQualifiedName);

        // Write all public properties
        // We have to use reflection here in case we are serializing a derived type with additional properties.
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var property in properties)
        {
            // Skip properties that don't have a getter or are marked with JsonIgnore
            if (!property.CanRead || property.GetCustomAttribute<JsonIgnoreAttribute>() is not null)
            {
                continue;
            }

            var propertyValue = property.GetValue(value);

            writer.WritePropertyName(property.Name);
            JsonSerializer.Serialize(writer, propertyValue, property.PropertyType, options);
        }

        writer.WriteEndObject();
    }
}
