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

using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace ProvisionData.Dapper;

/// <summary>
/// Provides extension methods for registering column mapping in an IServiceCollection.
/// </summary>
/// <remarks>These extension methods simplify the process of scanning assemblies and registering all types that
/// require column mapping into the dependency injection container. Each discovered implementation is registered as all
/// of its implemented interfaces with a transient lifetime. These methods are typically used to automate service
/// registration for applications that utilize column mapping.</remarks>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all implementations of <see cref="INeedColumnMapping"/> from the calling assembly.
    /// </summary>
    /// <param name="services">The service collection to register the mappings in.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance so that additional calls can be chained.</returns>
    public static IServiceCollection AddColumnMapping(this IServiceCollection services)
    {
        return services.AddColumnMapFromAssembly(Assembly.GetCallingAssembly());
    }

    /// <summary>
    /// Registers column mapping services by scanning the assembly that contains the specified type.
    /// </summary>
    /// <typeparam name="T">The type whose containing assembly will be scanned for column mapping definitions.</typeparam>
    /// <param name="services">The service collection to which the column mapping services will be added.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance so that additional calls can be chained.</returns>
    public static IServiceCollection AddColumnMapFromAssemblyContaining<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>(this IServiceCollection services)
    {
        return services.AddColumnMapFromAssembly(typeof(T).Assembly);
    }

    /// <summary>
    /// Scans the specified assemblies for types that implement the <see cref="INeedColumnMapping"/> interface and registers them with
    /// the dependency injection container as transient services.
    /// </summary>
    /// <remarks>Each discovered type that implements INeedColumnMapping is registered as all of its
    /// implemented interfaces with a transient lifetime. This method is typically used to automate service registration
    /// for column mapping implementations.</remarks>
    /// <param name="services">The IServiceCollection to add the discovered services to.</param>
    /// <param name="assembly">One or more assemblies to scan for types that implement INeedColumnMapping. At least one assembly must be
    /// provided.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance so that additional calls can be chained.</returns>
    /// <exception cref="ArgumentException">Thrown if no assemblies are provided in the assembly parameter.</exception>
    public static IServiceCollection AddColumnMapFromAssembly(this IServiceCollection services, params Assembly[] assembly)
    {
        // TODO: Consider adding support for filtering by namespace or type name patterns, etc.

        ArgumentNullException.ThrowIfNull(assembly);

        if (assembly.Length == 0)
        {
            throw new ArgumentException("At least one assembly must be provided.", nameof(assembly));
        }

        services.Scan(scan => scan
            .FromAssemblies(assembly)
                .AddClasses(classes => classes.AssignableTo<INeedColumnMapping>())
                    .AsImplementedInterfaces()
                    .WithTransientLifetime());

        return services;
    }
}
