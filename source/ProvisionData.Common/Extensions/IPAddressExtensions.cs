namespace ProvisionData.Extensions
{
    using System;
    using System.Net;

    public static class IPAddressExtensions
    {
        // Modified from [Saeb Amini](https://stackoverflow.com/users/68080/saeb-amini) via [StackOverflow](https://stackoverflow.com/a/13350494/32588)

        /// <summary>
        /// Converts a valid IPv4 Address to a <see cref="UInt32"/>.
        /// </summary>
        /// <param name="ipAddress">A string that contains an IPv4 Address in dotted-quad notation.</param>
        /// <returns>a <see cref="UInt32"/></returns>
        /// <exception cref="ArgumentNullException"><paramref name="ipAddress"/> is <code>null</code></exception>
        /// <exception cref="FormatException"><paramref name="ipAddress"/> is not a valid IP Address</exception>
        /// <remarks>
        /// <para>IP Addresses are in Network Order (big-endian), while <see cref="int"/> are little-endian on Windows. To get a correct value on a little-endian system you must reverse the bytes before converting.</para>
        /// <para>Also, even for IPv4, an <see cref="Int32"/> can't hold addresses bigger than 127.255.255.255 so use a <see cref="UInt32"/>.</para>
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
        /// Converts a <see cref="UInt32"/> to an IP Address.
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns>a <see cref="String"/></returns>
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
}
