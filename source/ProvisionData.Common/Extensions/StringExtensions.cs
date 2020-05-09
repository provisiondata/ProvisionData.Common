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
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Text.RegularExpressions;

    [DebuggerNonUserCode]
    public static class StringExtensions
    {
        [SuppressMessage("Style", "IDE0046:Convert to conditional expression", Justification = "Readability")]
        public static String Left(this String input, Int32 length)
        {
            if (input is null)
            {
                return null;
            }
            else if (String.IsNullOrWhiteSpace(input))
            {
                return String.Empty;
            }
            else
            {
                return input.Substring(0, input.Length < length ? input.Length : length);
            }
        }

        internal static String Format(this String format, params Object[] args)
            => Format(format, CultureInfo.InvariantCulture, args);

        internal static String Format(this String format, IFormatProvider formatProvider, params Object[] args)
            => String.Format(formatProvider, format, args);

        public static String Quoted(this String input)
            => input.StartsWith("\"", StringComparison.Ordinal) && input.EndsWith("\"", StringComparison.Ordinal) ? input : "\"" + input + "\"";

        public static Guid ToGuid(this String input)
        {
            if (Guid.TryParse(input, out var guid))
            {
                return guid;
            }

            throw new ArgumentException($"Don't know how to parse '{input}' into a GUID.");
        }

        private const String Ellipsis = " ...";
        /// <summary>
        /// Truncates <paramref name="input"/> to <paramref name="maxLength"/> and append an ellipsis (...) to the end.
        /// </summary>
        /// <param name="input">value to be truncated</param>
        /// <param name="maxLength">length to return including the ellipsis (...)</param>
        // ToDo: Make it so it doesn't truncate in the middle of a word. -dw
        public static String Truncate(this String input, Int32 maxLength)
            => String.IsNullOrEmpty(input) || maxLength <= 0
                ? String.Empty
                : input.Length > maxLength - Ellipsis.Length
                    ? input.Substring(0, maxLength - Ellipsis.Length) + Ellipsis
                    : input;

        /// <summary>
        /// Turn a pascal case or camel case string into proper case.
        /// If the input is an abbreviation, the input is return unmodified.
        /// </summary>
        /// <param name="input"></param>
        /// <example>
        /// input : HelloWorld
        /// output : Hello World
        /// </example>
        /// <example>
        /// input : BBC
        /// output : BBC
        /// </example>
        /// <example>
        /// input : IPAddress
        /// output : IP Address
        /// </example>
        public static String ToProperCase(this String input)
        {
            // If there are 0 or 1 characters, just return the string.
            if (input == null) return input;
            if (input.Length < 2) return input.ToUpper();
            //return as is if the input is just an abbreviation
            if (AllCaps.IsMatch(input)) return input;

            // Start with the first character.
            var result = input.Substring(0, 1).ToUpper();

            // Add the remaining characters.
            for (var i = 1; i < input.Length; i++)
            {
                if (Char.IsUpper(input[i]) && i + 1 < input.Length && Char.IsLower(input[i + 1])) result += " ";
                result += input[i];
            }

            return result;
        }
        private static readonly Regex AllCaps = new Regex("[0-9A-Z]+$", RegexOptions.Compiled);
    }
}
