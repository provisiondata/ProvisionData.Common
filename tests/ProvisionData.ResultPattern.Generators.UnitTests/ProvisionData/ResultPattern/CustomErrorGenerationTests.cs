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
/// Tests to verify that custom errors defined in consuming assemblies are properly generated.
/// This also demonstrates the REGISTRATION CHALLENGE: how do these errors get registered
/// in ErrorCodeRegistry which lives in a different assembly?
/// </summary>
public sealed class CustomErrorGenerationTests
{
    [Fact]
    public void OrderNotFoundError_Should_BeCreatable()
    {
        // Arrange & Act
        var error = new OrderNotFoundError(
            orderId: "ORD-12345",
            description: "Order ORD-12345 was not found in the system");

        // Assert
        Assert.Equal("ORD-12345", error.OrderId);
        Assert.Equal("Order ORD-12345 was not found in the system", error.Description);
        Assert.NotNull(error.Code);
    }

    [Fact]
    public void InventoryInsufficientError_Should_HandleMultipleProperties()
    {
        // Arrange & Act
        var error = new InventoryInsufficientError(
            productSku: "WIDGET-001",
            requestedQuantity: 100,
            availableQuantity: 25,
            description: "Insufficient inventory for WIDGET-001");

        // Assert
        Assert.Equal("WIDGET-001", error.ProductSku);
        Assert.Equal(100, error.RequestedQuantity);
        Assert.Equal(25, error.AvailableQuantity);
        Assert.Equal("Insufficient inventory for WIDGET-001", error.Description);
    }

    [Fact]
    public void DatabaseConnectionError_Should_WorkWithNoAdditionalProperties()
    {
        // Arrange & Act
        var error = new DatabaseConnectionError(
            description: "Failed to connect to database");

        // Assert
        Assert.Equal("Failed to connect to database", error.Description);
        Assert.NotNull(error.Code);
    }
}
