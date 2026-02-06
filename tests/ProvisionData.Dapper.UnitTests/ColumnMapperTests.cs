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
/// Tests the <see cref="ColumnMapper"/> functionality using an in-memory SQLite database.
/// </summary>
public class ColumnMapperTests : IAsyncLifetime
{
    private SqliteConnection _connection = null!;

    /// <summary>
    /// Initializes the test by creating an in-memory SQLite database.
    /// </summary>
    public async ValueTask InitializeAsync()
    {
        // Cleanup any existing mappings
        SqlMapper.RemoveTypeMap(typeof(Product));
        SqlMapper.RemoveTypeMap(typeof(ProductWithOptionalFields));

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
    /// Tests that the generic <see cref="ColumnMapper.ApplyMap{T}"/> method correctly maps columns to properties.
    /// </summary>
    [Fact]
    public async Task ApplyMap_Generic_ShouldMapColumnsToProperties()
    {
        // Arrange - Apply mapping for Product type
        ColumnMapper.Map<Product>();

        // Insert test data using database column names
        var productId = 1;
        const String productName = "Widget";
        const Decimal unitPrice = 19.99m;
        const Int32 quantityInStock = 100;

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
    /// Tests that the type-based <see cref="ColumnMapper.MapColumns(Type)"/> extension method correctly maps columns to properties.
    /// </summary>
    [Fact]
    public async Task ApplyMap_ByType_ShouldMapColumnsToProperties()
    {
        // Arrange - Apply mapping for Product type using reflection-based method
        typeof(Product).MapColumns();

        // Insert test data
        const String productName = "Gadget";
        const Decimal unitPrice = 29.99m;
        const Int32 quantityInStock = 50;

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

    [Fact]
    public async Task MapTypesFromAssemblies_Should_MapTypes()
    {
        // Arrange
        const String productName = "Widget";
        const Decimal unitPrice = 19.99m;
        const Int32 quantityInStock = 100;

        await _connection.ExecuteAsync("""
            INSERT INTO products (product_name, unit_price, quantity_in_stock)
            VALUES (@productName, @unitPrice, @quantityInStock)
            """, new { productName, unitPrice, quantityInStock });

        // Act
        ColumnMapper.MapTypesFromAssemblies(typeof(ProductWithOptionalFields).Assembly);

        // Assert
        var product = await _connection.QueryFirstOrDefaultAsync<Product>(
            "SELECT id, product_name, unit_price, quantity_in_stock FROM products"
        );

        Assert.NotNull(product);
        Assert.Equal(productName, product.Name);
        Assert.Equal(unitPrice, product.Price);
        Assert.Equal(quantityInStock, product.Stock);
    }

    [Fact]
    public async Task MapTypesFromExecutingAssembly_Should_MapTypes()
    {
        // Arrange
        const String productName = "Widget";
        const Decimal unitPrice = 19.99m;
        const Int32 quantityInStock = 100;

        await _connection.ExecuteAsync("""
            INSERT INTO products (product_name, unit_price, quantity_in_stock)
            VALUES (@productName, @unitPrice, @quantityInStock)
            """, new { productName, unitPrice, quantityInStock });

        // Act
        // This is a little redundant since it delegates to MapTypesFromAssemblies().
        ColumnMapper.MapTypesFromExecutingAssembly();

        // Assert
        var product = await _connection.QueryFirstOrDefaultAsync<Product>(
            "SELECT id, product_name, unit_price, quantity_in_stock FROM products"
        );

        Assert.NotNull(product);
        Assert.Equal(productName, product.Name);
        Assert.Equal(unitPrice, product.Price);
        Assert.Equal(quantityInStock, product.Stock);
    }

    [Fact]
    public async Task MapTypesFromAssemblyContaining_T_Should_MapTypes()
    {
        // Arrange
        const String productName = "Widget";
        const Decimal unitPrice = 19.99m;
        const Int32 quantityInStock = 100;

        await _connection.ExecuteAsync("""
            INSERT INTO products (product_name, unit_price, quantity_in_stock)
            VALUES (@productName, @unitPrice, @quantityInStock)
            """, new { productName, unitPrice, quantityInStock });

        // Act
        // This is a little redundant since it delegates to MapTypesFromAssemblies().
        ColumnMapper.MapTypesFromAssemblyContaining<ProductWithOptionalFields>();

        // Assert
        var product = await _connection.QueryFirstOrDefaultAsync<Product>(
            "SELECT id, product_name, unit_price, quantity_in_stock FROM products"
        );

        Assert.NotNull(product);
        Assert.Equal(productName, product.Name);
        Assert.Equal(unitPrice, product.Price);
        Assert.Equal(quantityInStock, product.Stock);
    }

    /// <summary>
    /// Tests that column mappings work correctly with both insert and retrieve operations.
    /// </summary>
    [Fact]
    public async Task Mapping_ShouldWorkWithInsertAndRetrieve()
    {
        // Arrange
        ColumnMapper.Map<Product>();

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
    /// Test entity with column mappings.
    /// </summary>
    [HasColumnMaps]
    private class Product
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
        [ColumnName("product_name")]
        public String Name { get; set; } = String.Empty;

        /// <summary>
        /// Gets or sets the product price.
        /// </summary>
        [ColumnName("unit_price")]
        public Decimal Price { get; set; }

        /// <summary>
        /// Gets or sets the quantity in stock.
        /// </summary>
        [ColumnName("quantity_in_stock")]
        public Int32 Stock { get; set; }

        /// <summary>
        /// Gets or sets the optional product description.
        /// </summary>
        [ColumnName("description")]
        public String? Description { get; set; }
    }
}
