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

public record Error(string Code, string Description)
{
    public static readonly Error None = new(string.Empty, string.Empty);

    // Factory methods for common error types
    public static Error NotFound(string code, string description)
        => new NotFoundError(code, description);

    public static Error Validation(string code, string description)
        => new ValidationError(code, description);

    public static Error Conflict(string code, string description)
        => new ConflictError(code, description);

    public static Error Unauthorized(string code, string description)
        => new UnauthorizedError(code, description);

    public static Error Failure(string code, string description)
        => new(code, description);
}

// Typed errors for better categorisation
public sealed record NotFoundError(string Code, string Description)
    : Error(Code, Description);

public sealed record ValidationError(string Code, string Description)
    : Error(Code, Description);

public sealed record ConflictError(string Code, string Description)
    : Error(Code, Description);

public sealed record UnauthorizedError(string Code, string Description)
    : Error(Code, Description);