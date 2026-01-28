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

/// <summary>
/// Provides a testable implementation of GUID generation that allows tests to override the GUID value.
/// </summary>
[DebuggerNonUserCode]
public static class TestableGuid
{
    private static Func<Guid> GetGuid = GetGuidInternal;

    /// <summary>
    /// Sets the GUID value that will be returned by all subsequent calls to <see cref="NewGuid"/>.
    /// </summary>
    /// <param name="guid">The GUID value to return.</param>
    [DebuggerNonUserCode]
    public static void GuidIs(Guid guid) => GetGuid = () => guid;

    /// <summary>
    /// Sets the GUID value that will be returned by all subsequent calls to <see cref="NewGuid"/>.
    /// </summary>
    /// <param name="guid">A string representation of the GUID value to return.</param>
    [DebuggerNonUserCode]
    public static void GuidIs(String guid)
    {
        var g = Guid.Parse(guid);
        GetGuid = () => g;
    }

    /// <summary>
    /// Generates a new GUID, using the overridden value if one has been set, otherwise generates a sequential GUID.
    /// </summary>
    /// <returns>A GUID value.</returns>
    [DebuggerNonUserCode]
    public static Guid NewGuid() => GetGuid?.Invoke() ?? GetGuidInternal();

    /// <summary>
    /// Resets the GUID generator to use the default sequential GUID generation.
    /// </summary>
    [DebuggerNonUserCode]
    public static void Reset() => GetGuid = GetGuidInternal;

    /// <summary>
    /// Generates a new sequential GUID using the <see cref="CombGuid"/> implementation.
    /// </summary>
    /// <returns>A sequential GUID.</returns>
    private static Guid GetGuidInternal() => CombGuid.NewGuid();
}
