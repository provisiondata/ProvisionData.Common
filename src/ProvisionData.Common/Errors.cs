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

namespace ProvisionData;

/// <summary>
/// Represents an error that occurred during an API call. This should be used to wrap HTTP-related errors,
/// including deserialization issues, transport errors, etc., but not Application or Domain-Specific errors.
/// </summary>
public sealed class ApiError : Error
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ApiError"/> class.
    /// </summary>
    /// <param name="description">A human-readable description of the API error.</param>
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Built-in error types are preserved by the library")]
    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "Built-in error types are preserved by the library")]
    public ApiError(String description) : base(ApiErrorCode.Instance, description) { }

    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Built-in error code types are preserved by the library")]
    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "Built-in error code types are preserved by the library")]
    internal sealed class ApiErrorCode : ErrorCode
    {
        public static readonly ApiErrorCode Instance = new();
        protected override String Name => nameof(ApiError);
    }
}

/// <summary>
/// Represents a business rule violation error.
/// </summary>
public sealed class BusinessRuleViolationError : Error
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BusinessRuleViolationError"/> class.
    /// </summary>
    /// <param name="description">A human-readable description of the business rule violation error.</param>
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Built-in error types are preserved by the library")]
    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "Built-in error types are preserved by the library")]
    public BusinessRuleViolationError(String description) : base(BusinessRuleViolationErrorCode.Instance, description) { }

    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Built-in error code types are preserved by the library")]
    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "Built-in error code types are preserved by the library")]
    internal sealed class BusinessRuleViolationErrorCode : ErrorCode
    {
        public static readonly BusinessRuleViolationErrorCode Instance = new();
        protected override String Name => nameof(BusinessRuleViolationError);
    }
}

/// <summary>
/// Represents a configuration error.
/// </summary>
public sealed class ConfigurationError : Error
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigurationError"/> class.
    /// </summary>
    /// <param name="description">A human-readable description of the configuration error.</param>
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Built-in error types are preserved by the library")]
    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "Built-in error types are preserved by the library")]
    public ConfigurationError(String description) : base(ConfigurationErrorCode.Instance, description) { }

    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Built-in error code types are preserved by the library")]
    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "Built-in error code types are preserved by the library")]
    internal sealed class ConfigurationErrorCode : ErrorCode
    {
        public static readonly ConfigurationErrorCode Instance = new();
        protected override String Name => nameof(ConfigurationError);
    }
}

/// <summary>
/// Represents a conflict error.
/// </summary>
public sealed class ConflictError : Error
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConflictError"/> class.
    /// </summary>
    /// <param name="description">A human-readable description of the conflict error.</param>
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Built-in error types are preserved by the library")]
    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "Built-in error types are preserved by the library")]
    public ConflictError(String description) : base(ConflictErrorCode.Instance, description) { }

    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Built-in error code types are preserved by the library")]
    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "Built-in error code types are preserved by the library")]
    internal sealed class ConflictErrorCode : ErrorCode
    {
        public static readonly ConflictErrorCode Instance = new();
        protected override String Name => nameof(ConflictError);
    }
}

/// <summary>
/// Represents a not found error.
/// </summary>
public sealed class NotFoundError : Error
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NotFoundError"/> class.
    /// </summary>
    /// <param name="description">A human-readable description of the not found error.</param>
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Built-in error types are preserved by the library")]
    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "Built-in error types are preserved by the library")]
    public NotFoundError(String description) : base(NotFoundErrorCode.Instance, description) { }

    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Built-in error code types are preserved by the library")]
    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "Built-in error code types are preserved by the library")]
    internal sealed class NotFoundErrorCode : ErrorCode
    {
        public static readonly NotFoundErrorCode Instance = new();
        protected override String Name => nameof(NotFoundError);
    }
}

/// <summary>
/// Represents an unauthorized error.
/// </summary>
public sealed class UnauthorizedError : Error
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnauthorizedError"/> class.
    /// </summary>
    /// <param name="description">A human-readable description of the unauthorized error.</param>
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Built-in error types are preserved by the library")]
    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "Built-in error types are preserved by the library")]
    public UnauthorizedError(String description) : base(UnauthorizedErrorCode.Instance, description) { }

    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Built-in error code types are preserved by the library")]
    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "Built-in error code types are preserved by the library")]
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
public sealed class UnhandledExceptionError : Error
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnhandledExceptionError"/> class.
    /// </summary>
    /// <param name="description">A human-readable description of the unhandled exception error.</param>
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Built-in error types are preserved by the library")]
    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "Built-in error types are preserved by the library")]
    public UnhandledExceptionError(String description) : base(UnhandledExceptionErrorCode.Instance, description) { }

    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Built-in error code types are preserved by the library")]
    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "Built-in error code types are preserved by the library")]
    internal sealed class UnhandledExceptionErrorCode : ErrorCode
    {
        public static readonly UnhandledExceptionErrorCode Instance = new();
        protected override String Name => nameof(UnhandledExceptionError);
    }
}

/// <summary>
/// Represents a validation error.
/// </summary>
public sealed class ValidationError : Error
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationError"/> class.
    /// </summary>
    /// <param name="description">A human-readable description of the validation error.</param>
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Built-in error types are preserved by the library")]
    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "Built-in error types are preserved by the library")]
    public ValidationError(String description) : base(ValidationErrorCode.Instance, description) { }

    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Built-in error code types are preserved by the library")]
    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "Built-in error code types are preserved by the library")]
    internal sealed class ValidationErrorCode : ErrorCode
    {
        public static readonly ValidationErrorCode Instance = new();
        protected override String Name => nameof(ValidationError);
    }
}
