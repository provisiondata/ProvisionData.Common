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

using ProvisionData.Net;
using Xunit;

namespace ProvisionData.UnitTests.Net;

public class EndPointTests
{
    [Fact]
    public void EndPoints_are_equatable()
    {
        new EndPoint("10.10.10.10").Equals(new EndPoint("10.10.10.10")).Should().BeTrue();
        new EndPoint("10.10.10.10").Equals(new EndPoint("10.10.10.11")).Should().BeFalse();

        new EndPoint("10.10.10.10", 10).Equals(new EndPoint("10.10.10.10", 10)).Should().BeTrue();
        new EndPoint("10.10.10.10", 10).Equals(new EndPoint("10.10.10.10", 11)).Should().BeFalse();

        (new EndPoint("10.10.10.10") == new EndPoint("10.10.10.10")).Should().BeTrue();
        (new EndPoint("10.10.10.10") != new EndPoint("10.10.10.11")).Should().BeTrue();

        (new EndPoint("10.10.10.10", 10) == new EndPoint("10.10.10.10", 10)).Should().BeTrue();
        (new EndPoint("10.10.10.10", 10) != new EndPoint("10.10.10.10", 11)).Should().BeTrue();
    }

    [Fact]
    public void EndPoints_with_same_Address_and_one_with_Port_0_are_equal()
    {
        new EndPoint("10.10.10.10", 0).Equals(new EndPoint("10.10.10.10", 1000)).Should().BeTrue();
    }

    [Fact]
    public void EndPoints_are_compareable()
    {
        new EndPoint("110.10.10.10").Should().BeLessThan(new EndPoint("120.10.10.10"));
        new EndPoint("110.10.10.10").Should().BeGreaterThan(new EndPoint("100.10.10.10"));

        new EndPoint("110.10.10.10").Should().BeLessThan(new EndPoint("110.10.10.10", 100));
        new EndPoint("110.10.10.10", 100).Should().BeGreaterThan(new EndPoint("110.10.10.10"));

        new EndPoint("110.10.10.10", 1000).Should().BeLessThan(new EndPoint("110.10.10.10", 2000));
        new EndPoint("110.10.10.10", 2000).Should().BeGreaterThan(new EndPoint("110.10.10.10", 1000));
    }
}
