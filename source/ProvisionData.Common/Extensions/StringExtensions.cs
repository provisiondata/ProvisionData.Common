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
    using System.Diagnostics;
    using System.Globalization;

    [DebuggerNonUserCode]
    public static class StringExtensions
    {
        internal static String Format(this String format, params Object[] args)
            => Format(format, CultureInfo.InvariantCulture, args);

        internal static String Format(this String format, IFormatProvider formatProvider, params Object[] args)
            => String.Format(formatProvider, format, args);

        public static String Quoted(this String value)
        {
            return value.StartsWith("\"", StringComparison.Ordinal) && value.EndsWith("\"", StringComparison.Ordinal) ? value : "\"" + value + "\"";
        }

        public static Guid ToGuid(this String value)
        {
            if (Guid.TryParse(value, out var guid))
            {
                return guid;
            }

            throw new ArgumentException($"Don't know how to parse '{value}' into a GUID.");
        }
    }
}
