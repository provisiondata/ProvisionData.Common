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

namespace ProvisionData.Net;

/// <summary>
/// Compares IPv4 addresses represented as strings by comparing each octet numerically.
/// </summary>
public class IPAddressComparer : IComparer<String>
{
    /// <summary>
    /// Compares two IPv4 address strings.
    /// </summary>
    /// <param name="left">The first IPv4 address string to compare.</param>
    /// <param name="right">The second IPv4 address string to compare.</param>
    /// <returns>A value indicating the relative order of the addresses.</returns>
    Int32 IComparer<String>.Compare(String? left, String? right)
        => Compare(left, right);

    /// <summary>
    /// Compares two IPv4 address strings.
    /// </summary>
    /// <param name="left">The first IPv4 address string to compare.</param>
    /// <param name="right">The second IPv4 address string to compare.</param>
    /// <returns>A value indicating the relative order of the addresses.</returns>
    /// <remarks>
    /// Null values are considered less than non-null values.
    /// IP addresses are compared octet-by-octet from left to right.
    /// </remarks>
    public static Int32 Compare(String? left, String? right)
    {
        if (left == null && right == null)
        {
            return 0;
        }

        if (left == null)
        {
            return -1;
        }

        if (right == null)
        {
            return 1;
        }

        var l = Split(left);
        var r = Split(right);

        if (l[0] == r[0])
        {
            if (l[1] == r[1])
            {
                if (l[2] == r[2])
                {
                    return l[3].CompareTo(r[3]);
                }

                return l[2].CompareTo(r[2]);
            }

            return l[1].CompareTo(r[1]);
        }

        return l[0].CompareTo(r[0]);
    }

    /// <summary>
    /// Splits an IPv4 address string into four octets as integers.
    /// </summary>
    /// <param name="value">An IPv4 address string in dotted-quad notation.</param>
    /// <returns>An array of four integers representing the octets.</returns>
    private static Int32[] Split(String value)
        => value.Split('.').Select(Int32.Parse).ToArray();
}
