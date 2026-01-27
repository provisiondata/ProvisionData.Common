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

using FluentAssertions;
using System.Text.RegularExpressions;
using Xunit;

namespace ProvisionData.UnitTests;

public partial class EnumerableComparerTest
{
    private readonly Func<String, Object> _convert = str =>
    {
        try
        {
            return Int32.Parse(str);
        }
        catch
        {
            return str;
        }
    };

    [Fact]
    public void Test()
    {
        var unsorted = new List<String>() { "z24", "z2", "z15", "z1", "New York", "z3", "z20", "z5", "Newark", "z11", "z 21", "z22", "NewYork" };

        var sorted = unsorted.OrderBy(str => Integers().Split(str.Replace(" ", "")).Select(_convert), new EnumerableComparer<Object>());

        sorted.Should().ContainInOrder("Newark", "New York", "NewYork", "z1", "z2", "z3", "z5", "z11", "z15", "z20", "z 21", "z22", "z24");
    }

    [GeneratedRegex("([0-9]+)")]
    private static partial Regex Integers();
}
