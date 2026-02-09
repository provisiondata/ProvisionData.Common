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

using ProvisionData.ResultPattern;
using System.Text.Json;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Configuration options for the Result Pattern. This class allows you to customize the behavior of the Result Pattern.
/// Specifically JSON serialization settings and versioning. You can configure these options when calling
/// <see cref="ResultPatternServiceCollectionExtensions.AddResultPattern(IServiceCollection, Action{ResultPatternOptions})"/>
/// to add the Result Pattern to your application's dependency injection container.
/// </summary>
public class ResultPatternOptions
{
    /// <summary>
    /// Gets the options used to configure JSON serialization and deserialization for this instance.
    /// </summary>
    /// <remarks>Use this property to customize serialization behavior, such as property naming policies,
    /// converters, or formatting. Changes to the options affect how JSON data is processed by this instance.</remarks>
    public JsonSerializerOptions JsonSerializerOptions { get; } = new();

    /// <summary>
    /// Gets or sets the version of the Result Pattern. Default is "v1.0". This can be used to manage evolution 
    /// of Custom Errors over time.
    /// </summary>
    public String Version { get; set; } = "v1.0";

    /// <summary>
    /// Enables strict mode for the Custom Error deserialization.
    /// </summary>
    /// <remarks>
    /// <para>
    /// When Strict Mode is enabled, and a <see cref="Result"/> or <see cref="Result{TValue}"/> contains an 
    /// <see cref="ErrorCode"/> or <see cref="Error"/> type that is not registered, deserialization will fail with
    /// an exception instead of silently accepting it. This is especially important in distributed systems where:
    /// </para>
    /// <list type="bullet">
    ///   <item>Clients and servers may be on different versions</item>
    ///   <item>Error types evolve over time</item>
    ///   <item>You want to avoid “mystery errors” that deserialize into nonsense</item>
    ///   <item>You want to detect version mismatches early</item>
    /// </list>
    /// </remarks>
    public Boolean StrictMode { get; set; }
}
