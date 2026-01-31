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

using Microsoft.Extensions.DependencyInjection;

namespace ProvisionData.Testing.Integration.Examples.Customers;

public class CustomerServiceTests(CustomersFixture fixture, ITestOutputHelper output)
    : CustomerTestBase<CustomerApplicationService>(fixture, output)
{
    [Fact]
    public async Task CreateCustomer_WithValidData_ShouldReturn_Success()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var command = new CreateCustomerCommand(id, "Acme Corporation", "contact@acme.com");
        var result = await SUT.CreateCustomerAsync(command, CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(id, result.Value);

        var repository = Services.GetRequiredService<ICustomerRepository>();
        var created = await repository.GetByIdAsync(id, CancellationToken);
        Assert.Equal("Acme Corporation", created.Name);
        Assert.Equal("contact@acme.com", created.Email);
    }

    [Fact]
    public async Task GetCustomer_WhenExists_ShouldReturnCustomer()
    {
        // Arrange - Create test data using the repository directly
        var id = Guid.NewGuid();
        var repository = Services.GetRequiredService<ICustomerRepository>();
        var customer = new Customer(id, "Test Corp");
        await repository.CreateAsync(customer, TestContext.Current.CancellationToken);

        // Act - Use the SUT to retrieve it
        var query = new GetCustomerByIdQuery(id);
        var result = await SUT.GetCustomerByIdAsync(query, CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(id, result.Value.Id);
        Assert.Equal("Test Corp", result.Value.Name);
        Assert.Matches(String.Empty, result.Value.Email);
    }

    [Fact]
    public async Task UpdateCustomer_WhenExists_ShouldPersistChanges()
    {
        // Arrange
        var id = Guid.NewGuid();
        var repository = Services.GetRequiredService<ICustomerRepository>();
        var customer = new Customer(id, "Original Name");
        await repository.CreateAsync(customer, CancellationToken);

        // Act
        var command = new UpdateCustomerCommand(id, "Updated Name", "user@example.com");
        await SUT.UpdateCustomerAsync(command, CancellationToken);

        // Assert
        var updated = await repository.GetByIdAsync(id, CancellationToken);
        Assert.Equal("Updated Name", updated.Name);
        Assert.Equal("user@example.com", updated.Email);
    }
}
