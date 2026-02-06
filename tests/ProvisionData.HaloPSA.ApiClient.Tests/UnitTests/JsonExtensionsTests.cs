// Provision Data HaloPSA API Client
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

namespace ProvisionData.HaloPSA.UnitTests;

public class JsonExtensionsTests
{
    [Fact]
    public void ToFormattedJson_AcceptsUnformatted_ReturnFomatted()
    {
        // Arrange
        const String unformatted = @"{""name"":""Test"",""object"":{""id"":1,""type"":""test""},""value"":123,""items"":[1,2,3]}";
        const String expected = """
            {
              "name": "Test",
              "object": {
                "id": 1,
                "type": "test"
              },
              "value": 123,
              "items": [
                1,
                2,
                3
              ]
            }
            """;

        // Act
        var formatted = unformatted.ToFormattedJson();

        // Assert
        formatted.Should().NotBeNull();
        formatted.Should().Be(expected);
    }
}
