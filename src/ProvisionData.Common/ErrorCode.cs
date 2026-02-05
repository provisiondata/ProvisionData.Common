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
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ProvisionData;

/// <summary>
/// Represents an error code value used to identify specific error conditions.
/// </summary>
/// <remarks>
/// <para>
/// ErrorCode uses value equality semantics based on type and name. Each error code type
/// should define a single static Instance property that serves as the canonical reference.
/// </para>
/// <para>
/// Users can extend ErrorCode to create custom error types by creating a sealed class
/// that inherits from ErrorCode and defines a public static Instance property.
/// </para>
/// </remarks>
[JsonConverter(typeof(ErrorCodeJsonConverter))]
[RequiresUnreferencedCode("ErrorCode serialization/deserialization uses reflection to discover types and singleton Instance fields/properties, and may not work with trimming.")]
[RequiresDynamicCode("ErrorCode serialization/deserialization uses Type.GetType() and reflection which requires dynamic code generation.")]
public abstract class ErrorCode : IEquatable<ErrorCode>
{
    /// <summary>
    /// Initializes a new instance of the ErrorCode class.
    /// </summary>
    protected ErrorCode()
    {
    }

    /// <summary>
    /// A human-readable name for the error code.
    /// </summary>
    protected abstract String Name { get; }

    /// <summary>
    /// Returns the name of the error code.
    /// </summary>
    /// <returns>The name of the error code.</returns>
    public override String ToString() => Name;

    /// <summary>
    /// Implicitly converts an <see cref="ErrorCode"/> to a <see cref="String"/> by returning its name.
    /// </summary>
    /// <param name="errorCode">The error code to convert.</param>
    /// <returns>The name of the error code.</returns>
    public static implicit operator String(ErrorCode errorCode) => errorCode.Name;

    /// <summary>
    /// Determines whether this error code is equal to another based on type and name.
    /// </summary>
    /// <param name="other">The error code to compare to.</param>
    /// <returns>True if both have the same type and name; otherwise, false.</returns>
    public virtual Boolean Equals(ErrorCode? other)
        => other is not null && GetType() == other.GetType() && Name == other.Name;

    /// <summary>
    /// Determines whether this error code is equal to another object based on type and name.
    /// </summary>
    /// <param name="obj">The object to compare to.</param>
    /// <returns>True if both have the same type and name; otherwise, false.</returns>
    public override Boolean Equals(Object? obj) => Equals(obj as ErrorCode);

    /// <summary>
    /// Returns the hash code for this error code based on its type and name.
    /// </summary>
    /// <returns>The hash code.</returns>
    public override Int32 GetHashCode() => HashCode.Combine(GetType(), Name);
}

/// <summary>
/// JSON converter for ErrorCode that serializes by type and name, allowing deserialization of any ErrorCode subclass.
/// </summary>
internal sealed class ErrorCodeJsonConverter : JsonConverter<ErrorCode>
{
    [RequiresUnreferencedCode("ErrorCode deserialization uses reflection to find singleton Instance fields/properties and may not work with trimming.")]
    [RequiresDynamicCode("ErrorCode deserialization uses Type.GetType() and reflection which requires dynamic code generation.")]
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Method is already annotated with RequiresUnreferencedCode")]
    [UnconditionalSuppressMessage("Trimming", "IL2046", Justification = "Base JsonConverter<T>.Read is not annotated, but this override requires reflection")]
    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "Method is already annotated with RequiresDynamicCode")]
    [UnconditionalSuppressMessage("AOT", "IL3051", Justification = "Base JsonConverter<T>.Read is not annotated, but this override requires dynamic code")]
    public override ErrorCode Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException("ErrorCode must be serialized as an object");

        String? assemblyQualifiedTypeName = null;
        String? name = null;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                break;

            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                var propertyName = reader.GetString();
                reader.Read();

                if (propertyName == "$type")
                    assemblyQualifiedTypeName = reader.GetString();
                else if (propertyName == "$name")
                    name = reader.GetString();
            }
        }

        if (String.IsNullOrEmpty(assemblyQualifiedTypeName))
            throw new JsonException("ErrorCode requires $type property");

        // Load the type and get its singleton Instance property or field
        var type = Type.GetType(assemblyQualifiedTypeName);
        if (type is null || !typeof(ErrorCode).IsAssignableFrom(type))
            throw new JsonException($"Unknown or invalid ErrorCode type: '{assemblyQualifiedTypeName}'");

        // Try field first (public static readonly), then property
        var instanceField = type.GetField("Instance",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

        if (instanceField is not null)
        {
            var instance = instanceField.GetValue(null) as ErrorCode;
            if (instance is null)
                throw new JsonException($"Failed to retrieve singleton instance from '{type.FullName}.Instance' field");
            return instance;
        }

        var instanceProperty = type.GetProperty("Instance",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

        if (instanceProperty is not null)
        {
            var instance = instanceProperty.GetValue(null) as ErrorCode;
            if (instance is null)
                throw new JsonException($"Failed to retrieve singleton instance from '{type.FullName}.Instance' property");
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
