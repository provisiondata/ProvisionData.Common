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

namespace ProvisionData;

/// <summary>
/// Represents an error that occurred during an API call. This should be used to wrap HTTP-related errors,
/// including deserialization issues, transport errors, etc., but not Application or Domain-Specific errors.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ApiError"/> class.
/// </remarks>
/// <param name="description">A human-readable description of the API error.</param>
/// <summary>
/// Represents an error that occurred during an API call. This should be used to wrap HTTP-related errors,
/// including deserialization issues, transport errors, etc., but not Application or Domain-Specific errors.
/// </summary>
public sealed class ApiError(String description) : Error(ApiErrorCode.Instance, description)
{
    internal sealed class ApiErrorCode : ErrorCode
    {
        public static readonly ApiErrorCode Instance = new();
        protected override String Name => nameof(ApiError);
    }
}

/// <summary>
/// Represents a business rule violation error.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="BusinessRuleViolationError"/> class.
/// </remarks>
/// <param name="description">A human-readable description of the business rule violation error.</param>
/// <summary>
/// Represents a business rule violation error.
/// </summary>
public sealed class BusinessRuleViolationError(String description) : Error(BusinessRuleViolationErrorCode.Instance, description)
{
    internal sealed class BusinessRuleViolationErrorCode : ErrorCode
    {
        public static readonly BusinessRuleViolationErrorCode Instance = new();
        protected override String Name => nameof(BusinessRuleViolationError);
    }
}

/// <summary>
/// Represents a configuration error.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ConfigurationError"/> class.
/// </remarks>
/// <param name="description">A human-readable description of the configuration error.</param>
/// <summary>
/// Represents a configuration error.
/// </summary>
public sealed class ConfigurationError(String description) : Error(ConfigurationErrorCode.Instance, description)
{
    internal sealed class ConfigurationErrorCode : ErrorCode
    {
        public static readonly ConfigurationErrorCode Instance = new();
        protected override String Name => nameof(ConfigurationError);
    }
}

/// <summary>
/// Represents a conflict error.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ConflictError"/> class.
/// </remarks>
/// <param name="description">A human-readable description of the conflict error.</param>
/// <summary>
/// Represents a conflict error.
/// </summary>
public sealed class ConflictError(String description) : Error(ConflictErrorCode.Instance, description)
{
    internal sealed class ConflictErrorCode : ErrorCode
    {
        public static readonly ConflictErrorCode Instance = new();
        protected override String Name => nameof(ConflictError);
    }
}

/// <summary>
/// Represents a not found error.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="NotFoundError"/> class.
/// </remarks>
/// <param name="description">A human-readable description of the not found error.</param>
/// <summary>
/// Represents a not found error.
/// </summary>
public sealed class NotFoundError(String description) : Error(NotFoundErrorCode.Instance, description)
{
    internal sealed class NotFoundErrorCode : ErrorCode
    {
        public static readonly NotFoundErrorCode Instance = new();
        protected override String Name => nameof(NotFoundError);
    }
}

/// <summary>
/// Represents an unauthorized error.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="UnauthorizedError"/> class.
/// </remarks>
/// <param name="description">A human-readable description of the unauthorized error.</param>
/// <summary>
/// Represents an unauthorized error.
/// </summary>
public sealed class UnauthorizedError(String description) : Error(UnauthorizedErrorCode.Instance, description)
{
    internal sealed class UnauthorizedErrorCode : ErrorCode
    {
        public static readonly UnauthorizedErrorCode Instance = new();
        protected override String Name => nameof(UnauthorizedError);
    }
}

/// <summary>
/// Represents an error caused by an unhandled exception encountered during application execution.
/// The usage of this error type should be rare.
/// </summary>
/// <remarks>
/// Use this type to report unexpected exceptions that were not caught by application logic. The
/// exception details are preserved for diagnostic purposes. In most cases it is probably better
/// to let the exception bubble up.
/// </remarks>
/// <remarks>
/// Initializes a new instance of the <see cref="UnhandledExceptionError"/> class.
/// </remarks>
/// <param name="description">A human-readable description of the unhandled exception error.</param>
/// <summary>
/// Represents an error caused by an unhandled exception encountered during application execution.
/// The usage of this error type should be rare.
/// </summary>
/// <remarks>
/// Use this type to report unexpected exceptions that were not caught by application logic. The
/// exception details are preserved for diagnostic purposes. In most cases it is probably better
/// to let the exception bubble up.
/// </remarks>
public sealed class UnhandledExceptionError(String description) : Error(UnhandledExceptionErrorCode.Instance, description)
{
    internal sealed class UnhandledExceptionErrorCode : ErrorCode
    {
        public static readonly UnhandledExceptionErrorCode Instance = new();
        protected override String Name => nameof(UnhandledExceptionError);
    }
}

/// <summary>
/// Represents a validation error.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ValidationError"/> class.
/// </remarks>
/// <param name="description">A human-readable description of the validation error.</param>
/// <summary>
/// Represents a validation error.
/// </summary>
public sealed class ValidationError(String description) : Error(ValidationErrorCode.Instance, description)
{
    internal sealed class ValidationErrorCode : ErrorCode
    {
        public static readonly ValidationErrorCode Instance = new();
        protected override String Name => nameof(ValidationError);
    }
}
