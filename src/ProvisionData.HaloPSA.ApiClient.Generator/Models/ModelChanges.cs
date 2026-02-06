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

namespace ProvisionData.HaloPSA.ModelGenerator.Models;

public class ModelChanges
{
    private const String UnknownType = "UnknownTypeDto";

    public static String SectionName { get; set; } = nameof(ModelChanges);

    public String UnknownTypeName { get; set; } = UnknownType;

    public ModelChange[] Changes { get; set; } = [];

    public String[] IgnoredJsonPropertyNames { get; set; } = [];
    public String[] IgnoredJsonPropertyTypes { get; set; } = [];

    public Boolean InitializeArrays { get; set; }
}
