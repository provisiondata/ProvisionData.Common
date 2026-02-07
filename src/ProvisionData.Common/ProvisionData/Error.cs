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
/// Represents an error with a code and description.
/// </summary>
[JsonConverter(typeof(ErrorJsonConverter))]
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
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
