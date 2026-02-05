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

using System.Runtime.CompilerServices;

namespace ProvisionData;

/// <summary>
/// Represents an error that occurred during an API call. This should be used to wrap HTTP-related errors,
/// including deserialization issues, transport errors, etc., but not Application or Domain-Specific errors.
/// </summary>
/// <param name="Description">A human-readable description of the API error.</param>
public sealed record ApiError(String Description) : Error(ApiErrorCode.Instance, Description)
{
    internal sealed class ApiErrorCode : ErrorCode {
        public static readonly ApiErrorCode Instance = new();
        protected override String Name => nameof(ApiError);
    }
}

/// <summary>
/// Represents a business rule violation error.
/// </summary>
/// <param name="Description">A human-readable description of the business rule violation error.</param>
public sealed record BusinessRuleViolationError(String Description) : Error(BusinessRuleViolationErrorCode.Instance, Description)
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
/// <param name="Description">A human-readable description of the configuration error.</param>
public sealed record ConfigurationError(String Description) : Error(ConfigurationErrorCode.Instance, Description)
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
/// <param name="Description">A human-readable description of the conflict error.</param>
public sealed record ConflictError(String Description) : Error(ConflictErrorCode.Instance, Description)
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
/// <param name="Description">A human-readable description of the not found error.</param>
public sealed record NotFoundError(String Description) : Error(NotFoundErrorCode.Instance, Description)
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
/// <param name="Description">A human-readable description of the unauthorized error.</param>
public sealed record UnauthorizedError(String Description) : Error(UnauthorizedErrorCode.Instance, Description)
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
/// <remarks>Use this type to report unexpected exceptions that were not caught by application logic. The
/// exception details are preserved for diagnostic purposes. In most cases it is probably better
/// to let the exception bubble up.
/// </remarks>
/// <param name="Description">A human-readable description of the unhandled exception error.</param>
public sealed record UnhandledExceptionError(String Description) : Error(UnhandledExceptionErrorCode.Instance, Description)
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
/// <param name="Description">A human-readable description of the validation error.</param>
public sealed record ValidationError(String Description) : Error(ValidationErrorCode.Instance, Description)
{
    internal sealed class ValidationErrorCode : ErrorCode
    {
        public static readonly ValidationErrorCode Instance = new();
        protected override String Name => nameof(ValidationError);
    }
}
