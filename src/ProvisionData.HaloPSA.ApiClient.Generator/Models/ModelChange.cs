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

namespace ProvisionData.HaloPSA.ModelGenerator.Models;

public record ModelChange
{
    public required String JsonModelName { get; set; }
    public String? ClientDtoName { get; set; }

    public String? JsonPropertyName { get; set; }
    public String? ClientPropertyName { get; set; }

    public String? JsonPropertyType { get; internal set; }
    public String? JsonFormat { get; internal set; }

    public String? ClientPropertyType { get; set; }

    public String? DefaultValue { get; set; }
    public Boolean Ignore { get; set; }
    public Boolean? Nullable { get; set; }
    public Boolean PrivateConstructor { get; set; }
    public Boolean Required { get; set; }
}
