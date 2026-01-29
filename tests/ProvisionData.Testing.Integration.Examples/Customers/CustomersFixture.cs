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

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace ProvisionData.Testing.Integration.Examples.Customers;

// Fixture
public class CustomersFixture : IntegrationTestFixture
{
    protected override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // Register data access
        services.AddDbContext<CustomerDbContext>(options =>
            options.UseInMemoryDatabase("CustomerTests")
                   .EnableSensitiveDataLogging(true));

        // Register repositories
        services.AddScoped<ICustomerRepository, CustomerRepository>();

        // Register services
        services.AddScoped<CustomerApplicationService>();
    }

    [SuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "<Pending>")]
    protected override void InitializeFixture(IServiceProvider services)
    {
        // Ensure database schema exists
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CustomerDbContext>();
        dbContext.Database.EnsureCreated();
    }

    public override void BeginTest()
    {
        base.BeginTest();

        // Clear data for test isolation
        var dbContext = Services.GetRequiredService<CustomerDbContext>();
        dbContext.Customers.RemoveRange(dbContext.Customers);
        dbContext.SaveChanges();
    }
}
