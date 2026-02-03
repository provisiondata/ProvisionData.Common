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

namespace ProvisionData;

/// <summary>
/// Represents a configuration error.
/// </summary>
/// <param name="Code">A code identifying the configuration error.</param>
/// <param name="Description">A human-readable description of the configuration error.</param>
public sealed record ConfigurationError(String Code, String Description) : Error(Code, Description);

/// <summary>
/// Represents a conflict error.
/// </summary>
/// <param name="Code">A code identifying the conflict error.</param>
/// <param name="Description">A human-readable description of the conflict error.</param>
public sealed record ConflictError(String Code, String Description) : Error(Code, Description);

/// <summary>
/// Represents a not found error.
/// </summary>
/// <param name="Code">A code identifying the not found error.</param>
/// <param name="Description">A human-readable description of the not found error.</param>
public sealed record NotFoundError(String Code, String Description) : Error(Code, Description);

/// <summary>
/// Represents a business rule violation error.
/// </summary>
/// <param name="Code">A code identifying the business rule violation error.</param>
/// <param name="Description">A human-readable description of the business rule violation error.</param>
public sealed record BusinessRuleViolationError(String Code, String Description) : Error(Code, Description);

/// <summary>
/// Represents an unauthorized error.
/// </summary>
/// <param name="Code">A code identifying the unauthorized error.</param>
/// <param name="Description">A human-readable description of the unauthorized error.</param>
public sealed record UnauthorizedError(String Code, String Description) : Error(Code, Description);

/// <summary>
/// Represents a validation error.
/// </summary>
/// <param name="Code">A code identifying the validation error.</param>
/// <param name="Description">A human-readable description of the validation error.</param>
public sealed record ValidationError(String Code, String Description) : Error(Code, Description);

/// <summary>
/// Represents an error that occurred during an API call. This should be used to wrap HTTP-related errors,
/// including deserialization issues, transport errors, etc., but not Application or Domain-Specific errors.
/// </summary>
/// <param name="Code">A code identifying the validation error.</param>
/// <param name="Description">A human-readable description of the validation error.</param>
public sealed record ApiError(String Code, String Description) : Error(Code, Description);
