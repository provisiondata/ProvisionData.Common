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
using ProvisionData.Extensions;
using Xunit;

namespace ProvisionData.UnitTests.Extensions;

public class IPAddressExtensionsTests
{
    [Theory]
    [InlineData("0.0.0.0", 0u)]
    [InlineData("0.0.0.1", 1u)]
    [InlineData("255.255.255.255", 4294967295u)]
    [InlineData("10.30.0.11", 169738251u)]
    [InlineData("10.250.0.1", 184156161u)]
    [InlineData("208.73.58.199", 3494460103u)]
    public void IpAddressToUInt32(String input, UInt32 expected)
        => input.IpAddressToUInt32().Should().Be(expected);

    [Theory]
    [InlineData(0u, "0.0.0.0")]
    [InlineData(1u, "0.0.0.1")]
    [InlineData(4294967294u, "255.255.255.254")]
    [InlineData(169738251u, "10.30.0.11")]
    [InlineData(184156161u, "10.250.0.1")]
    [InlineData(3494460103u, "208.73.58.199")]
    public void UInt32ToIpAddress(UInt32 input, String expected)
        => input.UInt32ToIpAddress().Should().Be(expected);

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("a.b.c.d")]
    [InlineData("256.256.256.256")]
    public void IpAddressToUInt32_throws_FormatException_on_invalid_IPv4_Address(String input)
        => Assert.Throws<FormatException>(() => input.IpAddressToUInt32());
}
