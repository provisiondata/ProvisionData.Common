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

using ProvisionData.ResultPattern;

namespace ProvisionData.ResultPattern.CustomErrors.Tests;

/// <summary>
/// Example of a domain-specific error in a consuming application.
/// This demonstrates how the generator handles errors defined outside the core library.
/// </summary>
public sealed partial class OrderNotFoundError : Error
{
    /// <summary>
    /// The unique identifier of the order that was not found.
    /// </summary>
    public String OrderId { get; init; }
}

/// <summary>
/// Example of an error with multiple properties.
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

/// <summary>
/// Example of an error with no additional properties.
/// </summary>
public sealed partial class DatabaseConnectionError : Error
{
    // No additional properties - just uses the base description
}
