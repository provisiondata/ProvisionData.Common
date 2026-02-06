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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProvisionData.Data;
using ProvisionData.Testing;

namespace ProvisionData.Dapper.Tests;

public class ColumnMappingTestFixture : IntegrationTestFixture
{
    protected override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IDbConnectionFactory, TestSqliteConnectionFactory>();
    }

    /// <summary>
    /// Initializes the test by creating an in-memory SQLite database.
    /// </summary>
    protected override async ValueTask InitializeFixtureAsync(IServiceProvider services)
    {
        var connection = await services.GetRequiredService<IDbConnectionFactory>().CreateConnectionAsync();

        ColumnMapper.MapTypesFromAssemblyContaining<Customer>();

        // Create test customers table
        await connection.ExecuteAsync("""
            CREATE TABLE customers (
                customer_id INTEGER PRIMARY KEY,
                full_name TEXT NOT NULL,
                email_address TEXT NOT NULL
            )
            """);

        // Create order table
        await connection.ExecuteAsync("""
            CREATE TABLE orders (
                order_id INTEGER PRIMARY KEY,
                customer_id INTEGER NOT NULL,
                order_total REAL NOT NULL
            )
            """);

        // Insert test data for both tables
        await connection.ExecuteAsync("""
            INSERT INTO customers (customer_id, full_name, email_address)
            VALUES (@Id, @Name, @Email)
            """, new { Id = 1, Name = "Test Customer", Email = "test@example.com" });

        await connection.ExecuteAsync("""
            INSERT INTO orders (order_id, customer_id, order_total)
            VALUES (@Id, @CustomerId, @Total)
            """, new { Id = 1, CustomerId = 1, Total = 99.99m });
    }
}
