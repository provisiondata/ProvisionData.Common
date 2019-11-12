/*******************************************************************************
 * MIT License
 *
 * Copyright 2020 Provision Data Systems Inc.  https://provisiondata.com
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a 
 * copy of this software and associated documentation files (the "Software"),
 * to deal in the Software without restriction, including without limitation
 * the rights to use, copy, modify, merge, publish, distribute, sublicense,
 * and/or sell copies of the Software, and to permit persons to whom the 
 * Software is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
 * DEALINGS IN THE SOFTWARE.
 *
 *******************************************************************************/

namespace ProvisionData.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public static class TypeExtensions
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

        //public static IEnumerable<PropertyInfo> GetAllProperties(this Type type)
        //        => GetAll(type, ti => ti.DeclaredProperties);

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
            var sts = (from assembly in AppDomain.CurrentDomain.GetAssemblies().Where(a => a.FullName.StartsWith(assemblyPrefix, StringComparison.OrdinalIgnoreCase))
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
                type = typeInfo.BaseType;
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
}
