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

namespace ProvisionData.Dapper.Tests;

/// <summary>
/// Test entity for customers.
/// </summary>
public class Customer : INeedColumnMapping
{
    /// <summary>
    /// Gets or sets the customer ID.
    /// </summary>
    [ColumnMap("customer_id")]
    public Int32 Id { get; set; }

    /// <summary>
    /// Gets or sets the customer name.
    /// </summary>
    [ColumnMap("full_name")]
    public String Name { get; set; } = String.Empty;

    /// <summary>
    /// Gets or sets the customer email.
    /// </summary>
    [ColumnMap("email_address")]
    public String Email { get; set; } = String.Empty;
}

/// <summary>
/// Test entity for orders.
/// </summary>
public class Order : INeedColumnMapping
{
    /// <summary>
    /// Gets or sets the order ID.
    /// </summary>
    [ColumnMap("order_id")]
    public Int32 Id { get; set; }

    /// <summary>
    /// Gets or sets the customer ID.
    /// </summary>
    [ColumnMap("customer_id")]
    public Int32 CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the order total.
    /// </summary>
    [ColumnMap("order_total")]
    public Decimal Total { get; set; }
}
