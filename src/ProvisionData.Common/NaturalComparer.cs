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

using System.Text.RegularExpressions;

namespace ProvisionData;

/// <summary>
/// Compares objects using natural ordering, where numeric sequences are compared numerically rather than lexicographically.
/// </summary>
/// <typeparam name="T">The type of objects to compare.</typeparam>
/// <remarks>
/// This comparer is useful for sorting alphanumeric strings in a human-friendly way.
/// For example, "file2.txt" comes before "file10.txt" in natural order.
/// </remarks>
public partial class NaturalComparer<T> : IComparer<T>
{
    private readonly EnumerableComparer<Object> _comparer = new();

    /// <summary>
    /// Gets a compiled regular expression for splitting numeric sequences.
    /// </summary>
    /// <returns>A compiled regex for identifying numeric sequences.</returns>
    [GeneratedRegex("([0-9]+)", RegexOptions.Compiled)]
    private partial Regex SplitRegex();

    /// <summary>
    /// Gets a compiled regular expression for replacing multiple whitespace characters.
    /// </summary>
    /// <returns>A compiled regex matching one or more whitespace characters.</returns>
    [GeneratedRegex(@"\s+", RegexOptions.Compiled)]
    private partial Regex ReplaceRegex();

    /// <summary>
    /// Converts a sequence of strings to an array of objects, parsing numeric strings as integers.
    /// </summary>
    /// <param name="str">The sequence of strings to convert.</param>
    /// <returns>An array of objects containing integers or strings.</returns>
    private static Object[] ConvertToObjects(IEnumerable<String> str)
    {
        var list = new List<Object>();
        foreach (var s in str)
        {
            if (String.IsNullOrEmpty(s))
            {
                continue;
            }

            if (Int32.TryParse(s, out var result))
            {
                list.Add(result);
            }
            else
            {
                list.Add(s);
            }
        }

        return list.ToArray();
    }

    /// <summary>
    /// Compares two objects using natural ordering.
    /// </summary>
    /// <param name="x">The first object to compare.</param>
    /// <param name="y">The second object to compare.</param>
    /// <returns>A value indicating the relative order of the objects.</returns>
    public Int32 Compare(T? x, T? y)
    {
        if (x == null && y == null)
        {
            return 0;
        }

        if (x == null)
        {
            return -1;
        }

        if (y == null)
        {
            return 1;
        }

        var xr = ReplaceRegex().Replace(x.ToString()!, "");
        var xs = SplitRegex().Split(xr);
        var xc = NaturalComparer<T>.ConvertToObjects(xs);
        var yr = ReplaceRegex().Replace(y.ToString()!, "");
        var ys = SplitRegex().Split(yr);
        var yc = NaturalComparer<T>.ConvertToObjects(ys);
        return _comparer.Compare(xc, yc);
    }
}
