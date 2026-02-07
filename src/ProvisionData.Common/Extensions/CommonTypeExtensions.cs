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

using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace ProvisionData.Extensions;

/// <summary>
/// 
/// </summary>
public static class CommonTypeExtensions
{
    /// <summary>
    /// Validates that a GUID is not empty; throws an exception if it is.
    /// </summary>
    /// <param name="g">The GUID to validate.</param>
    /// <param name="parameter">The name of the parameter, used in the exception message.</param>
    /// <returns>The original GUID if validation succeeds.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="g"/> is <see cref="Guid.Empty"/>.</exception>
    public static Guid ThrowIfEmpty(this Guid g, String parameter)
    {
        if (g == Guid.Empty)
        {
            throw new ArgumentException("Must not be Guid.Empty", parameter);
        }

        return g;
    }

    /// <summary>
    /// Gets all constructors from a type and its base types.
    /// </summary>
    /// <param name="type">The type to inspect.</param>
    /// <returns>An enumerable of all constructors in the type and its base types.</returns>
    [SuppressMessage("Trimming", "IL2072:Target parameter argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method. The return value of the source method does not have matching annotations.", Justification = "Method intentionally uses reflection to discover all constructors. Callers must ensure the type parameter has appropriate DynamicallyAccessedMembers annotations.")]
    public static IEnumerable<ConstructorInfo> GetAllConstructors(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)]
        this Type type)
    {
        while (type != null)
        {
            var typeInfo = type.GetTypeInfo();
            foreach (var ctor in typeInfo.DeclaredConstructors)
            {
                yield return ctor;
            }

            type = typeInfo.BaseType!;
        }
    }

    /// <summary>
    /// Gets all events from a type and its base types.
    /// </summary>
    /// <param name="type">The type to inspect.</param>
    /// <returns>An enumerable of all events in the type and its base types.</returns>
    [SuppressMessage("Trimming", "IL2072:Target parameter argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method. The return value of the source method does not have matching annotations.", Justification = "Method intentionally uses reflection to discover all events. Callers must ensure the type parameter has appropriate DynamicallyAccessedMembers annotations.")]
    public static IEnumerable<EventInfo> GetAllEvents(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicEvents | DynamicallyAccessedMemberTypes.NonPublicEvents)]
        this Type type)
    {
        while (type != null)
        {
            var typeInfo = type.GetTypeInfo();
            foreach (var evt in typeInfo.DeclaredEvents)
            {
                yield return evt;
            }

            type = typeInfo.BaseType!;
        }
    }

    /// <summary>
    /// Gets all fields from a type and its base types.
    /// </summary>
    /// <param name="type">The type to inspect.</param>
    /// <returns>An enumerable of all fields in the type and its base types.</returns>
    [SuppressMessage("Trimming", "IL2072:Target parameter argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method. The return value of the source method does not have matching annotations.", Justification = "Method intentionally uses reflection to discover all fields. Callers must ensure the type parameter has appropriate DynamicallyAccessedMembers annotations.")]
    public static IEnumerable<FieldInfo> GetAllFields(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.NonPublicFields)]
        this Type type)
    {
        while (type != null)
        {
            var typeInfo = type.GetTypeInfo();
            foreach (var field in typeInfo.DeclaredFields)
            {
                yield return field;
            }

            type = typeInfo.BaseType!;
        }
    }

    /// <summary>
    /// Gets all members from a type and its base types.
    /// </summary>
    /// <param name="type">The type to inspect.</param>
    /// <returns>An enumerable of all members in the type and its base types.</returns>
    public static IEnumerable<MemberInfo> GetAllMembers(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
        this Type type)
    {
        while (type != null)
        {
            var typeInfo = type.GetTypeInfo();
            foreach (var member in typeInfo.DeclaredMembers)
            {
                yield return member;
            }

            type = typeInfo.BaseType!;
        }
    }

    /// <summary>
    /// Gets all methods from a type and its base types.
    /// </summary>
    /// <param name="type">The type to inspect.</param>
    /// <returns>An enumerable of all methods in the type and its base types.</returns>
    [SuppressMessage("Trimming", "IL2072:Target parameter argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method. The return value of the source method does not have matching annotations.", Justification = "Method intentionally uses reflection to discover all methods. Callers must ensure the type parameter has appropriate DynamicallyAccessedMembers annotations.")]
    public static IEnumerable<MethodInfo> GetAllMethods(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.NonPublicMethods)]
        this Type type)
    {
        while (type != null)
        {
            var typeInfo = type.GetTypeInfo();
            foreach (var method in typeInfo.DeclaredMethods)
            {
                yield return method;
            }

            type = typeInfo.BaseType!;
        }
    }

    /// <summary>
    /// Gets all nested types from a type and its base types.
    /// </summary>
    /// <param name="type">The type to inspect.</param>
    /// <returns>An enumerable of all nested types in the type and its base types.</returns>
    [SuppressMessage("Trimming", "IL2072:Target parameter argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method. The return value of the source method does not have matching annotations.", Justification = "Method intentionally uses reflection to discover all nested types. Callers must ensure the type parameter has appropriate DynamicallyAccessedMembers annotations.")]
    public static IEnumerable<TypeInfo> GetAllNestedTypes(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicNestedTypes | DynamicallyAccessedMemberTypes.NonPublicNestedTypes)]
        this Type type)
    {
        while (type != null)
        {
            var typeInfo = type.GetTypeInfo();
            foreach (var nestedType in typeInfo.DeclaredNestedTypes)
            {
                yield return nestedType;
            }

            type = typeInfo.BaseType!;
        }
    }

    /// <summary>
    /// Gets all types implementing an open generic type from the specified assemblies.
    /// </summary>
    /// <param name="assemblies">The assemblies to search.</param>
    /// <param name="openGenericType">The open generic type to match.</param>
    /// <returns>An enumerable of types that implement the specified open generic type.</returns>
    public static IEnumerable<Type> GetAllTypesImplementingOpenGenericType(this IEnumerable<Assembly> assemblies, Type openGenericType) => GetAllTypesImplementingOpenGenericType(assemblies, openGenericType, _ => true);

    /// <summary>
    /// Gets all types implementing an open generic type from the specified assemblies, filtered by a predicate.
    /// </summary>
    /// <param name="assemblies">The assemblies to search.</param>
    /// <param name="openGenericType">The open generic type to match.</param>
    /// <param name="predicate">A predicate to filter assemblies.</param>
    /// <returns>An enumerable of types that implement the specified open generic type and match the predicate.</returns>
    [SuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Method performs intentional runtime type discovery across assemblies using reflection. This is a utility method for scanning loaded assemblies and cannot be statically analyzed by the trimmer.")]
    [SuppressMessage("Trimming", "IL2075:'this' argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method. The return value of the source method does not have matching annotations.", Justification = "Method intentionally performs type discovery on all available types across assemblies. The reflection APIs are used with appropriate runtime safeguards.")]
    public static IEnumerable<Type> GetAllTypesImplementingOpenGenericType(this IEnumerable<Assembly> assemblies, Type openGenericType, Predicate<Assembly> predicate)
    {
        return from assembly in assemblies
               from type in assembly.GetTypes()
               from interfaces in type.GetTypeInfo().GetInterfaces()
               let baseType = type.GetTypeInfo()
               where predicate(assembly)
               where (baseType?.IsGenericType == true && openGenericType.GetTypeInfo().IsAssignableFrom(baseType.GetGenericTypeDefinition()))
                  || (interfaces.GetTypeInfo().IsGenericType && openGenericType.GetTypeInfo().IsAssignableFrom(interfaces.GetGenericTypeDefinition()))
               group type by type into g
               select g.Key;
    }

    /// <summary>
    /// Gets all subtypes of a specified type from assemblies with a specific prefix.
    /// </summary>
    /// <typeparam name="T">The base type to find subtypes of.</typeparam>
    /// <param name="assemblyPrefix">The prefix to filter assemblies by. Defaults to "PDSI".</param>
    /// <returns>An array of types that are assignable to <typeparamref name="T"/>.</returns>
    [SuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Method performs intentional runtime assembly scanning to discover subtypes. This is a utility method that cannot be statically analyzed by the trimmer and requires all candidate types to be preserved at runtime.")]
    public static Type[] GetSubTypes<T>(String assemblyPrefix = "PDSI")
    {
        var t = typeof(T);
        var sts = (from assembly in AppDomain.CurrentDomain
                    .GetAssemblies()
                    .Where(a => a.FullName?.StartsWith(assemblyPrefix, StringComparison.OrdinalIgnoreCase) == true)
                   from type in assembly.GetExportedTypes()
                   where t.IsAssignableFrom(type)
                   select type).ToArray();
        return sts;
    }

    /// <summary>
    /// Returns all properties on the given type, going up the inheritance hierarchy.
    /// </summary>
    /// <param name="type">The type to inspect.</param>
    /// <returns>An enumerable of properties from the type and its base types and interfaces, with duplicates removed.</returns>
    [SuppressMessage("Trimming", "IL2072:Target parameter argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method. The return value of the source method does not have matching annotations.", Justification = "Method intentionally uses reflection to discover all properties including from interfaces. Callers must ensure the type parameter has appropriate DynamicallyAccessedMembers annotations.")]
    public static IEnumerable<PropertyInfo> GetAllProperties([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties | DynamicallyAccessedMemberTypes.Interfaces)] this Type type)
    {
        var props = new List<PropertyInfo>(type.GetProperties());
        foreach (var interfaceType in type.GetInterfaces())
        {
            props.AddRange(GetAllProperties(interfaceType));
        }

        var tracked = new List<PropertyInfo>(props.Count);
        var duplicates = new List<PropertyInfo>(props.Count);
        foreach (var p in props)
        {
            var duplicate = tracked.SingleOrDefault(n => n.Name == p.Name && n.PropertyType == p.PropertyType);
            if (duplicate != null)
            {
                duplicates.Add(p);
            }
            else
            {
                tracked.Add(p);
            }
        }

        foreach (var d in duplicates)
        {
            props.Remove(d);
        }

        return props;
    }
}
