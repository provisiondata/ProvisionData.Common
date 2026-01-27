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
using System.Text.RegularExpressions;

namespace ProvisionData.Extensions;

[DebuggerNonUserCode]
public static partial class StringExtensions
{
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

    internal static String Format(this String format, params Object[] args)
        => Format(format, CultureInfo.InvariantCulture, args);

    internal static String Format(this String format, IFormatProvider formatProvider, params Object[] args)
        => String.Format(formatProvider, format, args);

    public static String Quoted(this String input)
        => input.StartsWith('"') && input.EndsWith('"') ? input : $"\"{input}\"";

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
                ? String.Concat(input.AsSpan(0, maxLength - Ellipsis.Length), Ellipsis)
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
        if (input == null)
        {
            return input!;
        }

        if (input.Length < 2)
        {
            return input.ToUpper();
        }
        //return as is if the input is just an abbreviation
        if (AllCaps().IsMatch(input))
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

    [GeneratedRegex("[0-9A-Z]+$", RegexOptions.Compiled)]
    private static partial Regex AllCaps();
}
