// Provision Data Application Framework
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

namespace ProvisionData.ResultPattern;

/// <summary>
/// Example of a dynamically generated custom error with multiple properties.
/// </summary>
public sealed partial class InventoryInsufficientError : Error
{
    /// <summary>
    /// The product SKU that has insufficient inventory.
    /// </summary>
    public String ProductSku { get; init; }

    /// <summary>
    /// The requested quantity.
    /// </summary>
    public Int32 RequestedQuantity { get; init; }

    /// <summary>
    /// The available quantity in inventory.
    /// </summary>
    public Int32 AvailableQuantity { get; init; }
}

