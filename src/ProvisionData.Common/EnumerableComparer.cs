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
/// Compares two sequences.
/// </summary>
/// <typeparam name="T">Type of item in the sequences.</typeparam>
/// <remarks>
/// Compares elements from the two input sequences in turn. If we
/// run out of list before finding unequal elements, then the shorter
/// list is deemed to be the lesser list.
/// </remarks>
public class EnumerableComparer<T> : IComparer<IEnumerable<T>>
{
    /// <summary>
    /// Create a sequence comparer using the default comparer for T.
    /// </summary>
    public EnumerableComparer()
    {
        _comp = Comparer<T>.Default;
    }

    /// <summary>
    /// Create a sequence comparer, using the specified item comparer
    /// for T.
    /// </summary>
    /// <param name="comparer">Comparer for comparing each pair of
    /// items from the sequences.</param>
    public EnumerableComparer(IComparer<T> comparer)
    {
        _comp = comparer;
    }

    /// <summary>
    /// Object used for comparing each element.
    /// </summary>
    private readonly IComparer<T> _comp;

    /// <summary>
    /// Compare two sequences of T.
    /// </summary>
    /// <param name="x">First sequence.</param>
    /// <param name="y">Second sequence.</param>
    public Int32 Compare(IEnumerable<T>? x, IEnumerable<T>? y)
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

        using (var leftIt = x.GetEnumerator())
        using (var rightIt = y.GetEnumerator())
        {
            while (true)
            {
                var left = leftIt.MoveNext();
                var right = rightIt.MoveNext();

                if (!(left || right))
                {
                    return 0;
                }

                if (!left)
                {
                    return -1;
                }

                if (!right)
                {
                    return 1;
                }

                var itemResult = _comp.Compare(leftIt.Current, rightIt.Current);
                if (itemResult != 0)
                {
                    return itemResult;
                }
            }
        }
    }
}
