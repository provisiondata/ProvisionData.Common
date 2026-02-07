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

using System.Net;

namespace ProvisionData.Extensions;

/// <summary>
/// Extension methods for IP address operations.
/// </summary>
public static class IPAddressExtensions
{
    // Modified from [Saeb Amini](https://stackoverflow.com/users/68080/saeb-amini) via [StackOverflow](https://stackoverflow.com/a/13350494/32588)

    /// <summary>
    /// Converts a valid IPv4 address string to a <see cref="UInt32"/>.
    /// </summary>
    /// <param name="ipAddress">A string that contains an IPv4 address in dotted-quad notation.</param>
    /// <returns>A <see cref="UInt32"/> representing the IP address.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="ipAddress"/> is <see langword="null"/>.</exception>
    /// <exception cref="FormatException"><paramref name="ipAddress"/> is not a valid IPv4 address.</exception>
    /// <remarks>
    /// <para>IP addresses are in network byte order (big-endian), while <see cref="UInt32"/> values are stored in the system's native byte order. 
    /// On little-endian systems (such as Windows), the bytes are reversed to ensure correct conversion.</para>
    /// <para>A <see cref="UInt32"/> is used because even for IPv4, a <see cref="Int32"/> cannot hold addresses larger than 127.255.255.255.</para>
    /// </remarks>
    public static UInt32 IpAddressToUInt32(this String ipAddress)
    {
        var bytes = IPAddress.Parse(ipAddress).GetAddressBytes();

        if (BitConverter.IsLittleEndian)
        {
            // Flip big-endian (Network Order) to little-endian
            Array.Reverse(bytes);
        }

        return BitConverter.ToUInt32(bytes, 0);
    }

    /// <summary>
    /// Converts a <see cref="UInt32"/> to an IPv4 address string.
    /// </summary>
    /// <param name="ipAddress">The <see cref="UInt32"/> value to convert.</param>
    /// <returns>A string representation of the IPv4 address.</returns>
    /// <remarks>
    /// This method reverses the byte conversion process performed by <see cref="IpAddressToUInt32"/>,
    /// handling endianness conversion as needed.
    /// </remarks>
    public static String UInt32ToIpAddress(this UInt32 ipAddress)
    {
        var bytes = BitConverter.GetBytes(ipAddress);

        if (BitConverter.IsLittleEndian)
        {
            // Flip little-endian to big-endian (Network Order)
            Array.Reverse(bytes);
        }

        return new IPAddress(bytes).ToString();
    }
}
