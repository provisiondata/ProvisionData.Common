// Provision Data Application Framework
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

namespace ProvisionData.ResultPattern;

/// <summary>
/// Represents an error with a code and description.
/// </summary>
//[JsonConverter(typeof(ErrorJsonConverter))]
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
    /// Determines whether this error is of a specific error type.
    /// </summary>
    /// <typeparam name="TError">The error type to check for.</typeparam>
    /// <returns>True if this error is of the specified type; otherwise, false.</returns>
    public Boolean IsErrorType<TError>() where TError : Error
        => this is TError;
}
