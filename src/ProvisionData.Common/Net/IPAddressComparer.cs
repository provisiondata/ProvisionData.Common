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

public class IPAddressComparer : IComparer<String>
{
    Int32 IComparer<String>.Compare(String? left, String? right)
        => IPAddressComparer.Compare(left, right);

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

    private static Int32[] Split(String value)
        => value.Split('.').Select(Int32.Parse).ToArray();
}
