namespace ProvisionData.Extensions
{
    using System;
    using FluentAssertions;
    using Xunit;

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
}
