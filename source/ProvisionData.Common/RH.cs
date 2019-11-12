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

namespace ProvisionData
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;

    /// <summary>ResourceHelper</summary>
    [DebuggerStepThrough]
    public static class RH
    {
        private static readonly IDictionary<String, String> Cache = new Dictionary<String, String>();

        public static String GS(String resource)
        {
            var type = typeof(RH);
            return GS(resource, type);
        }

        /// <summary>
        /// Looks in the assembly and namespace that contains <typeparamref name="T"/> for a resource named <paramref name="resource"/> and returns it as a <seealso cref="String"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resource"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static String GS<T>(String resource)
        {
            var type = typeof(T);
            return GS(resource, type);
        }

        public static String GS(String resource, Type type)
        {
            var cachekey = type.FullName + "::" + resource;
            if (!Cache.ContainsKey(cachekey))
            {
                try
                {
                    var assembly = type.GetTypeInfo().Assembly;
                    if (assembly == null)
                    {
                        throw new Exception("Could not load the assembly.");
                    }

                    var key = type.Namespace + "." + resource;

                    // ReSharper disable once AssignNullToNotNullAttribute
                    using (var reader = new StreamReader(assembly.GetManifestResourceStream(key)))
                    {
                        Cache[cachekey] = reader.ReadToEnd();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("ResourceHelper.GetString('" + resource + "') failed to load the resource code from " + type.AssemblyQualifiedName, ex);
                }
            }

            return Cache[cachekey];
        }

        [DebuggerStepThrough]
        public static Stream GRS(String resource)
        {
            var type = typeof(RH);
            return GRS(resource, type);
        }

        [DebuggerStepThrough]
        public static Stream GRS<T>(String resource)
        {
            var type = typeof(T);
            return GRS(resource, type);
        }

        [DebuggerStepThrough]
        public static Stream GRS(String resource, Type type)
        {
            var assembly = type.GetTypeInfo().Assembly;

            if (assembly == null)
            {
                throw new Exception("Could not load the assembly.");
            }

            var key = type.Namespace + "." + resource;

            return assembly.GetManifestResourceStream(key);
        }
    }
}