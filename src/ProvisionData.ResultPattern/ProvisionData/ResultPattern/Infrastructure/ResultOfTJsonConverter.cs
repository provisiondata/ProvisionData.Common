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
/// Provides a custom JSON converter for serializing and deserializing instances of the <see cref="Result{T}"/> type using
/// System.Text.Json.
/// </summary>
/// <remarks>This converter enables correct handling of <see cref="Result{T}"/> objects when using System.Text.Json, ensuring
/// that both success and failure cases are represented accurately in JSON. Use this converter when you need to
/// serialize or deserialize <see cref="Result{T}"/> values in your application, such as for API responses or data
/// persistence.</remarks>
/// <typeparam name="T">The type of the value contained within the <see cref="Result{T}"/> instance.</typeparam>
public sealed class ResultOfTJsonConverter<T> : JsonConverter<Result<T>>
{
    private sealed record ResultDto(Boolean IsSuccess, Error Error, T? Value);

    /// <inheritdoc/>
    public override Result<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var dto = JsonSerializer.Deserialize<ResultDto>(ref reader, options)
                  ?? throw new JsonException("Unable to deserialize Result<T>.");

        return dto.IsSuccess
            ? Result<T>.Success(dto.Value!)
            : Result<T>.Failure(dto.Error);
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, Result<T> value, JsonSerializerOptions options)
    {
        var dto = new ResultDto(
            value.IsSuccess,
            value.Error,
            value.IsSuccess ? value.Value : default
        );

        JsonSerializer.Serialize(writer, dto, options);
    }
}
