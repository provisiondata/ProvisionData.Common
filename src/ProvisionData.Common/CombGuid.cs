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

using System.Diagnostics;

namespace ProvisionData;
// https://github.com/nhibernate/nhibernate-core/blob/master/src/NHibernate/Id/GuidCombGenerator.cs
// GNU LESSER GENERAL PUBLIC LICENSE
[DebuggerStepThrough]
public static class CombGuid
{
    public static Guid NewGuid()
    {
        var guidArray = Guid.NewGuid().ToByteArray();

        var baseDate = new DateTime(1900, 1, 1);
        var now = DateTime.Now;

        // Get the days and milliseconds which will be used to build the byte string
        var days = new TimeSpan(now.Ticks - baseDate.Ticks);
        var msecs = now.TimeOfDay;

        // Convert to a byte array
        // Note that SQL Server is accurate to 1/300th of a millisecond so we divide by 3.333333
        var daysArray = BitConverter.GetBytes(days.Days);
        var msecsArray = BitConverter.GetBytes((Int64)(msecs.TotalMilliseconds / 3.333333));

        // Reverse the bytes to match SQL Servers ordering
        Array.Reverse(daysArray);
        Array.Reverse(msecsArray);

        // Copy the bytes into the guid
        Array.Copy(daysArray, daysArray.Length - 2, guidArray, guidArray.Length - 6, 2);
        Array.Copy(msecsArray, msecsArray.Length - 4, guidArray, guidArray.Length - 4, 4);

        return new Guid(guidArray);
    }
}
