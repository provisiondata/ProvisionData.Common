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
/// This is the error code that is returned when an error occurs that does not match any of
/// the registered error types.
/// </summary>
/// <remarks> It serves as a fallback for unrecognized errors, allowing
/// the system to handle them gracefully without crashing or losing information. When
/// deserialized, it will contain the raw data of the unrecognized error, which can be
/// useful for logging, debugging, or future analysis to identify new error types that
/// may need to be registered.</remarks>

public class UnrecognizedError(ErrorCode code, String description) : Error(code, description)
{
    internal sealed class UnrecognizedErrorCode : ErrorCode
    {
        /// <summary>
        /// Supports internal fuctionality and is not intended for public use.
        /// </summary>
        internal static readonly UnrecognizedErrorCode Instance = new();

        /// <summary>
        /// Supports internal fuctionality and is not intended for public use.
        /// </summary>
        protected override String Name => nameof(UnrecognizedError);
    }
}
