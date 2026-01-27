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
using ProvisionData.Net;
using Xunit;

namespace ProvisionData.UnitTests.Net;

public class IPAddressComparerTests
{
    [Fact]
    public void IPAddressComparer_works_as_expected()
    {
        var a = new[] { "204.63.46.18", "110.249.212.46", "51.15.144.131", "8.8.8.8", "204.63.42.225", "193.200.164.173" };
        var b = new[] { "8.8.8.8", "51.15.144.131", "110.249.212.46", "193.200.164.173", "204.63.42.225", "204.63.46.18" };

        var list = new List<String>(a);

        list.Sort(new IPAddressComparer());

        list.Should().ContainInOrder(b);
    }
}
