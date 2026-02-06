// Provision Data HaloPSA API Client
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

using Humanizer;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;

namespace ProvisionData.HaloPSA.Generator;

public static partial class TypeHelper
{
    [return: NotNullIfNotNull(nameof(name))]
    public static String? ToPascalCase(this String? name)
    {
        if (String.IsNullOrWhiteSpace(name))
        {
            return name;
        }

        if (IsKnownType(name))
        {
            return name;
        }

        var parts = name.Split(['_', '-', '.', ' ', '.'], StringSplitOptions.RemoveEmptyEntries);
        var sb = new StringBuilder();
        var first = true;
        foreach (var part in parts)
        {
            if (part.Length == 0)
            {
                continue;
            }

            var p = part;

            var match = StartsWithANumber().Match(p);
            if (first && match.Success && Int32.TryParse(match.Groups[1].Value, out var value))
            {
                p = p.Replace(value.ToString(), value.ToWords());
            }

            sb.Append(Char.ToUpperInvariant(p[0]));
            if (p.Length > 1)
            {
                sb.Append(p.AsSpan(1));
            }

            first = false;
        }

        return sb.ToString();
    }

    public static Boolean IsKnownType(this String? typeName)
    {
        if (typeName is "String")
            return true;

        return IsValueType(typeName);
    }

    public static Boolean IsValueType(this String? typeName)
    {
        return typeName is "Int32" or "Int64" or "Single" or "Double" or "Decimal" or "Boolean" or "DateTime" or "DateTimeOffset";
    }

    [GeneratedRegex("^([0-9]+)[a-zA-Z][a-zA-Z0-9_]+$")]
    private static partial Regex StartsWithANumber();

}
