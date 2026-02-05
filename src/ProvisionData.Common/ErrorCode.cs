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
/// Represents an error code value used to identify specific error conditions.
/// </summary>
/// <remarks>
/// ErrorCode uses singleton instances with reference equality semantics. Each error code type
/// has a single static instance that serves as its unique identity.
/// </remarks>
public abstract class ErrorCode : IEquatable<ErrorCode>
{
    /// <summary>
    /// Initializes a new instance of the ErrorCode class.
    /// </summary>
    protected ErrorCode()
    {
    }

    /// <summary>
    /// A human-readable name for the error code.
    /// </summary>
    protected abstract String Name { get; }

    /// <summary>
    /// Returns the name of the error code.
    /// </summary>
    /// <returns>The name of the error code.</returns>
    public override String ToString() => Name;

    /// <summary>
    /// Implicitly converts an <see cref="ErrorCode"/> to a <see cref="String"/> by returning its name.
    /// </summary>
    /// <param name="errorCode">The error code to convert.</param>
    /// <returns>The name of the error code.</returns>
    public static implicit operator String(ErrorCode errorCode) => errorCode.Name;

    /// <summary>
    /// Determines whether this error code is equal to another based on reference equality.
    /// </summary>
    /// <param name="other">The error code to compare to.</param>
    /// <returns>True if both references point to the same singleton instance; otherwise, false.</returns>
    public virtual Boolean Equals(ErrorCode? other) => ReferenceEquals(this, other);

    /// <summary>
    /// Determines whether this error code is equal to another object based on reference equality.
    /// </summary>
    /// <param name="obj">The object to compare to.</param>
    /// <returns>True if both references point to the same singleton instance; otherwise, false.</returns>
    public override Boolean Equals(Object? obj) => ReferenceEquals(this, obj);

    /// <summary>
    /// Returns the hash code for this error code based on its identity.
    /// </summary>
    /// <returns>The identity-based hash code.</returns>
    public override Int32 GetHashCode() => RuntimeHelpers.GetHashCode(this);
}
