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

public static partial class ResultExtensions
{
    /// <summary>
    /// Converts a result to a typed result with the specified value.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="result">The result to convert.</param>
    /// <param name="value">The value for the successful result.</param>
    /// <returns>A successful <see cref="Result{T}"/> with the specified value, or a failed result with the original error.</returns>
    public static Result<T> ToResult<T>(this Result result, T value)
    {
        return result.IsSuccess
            ? Result<T>.Success(value)
            : Result<T>.Failure(result.Error);
    }

    /// <summary>
    /// Gets the value from a successful result or returns the specified default value if the result failed.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="result">The result to extract the value from.</param>
    /// <param name="defaultValue">The value to return if the result is failed.</param>
    /// <returns>The result value if successful; otherwise, the default value.</returns>
    public static T GetValueOrDefault<T>(this Result<T> result, T defaultValue)
    {
        return result.IsSuccess ? result.Value : defaultValue;
    }

    /// <summary>
    /// Gets the value from a successful result or returns the default value for the type if the result failed.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="result">The result to extract the value from.</param>
    /// <returns>The result value if successful; otherwise, the default value for type T.</returns>
    public static T? GetValueOrDefault<T>(this Result<T> result)
    {
        return result.IsSuccess ? result.Value : default;
    }
}

