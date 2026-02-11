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

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProvisionData.Testing;

namespace ProvisionData.ResultPattern;

/// <summary>
/// The tests in this assembly are testing the compiled ProvisionData.ResultPattern assembly.
/// They are not testing the project source code. If you are seeing unexpected results, make
/// sure the ProvisionData.ResultPattern project has been compiled, packed, and copied to the
/// LocalPackages folder in the solution root.
/// </summary>
public class ResultPatternIntegrationTestFixture : IntegrationTestFixture
{
    protected override void ConfigureConfiguration(IConfigurationBuilder builder)
    {
        // This override suppresses the requirement for the appsettings.Testing.json file.
        // These tests do not require any configuration settings at this time.
    }

    /// <summary>
    /// Registers the ResultPattern infrastructure. This is necessary to ensure that the
    /// JsonConverters and other infrastructure are properly registered for the tests. If you
    /// comment it out you should see a warning about using ResultPattern without registering
    /// services, and many tests SHOULD fail due to missing converters.
    /// </summary>
    /// <param name="services">the service collection to which ResultPattern services will be added</param>
    /// <param name="configuration">the configuration instance for the test fixture</param>
    protected override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddResultPattern();
    }

    protected override ValueTask InitializeFixtureAsync(IServiceProvider services)
    {
        var logger = services.GetRequiredService<ILogger<ResultPatternIntegrationTestFixture>>();
        logger.LogInformation("""
            Initializing ResultPattern integration test fixture. The module initializers in the
            ProvisionData.ResultPattern.UnitTests.CustomErrors assembly should have run by now.
            """);

        return ValueTask.CompletedTask;
    }
}

public class ResultPatternUnitTestBase(ResultPatternIntegrationTestFixture fixture, ITestOutputHelper output)
    : IntegrationTestBase<ResultPatternIntegrationTestFixture>(fixture, output)
{
}
