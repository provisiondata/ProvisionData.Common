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

namespace ProvisionData.ResultPattern;

/// <summary>
/// Represents an error that occurred during an API call. This should be used to wrap HTTP-related errors,
/// including deserialization issues, transport errors, etc., but not Application or Domain-Specific errors.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ApiError"/> class.
/// </remarks>
/// <param name="description">A human-readable description of the API error.</param>
/// <summary>
/// Represents an error that occurred during an API call. This should be used to wrap HTTP-related errors,
/// including deserialization issues, transport errors, etc., but not Application or Domain-Specific errors.
/// </summary>
public sealed class ApiError(String description) : Error(ApiErrorCode.Instance, description)
{
    internal sealed class ApiErrorCode : ErrorCode
    {
        public static readonly ApiErrorCode Instance = new();
        protected override String Name => nameof(ApiError);
    }
}
