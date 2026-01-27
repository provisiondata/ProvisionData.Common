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

public class TypeExtensionsTests
{
    [Fact]
    public void GetAllProperties_returns_inherited_properties()
    {
        CommonTypeExtensions.GetAllProperties(typeof(Foo)).Count().Should().Be(1);
        CommonTypeExtensions.GetAllProperties(typeof(Bar)).Count().Should().Be(2);
    }

    private class Foo
    {
        public String Name { get; set; } = String.Empty;
    }

    private class Bar : Foo
    {
        public Int32 Age { get; set; }
    }
}
