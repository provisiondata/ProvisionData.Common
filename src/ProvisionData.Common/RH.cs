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

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;

namespace ProvisionData;

/// <summary>Resource helper for loading embedded resources from assemblies.</summary>
[DebuggerStepThrough]
public static class RH
{
    private static readonly ConcurrentDictionary<String, String> Cache = new();

    /// <summary>
    /// Looks in the assembly and namespace that contains <typeparamref name="T"/> for a resource named <paramref name="resource"/> and returns it as a <see cref="String"/>.
    /// </summary>
    /// <typeparam name="T">The type used to locate the assembly and namespace for the resource.</typeparam>
    /// <param name="resource">The case-sensitive name of the manifest resource being requested.</param>
    /// <returns>The manifest resource as a string; or throws if not found.</returns>
    /// <remarks>
    /// Strings returned by this method are cached in memory the first time they are accessed.
    /// Subsequent requests for the same resource are returned directly from the cache.
    /// </remarks>
    [DebuggerStepThrough]
    public static String GetString<T>(String resource)
    {
        var type = typeof(T);
        return GetString(resource, type);
    }

    /// <summary>
    /// Looks in the assembly and namespace that contains <paramref name="type"/> for an embedded 
    /// resource named <paramref name="resource"/> and returns it as a <see cref="String"/>, if found.
    /// </summary>
    /// <param name="resource">The case-sensitive name of the manifest resource being requested.</param>
    /// <param name="type">The type used to locate the assembly and namespace for the resource.</param>
    /// <returns>The manifest resource as a string; or throws if not found.</returns>
    /// <remarks>
    /// Strings returned by this method are cached in memory the first time they are accessed.
    /// Subsequent requests for the same resource are returned directly from the cache.
    /// </remarks>
    [DebuggerStepThrough]
    public static String GetString(String resource, Type type)
    {
        var cachekey = type.AssemblyQualifiedName + "::" + resource;
        return Cache.GetOrAdd(cachekey, k =>
        {
            try
            {
                var assembly = type.GetTypeInfo().Assembly
                ?? throw new Exception("Could not load the assembly.");

                var key = type.Namespace + "." + resource;

                using var stream = assembly.GetManifestResourceStream(key)
                    ?? throw new Exception($"Failed to load the resource '{resource}' from {type.AssemblyQualifiedName}.");

                using var reader = new StreamReader(stream);
                return reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to load the resource '{resource}' from {type.AssemblyQualifiedName}.", ex);
            }
        });
    }

    /// <summary>
    /// Loads the specified manifest resource, scoped by the namespace of the specified type, from the assembly containing the type as a byte array.
    /// </summary>
    /// <typeparam name="T">The type used to locate the assembly and namespace for the resource.</typeparam>
    /// <param name="resource">The case-sensitive name of the manifest resource being requested.</param>
    /// <returns>A byte array containing the manifest resource; or throws if not found.</returns>
    [DebuggerStepThrough]
    public static Byte[] GetBytes<T>(String resource) => GetBytes(resource, typeof(T));

    /// <summary>
    /// Loads the specified manifest resource, scoped by the namespace of the specified type, from the assembly containing the type as a byte array.
    /// </summary>
    /// <param name="resource">The case-sensitive name of the manifest resource being requested.</param>
    /// <param name="type">The type used to locate the assembly and namespace for the resource.</param>
    /// <returns>A byte array containing the manifest resource; or throws if not found.</returns>
    [DebuggerStepThrough]
    public static Byte[] GetBytes(String resource, Type type)
    {
        using (var stream = GetStream(resource, type))
        {
            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }

    /// <summary>
    /// Loads the specified manifest resource, scoped by the namespace of the specified type, from the assembly containing the type as a stream.
    /// </summary>
    /// <typeparam name="T">The type used to locate the assembly and namespace for the resource.</typeparam>
    /// <param name="resource">The case-sensitive name of the manifest resource being requested.</param>
    /// <returns>A stream containing the manifest resource; or throws if not found. The caller is responsible for disposing the stream.</returns>
    [DebuggerStepThrough]
    public static Stream GetStream<T>(String resource)
    {
        var type = typeof(T);
        return GetStream(resource, type);
    }

    /// <summary>
    /// Loads the specified manifest resource, scoped by the namespace of the specified type, from the assembly containing the type as a stream.
    /// </summary>
    /// <param name="resource">The case-sensitive name of the manifest resource being requested.</param>
    /// <param name="type">The type used to locate the assembly and namespace for the resource.</param>
    /// <returns>A stream containing the manifest resource; or throws if not found. The caller is responsible for disposing the stream.</returns>
    [DebuggerStepThrough]
    public static Stream GetStream(String resource, Type type)
    {
        if (String.IsNullOrWhiteSpace(resource))
        {
            throw new ArgumentException($"'{nameof(resource)}' cannot be null or whitespace.", nameof(resource));
        }

        var assembly = type.GetTypeInfo().Assembly
            ?? throw new InvalidOperationException("Could not load the assembly.");

        var key = type.Namespace + "." + resource;
        try
        {
            return assembly.GetManifestResourceStream(key)
                ?? throw new Exception($"Failed to load the resource '{resource}' from {type.AssemblyQualifiedName}.");
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to load the resource '{resource}' from {type.AssemblyQualifiedName}.", ex);
        }
    }
}
