/*******************************************************************************
 * MIT License
 *
 * Copyright 2020 Provision Data Systems Inc.  https://provisiondata.com
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a 
 * copy of this software and associated documentation files (the "Software"),
 * to deal in the Software without restriction, including without limitation
 * the rights to use, copy, modify, merge, publish, distribute, sublicense,
 * and/or sell copies of the Software, and to permit persons to whom the 
 * Software is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
 * DEALINGS IN THE SOFTWARE.
 *
 *******************************************************************************/

namespace ProvisionData.Net
{
    using System;

    public readonly struct EndPoint : IEquatable<EndPoint>, IComparable<EndPoint>
    {
        public static readonly EndPoint Empty = new EndPoint();

        public EndPoint(String address) : this(address, 0) { }

        public EndPoint(String address, Int32 port)
        {
            if (String.IsNullOrWhiteSpace(address) || !Ptn.IPv4.IsMatch(address))
            {
                throw new ArgumentException($"'{address}' is not a valid IPv4 Address", nameof(address));
            }

            if (port < 0 || port > 65535)
            {
                throw new ArgumentOutOfRangeException(nameof(port), $"'{port}' is not a valid IPv4 Port");
            }

            Address = address ?? throw new ArgumentNullException(nameof(address));
            Port = port;
        }

        public String Address { get; }
        public Int32 Port { get; }

        public override String ToString() => Port == 0 ? Address : $"{Address}:{Port}";

        public Int32 CompareTo(EndPoint other)
        {
            // If other is Empty, this instance is greater.
            if (other.Equals(Empty))
            {
                return 1;
            }

            var result = IPAddressComparer.Compare(Address, other.Address);

            return result == 0 ? Port.CompareTo(other.Port) : result;
        }

        public static Boolean operator >(EndPoint x, EndPoint y) => x.CompareTo(y) == 1;
        public static Boolean operator <(EndPoint x, EndPoint y) => x.CompareTo(y) == -1;
        public static Boolean operator >=(EndPoint x, EndPoint y) => x.CompareTo(y) >= 0;
        public static Boolean operator <=(EndPoint x, EndPoint y) => x.CompareTo(y) <= 0;

        public override Int32 GetHashCode() => (Address, Port).GetHashCode();
        public Boolean Equals(EndPoint other) => Address == other.Address && (Port == 0 || other.Port == 0 || Port == other.Port);
        public override Boolean Equals(Object obj) => obj is EndPoint other && Equals(other);
        public static Boolean operator ==(EndPoint x, EndPoint y) => x.Equals(y);
        public static Boolean operator !=(EndPoint x, EndPoint y) => !x.Equals(y);

        public static EndPoint Parse(String value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (TryParse(value, out var endPoint))
            {
                return endPoint;
            }

            throw new ArgumentException($"'{value}' cannot be parsed as an EndPoint", nameof(value));
        }

        public static Boolean TryParse(String value, out EndPoint endPoint)
        {
            if (!String.IsNullOrWhiteSpace(value))
            {
                var normalized = value.Trim();
                if (Ptn.IPv4AndOptionalPort.IsMatch(normalized))
                {
                    var tokens = normalized.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                    var portNumber = tokens.Length == 2 ? Int32.Parse(tokens[1]) : 0;
                    endPoint = new EndPoint(tokens[0], portNumber);
                    return true;
                }
            }

            endPoint = Empty;
            return false;
        }
    }
}
