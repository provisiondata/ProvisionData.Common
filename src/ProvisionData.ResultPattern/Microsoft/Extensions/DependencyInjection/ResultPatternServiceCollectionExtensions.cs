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

using Microsoft.Extensions.Options;
using ProvisionData.ResultPattern;
using ProvisionData.ResultPattern.Infrastructure;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Provides extension methods for registering result pattern serialization and configuration services in an ASP.NET
/// Core application's dependency injection container.
/// </summary>
/// <remarks>This class enables integration of result pattern o and JSON serialization settings into the
/// application's service collection. Use the provided extension method to configure result pattern support, including
/// custom JSON type information resolvers for error polymorphism. Thread safety and correct configuration are ensured
/// when used during application startup.</remarks>
public static class ResultPatternServiceCollectionExtensions
{
    /// <summary>
    /// Adds result pattern services to the specified service collection.
    /// </summary>
    /// <param name="services">The service collection to add the result pattern services to.</param>
    /// <param name="options">An optional action to configure the result pattern o.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddResultPattern(this IServiceCollection services, Action<ResultPatternOptions>? options = null)
    {
        services.Configure<ResultPatternOptions>(o =>
        {
            var json = o.JsonSerializerOptions;

            json.TypeInfoResolverChain.Insert(0, ResultPatternJsonSerializerContext.Default);

            json.TypeInfoResolverChain.Insert(1, new DefaultJsonTypeInfoResolver
            {
                Modifiers =
                {
                    ti => ErrorJsonPolymorphism.Apply(ti)
                }
            });
        });

        services.PostConfigure<JsonSerializerOptions>(o =>
        {
            var rp = services.BuildServiceProvider().GetRequiredService<IOptions<ResultPatternOptions>>().Value;

            // Merge your JSON settings into ASP.NET's
            foreach (var resolver in rp.JsonSerializerOptions.TypeInfoResolverChain)
            {
                if (!o.TypeInfoResolverChain.Contains(resolver))
                {
                    o.TypeInfoResolverChain.Add(resolver);
                }
            }
        });

        return services;
    }
}
