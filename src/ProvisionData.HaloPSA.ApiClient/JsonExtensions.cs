// Provision Data Libraries
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

using System.Text.Json;
using System.Text.Json.Nodes;

namespace ProvisionData.HaloPSA;

/// <summary>
/// Utility extensions for JSON formatting.
/// </summary>
public static class JsonExtensions
{
    private static readonly JsonSerializerOptions JSO = new()
    {
        WriteIndented = true,
        IndentCharacter = ' ',
        IndentSize = 2
    };

    /// <summary>
    /// Formats a JSON string to be more human-readable.
    /// </summary>
    /// <param name="json"></param>
    /// <param name="options">Optional <see cref="JsonSerializerOptions"/> to customize formatting.</param>
    /// <returns>Formatted JSON representation of <paramref name="json"/>.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="json"/> is null.</exception>
    /// <exception cref="JsonException"><paramref name="json"/> does not represent a valid single JSON value.</exception>
    /// <remarks>
    /// The default options use 2-space indentation. You can provide custom <see cref="JsonSerializerOptions"/> if different formatting is desired.
    /// </remarks>
    public static String ToFormattedJson(this String json, JsonSerializerOptions? options = null)
    {
        var reader = JsonNode.Parse(json)
            ?? throw new ArgumentException("Invalid JSON string.", nameof(json));

        return reader.ToJsonString(options ?? JSO);
    }
}
