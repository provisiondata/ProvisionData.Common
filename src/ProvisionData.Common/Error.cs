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
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ProvisionData;

/// <summary>
/// Represents an error with a code and description.
/// </summary>
[JsonConverter(typeof(ErrorJsonConverter))]
[RequiresUnreferencedCode("Error serialization/deserialization uses reflection to discover types, constructors, and properties, and may not work with trimming.")]
[RequiresDynamicCode("Error serialization/deserialization uses Type.GetType() and reflection which requires dynamic code generation.")]
public class Error
{
    /// <summary>
    /// Gets the code identifying the error.
    /// </summary>
    public ErrorCode Code { get; }

    /// <summary>
    /// Gets the human-readable description of the error.
    /// </summary>
    public String Description { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Error"/> class.
    /// </summary>
    /// <param name="code">A code identifying the error. Must not be null.</param>
    /// <param name="description">A human-readable description of the error. Must not be null, empty, or whitespace.</param>
    public Error(ErrorCode code, String description)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        Code = code ?? throw new ArgumentNullException(nameof(code));
        Description = description;
    }

    /// <summary>
    /// Creates a new API error with the specified error code and description.
    /// </summary>
    /// <param name="description">A human-readable description of the error. Must not be null.</param>
    /// <returns>An <see cref="Error"/> instance representing the specified API error.</returns>
    public static Error ApiError(String description)
        => new ApiError(description);

    /// <summary>
    /// Creates an error that represents a violation of a business rule.
    /// </summary>
    /// <param name="description">A human-readable description of the business rule violation. Must not be null or empty.</param>
    /// <returns>An <see cref="Error"/> instance representing the business rule violation, containing the specified
    /// description.</returns>
    public static Error BusinessRuleViolation(String description)
        => new BusinessRuleViolationError(description);

    /// <summary>
    /// Creates a new error instance representing a configuration error with the specified description.
    /// </summary>
    /// <param name="description">A detailed message describing the nature of the configuration error. This information is intended to assist in
    /// diagnosing or resolving the issue.</param>
    /// <returns>An <see cref="Error"/> object that encapsulates the provided description, representing a
    /// configuration error.</returns>
    public static Error Configuration(String description)
        => new ConfigurationError(description);

    /// <summary>
    /// Creates a <see cref="ConflictError"/> with the specified description.
    /// </summary>
    /// <param name="description">The error description.</param>
    /// <returns>A <see cref="ConflictError"/> instance.</returns>
    public static Error Conflict(String description)
        => new ConflictError(description);

    /// <summary>
    /// Creates a <see cref="NotFoundError"/> with the specified description.
    /// </summary>
    /// <param name="description">The error description.</param>
    /// <returns>A <see cref="NotFoundError"/> instance.</returns>
    public static Error NotFound(String description)
        => new NotFoundError(description);

    /// <summary>
    /// Creates an <see cref="UnauthorizedError"/> with the specified description.
    /// </summary>
    /// <param name="description">The error description.</param>
    /// <returns>An <see cref="UnauthorizedError"/> instance.</returns>
    public static Error Unauthorized(String description)
        => new UnauthorizedError(description);

    /// <summary>
    /// Creates an error object representing an unhandled exception. Usage of this error should
    /// be rare and typically indicates an unexpected failure in the system.
    /// </summary>
    /// <param name="exception">The exception to be encapsulated as an unhandled error. Must not be null.</param>
    /// <returns>An Error instance that wraps the specified exception as an unhandled error.</returns>
    public static Error Exception(Exception exception)
        // Note: Exceptions are generally not serializable, so we only capture the type name and message.
        => new UnhandledExceptionError($"{exception.GetType().Name}: {exception.Message}");

    /// <summary>
    /// Creates a <see cref="ValidationError"/> with the specified description.
    /// </summary>
    /// <param name="description">The error description.</param>
    /// <returns>A <see cref="ValidationError"/> instance.</returns>
    public static Error Validation(String description)
        => new ValidationError(description);

    /// <summary>
    /// Determines whether this error is of a specific error type.
    /// </summary>
    /// <typeparam name="TError">The error type to check for.</typeparam>
    /// <returns>True if this error is of the specified type; otherwise, false.</returns>
    public Boolean IsErrorType<TError>() where TError : Error
        => this is TError;
}

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
internal sealed class ErrorJsonConverter : JsonConverter<Error>
{
    [RequiresUnreferencedCode("Error deserialization uses reflection to discover constructors and properties, and may not work with trimming.")]
    [RequiresDynamicCode("Error deserialization uses Type.GetType() and reflection which requires dynamic code generation.")]
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Method is already annotated with RequiresUnreferencedCode")]
    [UnconditionalSuppressMessage("Trimming", "IL2046", Justification = "Base JsonConverter<T>.Read is not annotated, but this override requires reflection")]
    [UnconditionalSuppressMessage("Trimming", "IL2072", Justification = "Method is already annotated with RequiresUnreferencedCode")]
    [UnconditionalSuppressMessage("Trimming", "IL2075", Justification = "Method is already annotated with RequiresUnreferencedCode")]
    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "Method is already annotated with RequiresDynamicCode")]
    [UnconditionalSuppressMessage("AOT", "IL3051", Justification = "Base JsonConverter<T>.Read is not annotated, but this override requires dynamic code")]
    public override Error Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        // Get the type discriminator
        if (!root.TryGetProperty("$type", out var typeElement))
            throw new JsonException("Error serialization requires $type property");

        var typeName = typeElement.GetString();
        if (String.IsNullOrEmpty(typeName))
            throw new JsonException("Error $type cannot be null or empty");

        // Load the type
        var type = Type.GetType(typeName);
        if (type is null || !typeof(Error).IsAssignableFrom(type))
            throw new JsonException($"Unknown or invalid Error type: '{typeName}'");

        // Get all constructors and find the one with the most parameters (primary constructor for records)
        var constructors = type.GetConstructors()
            .OrderByDescending(c => c.GetParameters().Length)
            .ToArray();

        if (constructors.Length == 0)
            throw new JsonException($"Error type '{type.FullName}' has no public constructors");

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

    [RequiresUnreferencedCode("Error serialization uses reflection to discover all public properties and may not work with trimming.")]
    [RequiresDynamicCode("Error serialization uses reflection which requires dynamic code generation.")]
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Method is already annotated with RequiresUnreferencedCode")]
    [UnconditionalSuppressMessage("Trimming", "IL2046", Justification = "Base JsonConverter<T>.Write is not annotated, but this override requires reflection")]
    [UnconditionalSuppressMessage("Trimming", "IL2075", Justification = "Method is already annotated with RequiresUnreferencedCode")]
    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "Method is already annotated with RequiresDynamicCode")]
    [UnconditionalSuppressMessage("AOT", "IL3051", Justification = "Base JsonConverter<T>.Write is not annotated, but this override requires dynamic code")]
    public override void Write(Utf8JsonWriter writer, Error value, JsonSerializerOptions options)
    {
        var type = value.GetType();

        writer.WriteStartObject();

        // Write type discriminator
        writer.WriteString("$type", type.AssemblyQualifiedName);

        // Write all public properties
        var properties = type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        foreach (var property in properties)
        {
            // Skip properties that don't have a getter or are marked with JsonIgnore
            if (!property.CanRead || property.GetCustomAttribute<JsonIgnoreAttribute>() is not null)
                continue;

            var propertyValue = property.GetValue(value);

            writer.WritePropertyName(property.Name);
            JsonSerializer.Serialize(writer, propertyValue, property.PropertyType, options);
        }

        writer.WriteEndObject();
    }
}


