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

namespace ProvisionData.Extensions;

/// <summary>
/// Miscellaneous extension methods.
/// </summary>
public static class MiscExtensions
{
    /// <summary>
    /// Visits the exception and all its inner exceptions, executing an action on each.
    /// </summary>
    /// <param name="ex">The exception to visit.</param>
    /// <param name="action">An action to execute on each exception in the hierarchy.</param>
    /// <remarks>
    /// This method traverses the <see cref="Exception.InnerException"/> chain and executes
    /// the specified action on the original exception and all inner exceptions.
    /// </remarks>
    public static void Visit(this Exception ex, Action<Exception> action)
    {
        action(ex);
        var iex = ex.InnerException;
        while (iex != null)
        {
            action(iex);
            iex = iex.InnerException;
        }
    }

    /// <summary>
    /// Clamps a value between a minimum and maximum value.
    /// </summary>
    /// <typeparam name="T">The type of value to clamp. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="val">The value to clamp.</param>
    /// <param name="min">The minimum allowed value.</param>
    /// <param name="max">The maximum allowed value.</param>
    /// <returns>The original value if it is within the range, otherwise the minimum or maximum value.</returns>
    public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
    {
        if (val.CompareTo(min) < 0)
        {
            return min;
        }
        else if (val.CompareTo(max) > 0)
        {
            return max;
        }
        else
        {
            return val;
        }
    }
}
