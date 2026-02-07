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
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ProvisionData;

/// <summary>
/// JSON converter for ErrorCode that serializes by type and name, allowing deserialization of any ErrorCode subclass.
/// </summary>
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
internal sealed class ErrorCodeJsonConverter : JsonConverter<ErrorCode>
{
    [SuppressMessage("Trimming", "IL2057:Unrecognized value passed to the parameter of method. It's not possible to guarantee the availability of the target type.", Justification = "Type name is provided from JSON payload ($type property). The trimmer cannot statically analyze the type name at compile time, but callers must ensure they only deserialize types that have been preserved via IlDescriptors.xml or other trimming configuration.")]
    public override ErrorCode Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException("ErrorCode must be serialized as an object");
        }

        String? assemblyQualifiedTypeName = null;
        String? name = null;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                break;
            }

            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                var propertyName = reader.GetString();
                reader.Read();

                if (propertyName == "$type")
                {
                    assemblyQualifiedTypeName = reader.GetString();
                }
                else if (propertyName == "$name")
                {
                    name = reader.GetString();
                }
            }
        }

        if (String.IsNullOrEmpty(assemblyQualifiedTypeName))
        {
            throw new JsonException("ErrorCode requires $type property");
        }

        // Load the type and get its singleton Instance property or field
        var type = Type.GetType(assemblyQualifiedTypeName);
        if (type is null || !typeof(ErrorCode).IsAssignableFrom(type))
        {
            throw new JsonException($"Unknown or invalid ErrorCode type: '{assemblyQualifiedTypeName}'");
        }

        // Try field first (public static readonly), then property
        var instanceField = type.GetField("Instance",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

        if (instanceField is not null)
        {
            if (instanceField.GetValue(null) is not ErrorCode instance)
            {
                throw new JsonException($"Failed to retrieve singleton instance from '{type.FullName}.Instance' field");
            }

            return instance;
        }

        var instanceProperty = type.GetProperty("Instance",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

        if (instanceProperty is not null)
        {
            if (instanceProperty.GetValue(null) is not ErrorCode instance)
            {
                throw new JsonException($"Failed to retrieve singleton instance from '{type.FullName}.Instance' property");
            }

            return instance;
        }

        throw new JsonException($"ErrorCode type '{type.FullName}' does not have a static Instance field or property");
    }

    public override void Write(Utf8JsonWriter writer, ErrorCode value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString("$type", value.GetType().AssemblyQualifiedName);
        writer.WriteString("$name", value.ToString());
        writer.WriteEndObject();
    }
}
