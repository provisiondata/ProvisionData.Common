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

using System.Text.Json.Serialization.Metadata;

namespace ProvisionData.ResultPattern.Infrastructure;

/// <summary>
/// Supports internal fuctionality and is not intended for public use.
/// </summary>
public static class ErrorJsonPolymorphism
{
    private static readonly List<Action<JsonTypeInfo>> Hooks = [];

    /// <summary>
    /// Supports internal fuctionality and is not intended for public use.
    /// </summary>
    public static void Register(Action<JsonTypeInfo> hook)
        => Hooks.Add(hook);

    /// <summary>
    /// Supports internal fuctionality and is not intended for public use.
    /// </summary>
    public static void Apply(JsonTypeInfo ti)
    {
        foreach (var hook in Hooks)
        {
            hook(ti);
        }
    }
}
