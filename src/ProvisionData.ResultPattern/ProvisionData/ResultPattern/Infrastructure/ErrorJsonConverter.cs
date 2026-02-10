// Provision Data Application Framework
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
using System.Text.Json.Serialization;

namespace ProvisionData.ResultPattern.Infrastructure;

/// <summary>
/// JSON converter for <see cref="Error"/> types.
/// </summary>
public sealed class ErrorJsonConverter : JsonConverter<Error>
{
    private sealed record ErrorDto(String Type, String Name, String Description);

    /// <inheritdoc/>
    public override Error Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var dto = JsonSerializer.Deserialize<ErrorDto>(ref reader, options)
                  ?? throw new JsonException("Unable to deserialize Error.");

        var code = ErrorCodeRegistry.GetFor(typeToConvert);
        return new Error(code, dto.Description);
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, Error value, JsonSerializerOptions options)
    {
        var dto = new ErrorDto(
            Type: value.Code.GetType().Name,
            Name: value.Code.ToString(),
            Description: value.Description
        );

        JsonSerializer.Serialize(writer, dto, options);
    }
}
