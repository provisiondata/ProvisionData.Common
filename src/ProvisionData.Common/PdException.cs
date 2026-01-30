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
/// Represents an application-specific exception that can carry an optional <see cref="Error"/>
/// object with additional error details.
/// </summary>
/// <remarks>
/// This exception type extends <see cref="Exception"/> and is intended to be used throughout
/// the ProvisionData libraries to surface domain or application errors alongside a richer
/// error model when available.
/// </remarks>
public class PdException : Exception
{
    /// <summary>
    /// Gets an optional <see cref="Error"/> instance containing structured
    /// error information associated with this exception.
    /// </summary>
    public Error? Error { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PdException"/> class.
    /// </summary>
    public PdException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PdException"/> class with a specified
    /// error message.
    /// </summary>
    public PdException(String? message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PdException"/> class with a specified
    /// error message and an optional <see cref="Error"/> object containing additional details.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="error">An optional <see cref="Error"/> instance with structured error details.</param>
    public PdException(String? message, Error? error) : base(message)
    {
        Error = error;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PdException"/> class with a specified
    /// error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public PdException(String? message, Exception? innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PdException"/> class with a specified
    /// error message, an <see cref="Error"/> object containing additional details, and a
    /// reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="error">An optional <see cref="Error"/> instance with structured error details.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public PdException(String? message, Error? error, Exception? innerException) : base(message, innerException)
    {
        Error = error;
    }
}
