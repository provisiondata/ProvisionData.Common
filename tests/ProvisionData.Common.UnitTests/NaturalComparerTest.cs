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
using Xunit;

namespace ProvisionData.UnitTests;

public class NaturalComparerTest
{
    [Fact]
    public void Test()
    {
        var unsorted = new List<String>() { "z24", "z2", "z15", "z1", "New York", "z3", "z20", "z5", "Newark", "z11", "z 21", "z22", "NewYork" };

        var sorted = unsorted.OrderBy(str => str, new NaturalComparer<String>());

        sorted.Should().ContainInOrder("Newark", "New York", "NewYork", "z1", "z2", "z3", "z5", "z11", "z15", "z20", "z 21", "z22", "z24");
    }
}

