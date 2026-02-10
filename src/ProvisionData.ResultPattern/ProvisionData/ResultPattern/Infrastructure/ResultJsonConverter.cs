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
/// Converter for <see cref="Result"/> that serializes the IsSuccess property and the Error property (if failure).
/// </summary>
public sealed class ResultJsonConverter : JsonConverter<Result>
{
    private sealed record ResultDto(Boolean IsSuccess, Error Error);

    /// <inheritdoc/>
    public override Result Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var dto = JsonSerializer.Deserialize<ResultDto>(ref reader, options)
                  ?? throw new JsonException("Unable to deserialize Result.");

        return dto.IsSuccess
            ? Result.Success()
            : Result.Failure(dto.Error);
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, Result value, JsonSerializerOptions options)
    {
        var dto = new ResultDto(value.IsSuccess, value.Error);
        JsonSerializer.Serialize(writer, dto, options);
    }
}
