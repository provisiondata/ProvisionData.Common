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

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProvisionData.Testing;

namespace ProvisionData.ResultPattern;

public sealed partial class NotFoundError : Error { }

public sealed partial class TransactionError : Error
{
    public TransactionFailureReason Reason { get; init; }
    public String TransactionId { get; init; }
}

public enum TransactionFailureReason
{
    InsufficientFunds,
    CardExpired,
    InvalidCardNumber,
    NetworkError,
    UnknownError
}

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

public class ResultPatternIntegrationTestFixture : IntegrationTestFixture
{
    protected override void ConfigureConfiguration(IConfigurationBuilder builder)
    {
        // Removes the need for an appsettings.Testing.json file in the test project,
        // since the ResultPattern tests don't require any configuration settings.
    }

    protected override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddResultPattern();
    }

    protected override ValueTask InitializeFixtureAsync(IServiceProvider services)
    {
        var logger = services.GetRequiredService<ILogger<ResultPatternIntegrationTestFixture>>();
        logger.LogInformation("Initializing ResultPattern integration test fixture.");

        return ValueTask.CompletedTask;
    }
}

public class ResultPatternUnitTestBase(ResultPatternIntegrationTestFixture fixture, ITestOutputHelper output)
    : IntegrationTestBase<ResultPatternIntegrationTestFixture>(fixture, output)
{
}
