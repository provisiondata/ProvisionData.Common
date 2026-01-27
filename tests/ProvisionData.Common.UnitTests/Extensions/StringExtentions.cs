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
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace ProvisionData.UnitTests.Extensions;

public class StringExtentions
{
    [Theory]
    [InlineData("", 0, "")]
    [InlineData("", 5, "")]
    [InlineData("  ", 0, "")]
    [InlineData("  ", 5, "")]
    [InlineData("Hello", 0, "")]
    [InlineData("Hello", 1, "H")]
    [InlineData("Hello", 5, "Hello")]
    [InlineData("Hello", 6, "Hello")]
    public void Left_should_return_the_correct_result(String input, Int32 length, String expected)
       => input.Left(length).Should().Be(expected);

    [Theory]
    [InlineData(null, null)]
    [InlineData("", "")]
    [InlineData(" ", " ")]
    [InlineData("  ", "  ")]
    [InlineData("Hello", "Hello")]
    [InlineData("hello", "Hello")]
    [InlineData("HelloWorld", "Hello World")]
    [InlineData("helloWorld", "Hello World")]
    [InlineData("AB", "AB")]
    [InlineData("ABC", "ABC")]
    [InlineData("IPAddress", "IP Address")]
    [SuppressMessage("Usage", "xUnit1012:Null should only be used for nullable parameters", Justification = "<Pending>")]
    public void ToProperCase_should_return_the_correct_result(String input, String expected)
       => input.ToProperCase().Should().Be(expected);
}
