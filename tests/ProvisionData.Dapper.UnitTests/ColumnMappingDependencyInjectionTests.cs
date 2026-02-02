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
using Microsoft.Extensions.DependencyInjection;
using ProvisionData.Data;
using ProvisionData.Testing;

namespace ProvisionData.Dapper.Tests;

/// <summary>
/// Tests the dependency injection integration for column mapping using <see cref="INeedColumnMapping"/>.
/// </summary>
public class ColumnMappingDependencyInjectionTests(ColumnMappingTestFixture fixture, ITestOutputHelper output)
    : IntegrationTestBase<ColumnMappingTestFixture>(fixture, output)
{
    /// <summary>
    /// Tests that column mappings registered in the dependency injection container are correctly applied.
    /// </summary>
    [Fact]
    public async Task MapColumns_WithServiceProvider_ShouldApplyAllRegisteredMappings()
    {
        // Arrange
        var connection = await Services.GetRequiredService<IDbConnectionFactory>().CreateConnectionAsync();

        // Act - Apply all registered mappings
        var customer = await connection.QueryFirstOrDefaultAsync<Customer>(
            "SELECT customer_id, full_name, email_address FROM customers"
        );

        // Assert
        Assert.NotNull(customer);
        Assert.Equal(1, customer.Id);
        Assert.Equal("Test Customer", customer.Name);
        Assert.Equal("test@example.com", customer.Email);
    }

    /// <summary>
    /// Tests that multiple column mappings are correctly applied when registered in the dependency injection container.
    /// </summary>
    [Fact]
    public async Task MapColumns_WithMultipleMappings_ShouldApplyAllMappings()
    {
        // Arrange
        var connection = await Services.GetRequiredService<IDbConnectionFactory>().CreateConnectionAsync();

        // Query both types with mappings
        var customer = await connection.QueryFirstOrDefaultAsync<Customer>(
            "SELECT customer_id, full_name, email_address FROM customers"
        );

        var order = await connection.QueryFirstOrDefaultAsync<Order>(
            "SELECT order_id, customer_id, order_total FROM orders"
        );

        // Assert
        Assert.NotNull(customer);
        Assert.Equal("Test Customer", customer.Name);
        Assert.Equal("test@example.com", customer.Email);
        Assert.NotNull(order);
        Assert.Equal(99.99m, order.Total);
    }
}
