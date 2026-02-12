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

namespace ProvisionData.ResultPattern.Infrastructure;

/// <summary>
/// Represents errors that occur during application execution related to the Result Pattern.
/// This exception serves as a base class for more specific exceptions within the Result
/// Pattern framework, allowing for consistent error handling and improved maintainability
/// of the codebase.
/// </summary>
[Serializable]
public class ResultPatternException : Exception
{
    /// <summary>
    /// Create a useless instance of this exception.
    /// </summary>
    protected ResultPatternException()
    {
    }

    /// <summary>
    /// Creates a less useless instance of this exception with a message describing the error.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ResultPatternException(String? message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the ResultPatternException class with a specified error message and a reference to
    /// the inner exception that is the cause of this exception.
    /// </summary>
    /// <remarks>This constructor is useful for providing detailed error information when an exception is
    /// thrown, allowing for better debugging and error handling.</remarks>
    /// <param name="message">The error message that explains the reason for the exception. This message is intended to be understood by
    /// humans.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is
    /// specified.</param>
    public ResultPatternException(String? message, Exception? innerException) : base(message, innerException)
    {
    }
}
