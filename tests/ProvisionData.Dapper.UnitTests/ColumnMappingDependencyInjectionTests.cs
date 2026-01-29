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

using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;

namespace ProvisionData.Dapper.Tests;

/// <summary>
/// Tests the dependency injection integration for column mapping using <see cref="INeedColumnMapping"/>.
/// </summary>
public class ColumnMappingDependencyInjectionTests : IAsyncLifetime
{
    private SqliteConnection _connection = null!;

    /// <summary>
    /// Initializes the test by creating an in-memory SQLite database.
    /// </summary>
    public async ValueTask InitializeAsync()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        await _connection.OpenAsync();

        await _connection.ExecuteAsync("""
            CREATE TABLE customers (
                customer_id INTEGER PRIMARY KEY,
                full_name TEXT NOT NULL,
                email_address TEXT NOT NULL
            )
            """);
    }

    /// <summary>
    /// Disposes of the database connection.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (_connection != null)
        {
            await _connection.CloseAsync();
            _connection.Dispose();
        }

        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Tests that column mappings registered in the dependency injection container are correctly applied.
    /// </summary>
    [Fact]
    public async Task MapColumns_WithServiceProvider_ShouldApplyAllRegisteredMappings()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddTransient<INeedColumnMapping, Customer>();
        var serviceProvider = services.BuildServiceProvider();

        // Act - Apply all registered mappings
        serviceProvider.MapColumns();

        // Insert test data
        await _connection.ExecuteAsync("""
            INSERT INTO customers (customer_id, full_name, email_address)
            VALUES (@Id, @Name, @Email)
            """, new { Id = 1, Name = "John Doe", Email = "john@example.com" });

        // Query with mapping
        var customer = await _connection.QueryFirstOrDefaultAsync<Customer>(
            "SELECT customer_id, full_name, email_address FROM customers"
        );

        // Assert
        Assert.NotNull(customer);
        Assert.Equal(1, customer.Id);
        Assert.Equal("John Doe", customer.Name);
        Assert.Equal("john@example.com", customer.Email);
    }

    /// <summary>
    /// Tests that multiple column mappings are correctly applied when registered in the dependency injection container.
    /// </summary>
    [Fact]
    public async Task MapColumns_WithMultipleMappings_ShouldApplyAllMappings()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddTransient<INeedColumnMapping, Customer>();
        services.AddTransient<INeedColumnMapping, Order>();
        var serviceProvider = services.BuildServiceProvider();

        // Act
        serviceProvider.MapColumns();

        // Create order table
        await _connection.ExecuteAsync("""
            CREATE TABLE orders (
                order_id INTEGER PRIMARY KEY,
                customer_id INTEGER NOT NULL,
                order_total REAL NOT NULL
            )
            """);

        // Insert test data for both tables
        await _connection.ExecuteAsync("""
            INSERT INTO customers (customer_id, full_name, email_address)
            VALUES (@Id, @Name, @Email)
            """, new { Id = 1, Name = "Test Customer", Email = "test@example.com" });

        await _connection.ExecuteAsync("""
            INSERT INTO orders (order_id, customer_id, order_total)
            VALUES (@Id, @CustomerId, @Total)
            """, new { Id = 1, CustomerId = 1, Total = 99.99m });

        // Query both types with mappings
        var customer = await _connection.QueryFirstOrDefaultAsync<Customer>(
            "SELECT customer_id, full_name, email_address FROM customers"
        );

        var order = await _connection.QueryFirstOrDefaultAsync<Order>(
            "SELECT order_id, customer_id, order_total FROM orders"
        );

        // Assert
        Assert.NotNull(customer);
        Assert.Equal("Test Customer", customer.Name);
        Assert.NotNull(order);
        Assert.Equal(99.99m, order.Total);
    }

    /// <summary>
    /// Test entity for customers.
    /// </summary>
    private class Customer : INeedColumnMapping
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
    private class Order : INeedColumnMapping
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

    ///// <summary>
    ///// Implementation of <see cref="INeedColumnMapping"/> for the Customer type.
    ///// </summary>
    //private class CustomerColumnMapping : INeedColumnMapping
    //{
    //    /// <summary>
    //    /// Applies the column mapping for the Customer type.
    //    /// </summary>
    //    public void ApplyMap()
    //    {
    //        ColumnMapping.ApplyMap<Customer>();
    //    }
    //}

    ///// <summary>
    ///// Implementation of <see cref="INeedColumnMapping"/> for the Order type.
    ///// </summary>
    //private class OrderColumnMapping : INeedColumnMapping
    //{
    //    /// <summary>
    //    /// Applies the column mapping for the Order type.
    //    /// </summary>
    //    public void ApplyMap()
    //    {
    //        ColumnMapping.ApplyMap<Order>();
    //    }
    //}
}
