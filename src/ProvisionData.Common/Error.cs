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
/// Represents an error with a code and description.
/// </summary>
/// <param name="Code">A code identifying the error.</param>
/// <param name="Description">A human-readable description of the error.</param>
public record Error(String Code, String Description)
{
    /// <summary>
    /// Gets a special <see cref="Error"/> instance representing no error.
    /// </summary>
    public static readonly Error None = new(String.Empty, String.Empty);

    /// <summary>
    /// Creates a <see cref="NotFoundError"/> with the specified code and description.
    /// </summary>
    /// <param name="code">The error code.</param>
    /// <param name="description">The error description.</param>
    /// <returns>A <see cref="NotFoundError"/> instance.</returns>
    public static Error NotFound(String code, String description)
        => new NotFoundError(code, description);

    /// <summary>
    /// Creates a <see cref="ValidationError"/> with the specified code and description.
    /// </summary>
    /// <param name="code">The error code.</param>
    /// <param name="description">The error description.</param>
    /// <returns>A <see cref="ValidationError"/> instance.</returns>
    public static Error Validation(String code, String description)
        => new ValidationError(code, description);

    /// <summary>
    /// Creates a <see cref="ConflictError"/> with the specified code and description.
    /// </summary>
    /// <param name="code">The error code.</param>
    /// <param name="description">The error description.</param>
    /// <returns>A <see cref="ConflictError"/> instance.</returns>
    public static Error Conflict(String code, String description)
        => new ConflictError(code, description);

    /// <summary>
    /// Creates an <see cref="UnauthorizedError"/> with the specified code and description.
    /// </summary>
    /// <param name="code">The error code.</param>
    /// <param name="description">The error description.</param>
    /// <returns>An <see cref="UnauthorizedError"/> instance.</returns>
    public static Error Unauthorized(String code, String description)
        => new UnauthorizedError(code, description);

    /// <summary>
    /// Creates a generic <see cref="Error"/> with the specified code and description.
    /// </summary>
    /// <param name="code">The error code.</param>
    /// <param name="description">The error description.</param>
    /// <returns>An <see cref="Error"/> instance.</returns>
    public static Error Failure(String code, String description)
        => new(code, description);
}
