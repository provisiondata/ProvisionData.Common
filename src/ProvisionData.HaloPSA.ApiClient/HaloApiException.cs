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

namespace ProvisionData.HaloPSA;

/// <summary>
/// Represents errors that occur during HaloPSA API operations.
/// </summary>
public class HaloApiException : Exception
{
    /// <summary>
    /// Initializes a new instance of the HaloApiException class with default values.
    /// </summary>
    public HaloApiException() : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the HaloApiException class with a specified error message.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="json"></param>
    public HaloApiException(String? message, String? json = null) : base(message)
    {
        JSON = json ?? String.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the HaloApiException class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="innerException"></param>
    public HaloApiException(String? message, Exception? innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the HaloApiException class with a specified error message, JSON content, and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="json"></param>
    /// <param name="innerException"></param>
    public HaloApiException(String? message, String? json, Exception? innerException)
        : base(message, innerException)
    {
        JSON = json ?? String.Empty;
    }

    /// <summary>
    /// Gets the JSON-formatted string representing the serialized data or object state.
    /// </summary>
    public String JSON { get; } = String.Empty;
}
