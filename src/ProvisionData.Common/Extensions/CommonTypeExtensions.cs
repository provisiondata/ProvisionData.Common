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

using System.Reflection;

namespace ProvisionData.Extensions;

public static class CommonTypeExtensions
{
    public static Guid ThrowIfEmpty(this Guid g, String parameter)
    {
        if (g == Guid.Empty)
        {
            throw new ArgumentException("Must not be Guid.Empty", parameter);
        }

        return g;
    }

    public static IEnumerable<ConstructorInfo> GetAllConstructors(this Type type)
            => GetAll(type, ti => ti.DeclaredConstructors);

    public static IEnumerable<EventInfo> GetAllEvents(this Type type)
            => GetAll(type, ti => ti.DeclaredEvents);

    public static IEnumerable<FieldInfo> GetAllFields(this Type type)
            => GetAll(type, ti => ti.DeclaredFields);

    public static IEnumerable<MemberInfo> GetAllMembers(this Type type)
            => GetAll(type, ti => ti.DeclaredMembers);

    public static IEnumerable<MethodInfo> GetAllMethods(this Type type)
            => GetAll(type, ti => ti.DeclaredMethods);

    public static IEnumerable<TypeInfo> GetAllNestedTypes(this Type type)
            => GetAll(type, ti => ti.DeclaredNestedTypes);

    public static IEnumerable<Type> GetAllTypesImplementingOpenGenericType(this IEnumerable<Assembly> assemblies, Type openGenericType) => GetAllTypesImplementingOpenGenericType(assemblies, openGenericType, _ => true);

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

    private static IEnumerable<T> GetAll<T>(Type type, Func<TypeInfo, IEnumerable<T>> accessor)
    {
        while (type != null)
        {
            var typeInfo = type.GetTypeInfo();
            foreach (var ti in accessor(typeInfo))
            {
                yield return ti;
            }

            type = typeInfo.BaseType!;
        }
    }

    /// <summary>
    /// Returns all properties on the given type, going up the inheritance hierarchy.
    /// </summary>
    public static IEnumerable<PropertyInfo> GetAllProperties(this Type type)
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
