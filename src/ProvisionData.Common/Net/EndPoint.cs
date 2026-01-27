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

public readonly struct EndPoint : IEquatable<EndPoint>, IComparable<EndPoint>
{
    public static readonly EndPoint Empty = new();

    public EndPoint(String address) : this(address, 0) { }

    public EndPoint(String address, Int32 port)
    {
        if (String.IsNullOrWhiteSpace(address) || !Rgx.IPv4().IsMatch(address))
        {
            throw new ArgumentException($"'{address}' is not a valid IPv4 Address", nameof(address));
        }

        if (port is < 0 or > 65535)
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
    public override Boolean Equals(Object? obj) => obj is EndPoint other && Equals(other);
    public static Boolean operator ==(EndPoint x, EndPoint y) => x.Equals(y);
    public static Boolean operator !=(EndPoint x, EndPoint y) => !x.Equals(y);

    public static EndPoint Parse(String value)
    {
        ArgumentNullException.ThrowIfNull(value);

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
            if (Rgx.IPv4AndOptionalPort().IsMatch(normalized))
            {
                var tokens = normalized.Split([':'], StringSplitOptions.RemoveEmptyEntries);
                var portNumber = tokens.Length == 2 ? Int32.Parse(tokens[1]) : 0;
                endPoint = new EndPoint(tokens[0], portNumber);
                return true;
            }
        }

        endPoint = Empty;
        return false;
    }
}
