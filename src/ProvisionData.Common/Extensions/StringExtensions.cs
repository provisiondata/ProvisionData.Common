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

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace ProvisionData.Extensions;

/// <summary>
/// Extension methods for <see cref="String"/> type.
/// </summary>
[DebuggerNonUserCode]
public static partial class StringExtensions
{
    /// <summary>
    /// Returns the leftmost <paramref name="length"/> characters from the input string.
    /// </summary>
    /// <param name="input">The input string. If null, returns null.</param>
    /// <param name="length">The number of characters to return from the left.</param>
    /// <returns>The leftmost <paramref name="length"/> characters, or the entire string if it is shorter than <paramref name="length"/>.</returns>
    [SuppressMessage("Style", "IDE0057:Convert to conditional expression", Justification = "Readability")]
    public static String Left(this String input, Int32 length)
    {
        if (input is null)
        {
            return input!;
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

    /// <summary>
    /// Formats the string using the specified arguments and the invariant culture.
    /// </summary>
    /// <param name="format">A composite format string.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    /// <returns>A copy of format in which the format items have been replaced by the string representation of the corresponding objects.</returns>
    internal static String Format(this String format, params Object[] args)
        => Format(format, CultureInfo.InvariantCulture, args);

    /// <summary>
    /// Formats the string using the specified arguments and format provider.
    /// </summary>
    /// <param name="format">A composite format string.</param>
    /// <param name="formatProvider">An object that supplies culture-specific formatting information.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    /// <returns>A copy of format in which the format items have been replaced by the string representation of the corresponding objects.</returns>
    internal static String Format(this String format, IFormatProvider formatProvider, params Object[] args)
        => String.Format(formatProvider, format, args);

    /// <summary>
    /// Ensures the string is enclosed in double quotes.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The input string enclosed in double quotes if not already quoted.</returns>
    public static String Quoted(this String input)
        => input.StartsWith('"') && input.EndsWith('"') ? input : $"\"{input}\"";

    /// <summary>
    /// Converts a string representation of a GUID to a <see cref="Guid"/>.
    /// </summary>
    /// <param name="input">A string representation of a GUID.</param>
    /// <returns>A <see cref="Guid"/> parsed from the input string.</returns>
    /// <exception cref="ArgumentException">Thrown when the input cannot be parsed as a GUID.</exception>
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
    /// Truncates the input string to the specified maximum length and appends an ellipsis (...).
    /// </summary>
    /// <param name="input">The string to be truncated.</param>
    /// <param name="maxLength">The maximum length of the returned string, including the ellipsis.</param>
    /// <returns>The truncated string with an ellipsis appended, or an empty string if maxLength is less than or equal to zero.</returns>
    public static String Truncate(this String input, Int32 maxLength)
        => String.IsNullOrEmpty(input) || maxLength <= 0
            ? String.Empty
            : input.Length > maxLength - Ellipsis.Length
                ? String.Concat(input.AsSpan(0, maxLength - Ellipsis.Length), Ellipsis)
                : input;

    /// <summary>
    /// Converts a pascal case or camel case string into proper case (space-separated).
    /// Abbreviations are returned unmodified.
    /// </summary>
    /// <param name="input">The input string in pascal or camel case.</param>
    /// <returns>A proper case version of the input string with spaces between words.</returns>
    /// <example>
    /// <code>
    /// "HelloWorld".ToProperCase() // Returns "Hello World"
    /// "BBC".ToProperCase()        // Returns "BBC"
    /// "IPAddress".ToProperCase()  // Returns "IP Address"
    /// </code>
    /// </example>
    public static String ToProperCase(this String input)
    {
        // If there are 0 or 1 characters, just return the string.
        if (input == null)
        {
            return input!;
        }

        if (input.Length < 2)
        {
            return input.ToUpper();
        }
        //return as is if the input is just an abbreviation
        if (Rgx.AllCaps().IsMatch(input))
        {
            return input;
        }

        // Start with the first character.
        var result = input[..1].ToUpper();

        // Add the remaining characters.
        for (var i = 1; i < input.Length; i++)
        {
            if (Char.IsUpper(input[i]) && i + 1 < input.Length && Char.IsLower(input[i + 1]))
            {
                result += " ";
            }

            result += input[i];
        }

        return result;
    }
}
