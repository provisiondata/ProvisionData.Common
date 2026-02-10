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

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("ProvisionData.Dapper.UnitTests")]

namespace ProvisionData.Dapper.Internals;

/// <summary>
/// This class exists to test MapColumnsFromExecutingAssembly() functionality, ensuring that
/// column mappings are correctly applied when mapping from the executing assembly.
/// </summary>
[HasColumnMaps]
internal class ProductEx
{
    /// <summary>
    /// Gets or sets the product ID.
    /// </summary>
    [ColumnName("id")]
    public Int32 Id { get; set; }

    /// <summary>
    /// Gets or sets the product name, mapped from 'product_name' column.
    /// </summary>
    [ColumnName("product_name")]
    public String Name { get; set; } = String.Empty;

    /// <summary>
    /// Gets or sets the product price, mapped from 'unit_price' column.
    /// </summary>
    [ColumnName("unit_price")]
    public Decimal Price { get; set; }

    /// <summary>
    /// Gets or sets the quantity in stock, mapped from 'quantity_in_stock' column.
    /// </summary>
    [ColumnName("quantity_in_stock")]
    public Int32 Stock { get; set; }
}
