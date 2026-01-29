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

namespace ProvisionData.Dapper.Tests;

/// <summary>
/// Tests the <see cref="ColumnMapping"/> functionality using an in-memory SQLite database.
/// </summary>
public class ColumnMappingTests : IAsyncLifetime
{
    private SqliteConnection _connection = null!;

    /// <summary>
    /// Initializes the test by creating an in-memory SQLite database.
    /// </summary>
    public async ValueTask InitializeAsync()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        await _connection.OpenAsync();

        // Create test table
        await _connection.ExecuteAsync("""
            CREATE TABLE products (
                id INTEGER PRIMARY KEY,
                product_name TEXT NOT NULL,
                unit_price REAL NOT NULL,
                quantity_in_stock INTEGER NOT NULL
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
    /// Tests that the generic <see cref="ColumnMapping.ApplyMap{T}"/> method correctly maps columns to properties.
    /// </summary>
    [Fact]
    public async Task ApplyMap_Generic_ShouldMapColumnsToProperties()
    {
        // Arrange - Apply mapping for Product type
        ColumnMapping.ApplyMap<Product>();

        // Insert test data using database column names
        var productId = 1;
        const string productName = "Widget";
        const decimal unitPrice = 19.99m;
        const int quantityInStock = 100;

        await _connection.ExecuteAsync("""
            INSERT INTO products (id, product_name, unit_price, quantity_in_stock)
            VALUES (@id, @productName, @unitPrice, @quantityInStock)
            """, new { id = productId, productName, unitPrice, quantityInStock });

        // Act - Query data using Dapper with mapped type
        var products = (await _connection.QueryAsync<Product>(
            "SELECT id, product_name, unit_price, quantity_in_stock FROM products"
        )).ToList();

        // Assert
        Assert.Single(products);
        var product = products[0];
        Assert.Equal(productId, product.Id);
        Assert.Equal(productName, product.Name);
        Assert.Equal(unitPrice, product.Price);
        Assert.Equal(quantityInStock, product.Stock);
    }

    /// <summary>
    /// Tests that the type-based <see cref="ColumnMapping.ApplyMap(Type)"/> extension method correctly maps columns to properties.
    /// </summary>
    [Fact]
    public async Task ApplyMap_ByType_ShouldMapColumnsToProperties()
    {
        // Arrange - Apply mapping for Product type using reflection-based method
        typeof(Product).ApplyMap();

        // Insert test data
        const string productName = "Gadget";
        const decimal unitPrice = 29.99m;
        const int quantityInStock = 50;

        await _connection.ExecuteAsync("""
            INSERT INTO products (product_name, unit_price, quantity_in_stock)
            VALUES (@productName, @unitPrice, @quantityInStock)
            """, new { productName, unitPrice, quantityInStock });

        // Act
        var product = (await _connection.QueryFirstOrDefaultAsync<Product>(
            "SELECT id, product_name, unit_price, quantity_in_stock FROM products"
        ))!;

        // Assert
        Assert.NotNull(product);
        Assert.Equal(productName, product.Name);
        Assert.Equal(unitPrice, product.Price);
        Assert.Equal(quantityInStock, product.Stock);
    }

    /// <summary>
    /// Tests that column mappings correctly handle multiple properties on a type.
    /// </summary>
    [Fact]
    public async Task ApplyMap_WithMultipleProperties_ShouldMapAllColumns()
    {
        // Arrange
        ColumnMapping.ApplyMap<Product>();

        // Insert multiple products
        var testData = new[]
        {
            new { productName = "Product A", unitPrice = 10.0m, quantityInStock = 100 },
            new { productName = "Product B", unitPrice = 20.0m, quantityInStock = 200 },
            new { productName = "Product C", unitPrice = 30.0m, quantityInStock = 300 }
        };

        foreach (var item in testData)
        {
            await _connection.ExecuteAsync("""
                INSERT INTO products (product_name, unit_price, quantity_in_stock)
                VALUES (@productName, @unitPrice, @quantityInStock)
                """, item);
        }

        // Act
        var products = (await _connection.QueryAsync<Product>(
            "SELECT id, product_name, unit_price, quantity_in_stock FROM products"
        )).ToList();

        // Assert
        Assert.Equal(3, products.Count);
        Assert.Equal("Product A", products[0].Name);
        Assert.Equal("Product B", products[1].Name);
        Assert.Equal("Product C", products[2].Name);
    }

    /// <summary>
    /// Tests that column mappings work correctly with both insert and retrieve operations.
    /// </summary>
    [Fact]
    public async Task Mapping_ShouldWorkWithInsertAndRetrieve()
    {
        // Arrange
        ColumnMapping.ApplyMap<Product>();

        var originalProduct = new Product
        {
            Name = "Awesome Gadget",
            Price = 49.99m,
            Stock = 75
        };

        // Act - Insert using Dapper
        await _connection.ExecuteAsync("""
            INSERT INTO products (product_name, unit_price, quantity_in_stock)
            VALUES (@Name, @Price, @Stock)
            """, originalProduct);

        // Retrieve and verify
        var retrieved = await _connection.QueryFirstOrDefaultAsync<Product>(
            "SELECT id, product_name, unit_price, quantity_in_stock FROM products WHERE product_name = @Name",
            new { originalProduct.Name }
        );

        // Assert
        Assert.NotNull(retrieved);
        Assert.Equal(originalProduct.Name, retrieved.Name);
        Assert.Equal(originalProduct.Price, retrieved.Price);
        Assert.Equal(originalProduct.Stock, retrieved.Stock);
    }

    /// <summary>
    /// Tests that column mappings correctly handle nullable properties.
    /// </summary>
    [Fact]
    public async Task Mapping_ShouldHandleNullableValues()
    {
        // Arrange
        ColumnMapping.ApplyMap<ProductWithOptionalFields>();

        await _connection.ExecuteAsync("""
            CREATE TABLE IF NOT EXISTS products_extended (
                id INTEGER PRIMARY KEY,
                product_name TEXT NOT NULL,
                unit_price REAL NOT NULL,
                quantity_in_stock INTEGER NOT NULL,
                description TEXT
            )
            """);

        await _connection.ExecuteAsync("""
            INSERT INTO products_extended (product_name, unit_price, quantity_in_stock, description)
            VALUES (@Name, @Price, @Stock, @Description)
            """, new { Name = "Test Product", Price = 15.0m, Stock = 10, Description = (String?)null });

        // Act
        var product = await _connection.QueryFirstOrDefaultAsync<ProductWithOptionalFields>(
            "SELECT id, product_name, unit_price, quantity_in_stock, description FROM products_extended"
        );

        // Assert
        Assert.NotNull(product);
        Assert.Equal("Test Product", product.Name);
        Assert.Null(product.Description);
    }

    /// <summary>
    /// Test entity with column mappings.
    /// </summary>
    private class Product
    {
        /// <summary>
        /// Gets or sets the product ID.
        /// </summary>
        [ColumnMap("id")]
        public Int32 Id { get; set; }

        /// <summary>
        /// Gets or sets the product name, mapped from 'product_name' column.
        /// </summary>
        [ColumnMap("product_name")]
        public String Name { get; set; } = String.Empty;

        /// <summary>
        /// Gets or sets the product price, mapped from 'unit_price' column.
        /// </summary>
        [ColumnMap("unit_price")]
        public Decimal Price { get; set; }

        /// <summary>
        /// Gets or sets the quantity in stock, mapped from 'quantity_in_stock' column.
        /// </summary>
        [ColumnMap("quantity_in_stock")]
        public Int32 Stock { get; set; }
    }

    /// <summary>
    /// Test entity with optional fields for null handling testing.
    /// </summary>
    private class ProductWithOptionalFields
    {
        /// <summary>
        /// Gets or sets the product ID.
        /// </summary>
        public Int32 Id { get; set; }

        /// <summary>
        /// Gets or sets the product name.
        /// </summary>
        [ColumnMap("product_name")]
        public String Name { get; set; } = String.Empty;

        /// <summary>
        /// Gets or sets the product price.
        /// </summary>
        [ColumnMap("unit_price")]
        public Decimal Price { get; set; }

        /// <summary>
        /// Gets or sets the quantity in stock.
        /// </summary>
        [ColumnMap("quantity_in_stock")]
        public Int32 Stock { get; set; }

        /// <summary>
        /// Gets or sets the optional product description.
        /// </summary>
        [ColumnMap("description")]
        public String? Description { get; set; }
    }
}
