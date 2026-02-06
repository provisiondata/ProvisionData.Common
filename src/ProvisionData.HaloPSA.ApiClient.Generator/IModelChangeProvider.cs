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

using ProvisionData.HaloPSA.ModelGenerator.Models;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ProvisionData.HaloPSA.ModelGenerator;

public interface IModelChangeProvider
{
    String GetDtoName(String jsonModelName);
    ModelChange? GetChange([DisallowNull] String jsonModelName);
    ModelChange GetChange([DisallowNull] String jsonModelName, JsonProperty jsonProperty);

    /// <summary>
    /// Determines if a model type should be skipped during generation.
    /// A type is skipped if it has no corresponding entries in ModelChanges.
    /// Types are included only if they have at least one model-level or property-level change defined.
    /// </summary>
    /// <param name="jsonModelName">The JSON model name to check.</param>
    /// <returns>True if the type should be skipped, false if it should be included.</returns>
    Boolean ShouldSkip(String jsonModelName);
    Boolean ShouldSkip(ModelChange change) => ShouldSkip(change.JsonModelName);
}
