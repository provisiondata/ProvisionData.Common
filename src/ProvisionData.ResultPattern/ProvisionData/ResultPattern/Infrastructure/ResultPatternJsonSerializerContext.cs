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

//using ProvisionData.ResultPattern.Infrastructure;
//using System.Text.Json;
//using System.Text.Json.Serialization;

//namespace ProvisionData.ResultPattern;

///// <summary>
///// Provides source generation context for serializing and deserializing <see cref="ResultPattern.Result"/>,
///// <see cref="Result{T}"/>, <see cref="ResultPattern.Error"/>, and <see cref="ResultPattern.ErrorCode"/> types
///// using System.Text.Json.
///// </summary>
///// <remarks>This context enables efficient, reflection-free JSON serialization and deserialization for the
///// specified types when used with System.Text.Json source generation. Register this context with JsonSerializerOptions
///// to improve performance and reduce startup overhead in applications that frequently serialize or deserialize these
///// types.</remarks>
//[JsonSourceGenerationOptions(
//    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
//    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
//public partial class ResultPatternJsonSerializerContext : JsonSerializerContext
//{
//    static partial void CustomizeOptions(JsonSerializerOptions options)
//    {
//        options.Converters.Add(new ErrorJsonConverter());
//        options.Converters.Add(new ResultJsonConverter());
//        options.Converters.Add(new ResultOfTJsonConverterFactory());
//    }

//}

