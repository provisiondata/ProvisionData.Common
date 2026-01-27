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

namespace ProvisionData.Testing;

[DebuggerNonUserCode]
public static class TestableGuid
{
    private static Func<Guid> GetGuid = GetGuidInternal;

    [DebuggerNonUserCode]
    public static void GuidIs(Guid guid) => GetGuid = () => guid;

    [DebuggerNonUserCode]
    public static void GuidIs(String guid)
    {
        var g = Guid.Parse(guid);
        GetGuid = () => g;
    }

    [DebuggerNonUserCode]
    public static Guid NewGuid() => GetGuid?.Invoke() ?? GetGuidInternal();

    [DebuggerNonUserCode]
    public static void Reset() => GetGuid = GetGuidInternal;

    private static Guid GetGuidInternal() => CombGuid.NewGuid();
}
