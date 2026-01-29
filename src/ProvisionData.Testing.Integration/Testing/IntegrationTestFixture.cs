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

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ProvisionData.Testing;

/// <summary>
/// Provides a concrete implementation of a test services that sets up a full .NET services with dependency injection for integration tests.
/// </summary>
public class IntegrationTestFixture : TestFixtureBase
{
    private readonly IHost _host;

    /// <summary>
    /// Gets the configuration for the test services.
    /// </summary>
    public override IConfiguration Configuration { get; }

    /// <summary>
    /// Gets the dependency injection service provider for the current test scope.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when <see cref="BeginTest"/> has not been called.</exception>
    public override IServiceProvider Services => _testScope?.ServiceProvider
        ?? throw new InvalidOperationException("Test scope has not been started. Call BeginTest() before accessing Services.");

    private IServiceScope? _testScope;

    /// <summary>
    /// Initializes a new instance of the <see cref="IntegrationTestFixture"/> class.
    /// </summary>
    /// <remarks>
    /// This constructor builds the services and loads configuration from appsettings.Testing.json.
    /// </remarks>
    public IntegrationTestFixture()
    {
        var settings = new HostApplicationBuilderSettings()
        {
            EnvironmentName = "Testing"
        };

        ConfigureSettings(settings);

        var builder = Host.CreateEmptyApplicationBuilder(settings);

        ConfigureTest(builder.Configuration);

        ConfigureServices(builder.Services, builder.Configuration);

        _host = builder.Build();

        Configuration = _host.Services.GetRequiredService<IConfiguration>();

        InitializeFixture(_host.Services);
    }

    /// <summary>
    /// Provides an opportunity to configure the services instance before tests run.
    /// </summary>
    /// <remarks>Override this method in a derived class to apply custom configuration to the services. This
    /// method is called before the tests are started, allowing for additional setup or service registration.</remarks>
    /// <param name="services">The services to configure. Cannot be null.</param>
    protected virtual void InitializeFixture(IServiceProvider services)
    {
    }

    /// <summary>
    /// Called to configure services application builder settings before the services is created.
    /// </summary>
    /// <param name="settings">The services application builder settings to configure.</param>
    /// <remarks>
    /// Override this method in derived classes to customize services settings.
    /// </remarks>
    protected virtual void ConfigureSettings(HostApplicationBuilderSettings settings)
    {
    }

    /// <summary>
    /// Called to configure the configuration builder before services are configured.
    /// </summary>
    /// <param name="builder">The configuration builder to configure.</param>
    /// <remarks>
    /// By default, this loads appsettings.Testing.json from the current directory.
    /// Override this method in derived classes to customize configuration loading.
    /// </remarks>
    protected virtual void ConfigureTest(IConfigurationBuilder builder)
    {
        var basePath = Directory.GetCurrentDirectory();
        builder.SetBasePath(basePath)
            // appsettings.Testing.json: Excluded by .gitignore to prevent sensitive data from being checked in
            .AddJsonFile("appsettings.Testing.json", optional: false);
    }

    /// <summary>
    /// Called to configure dependency injection services for the test services.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configuration">The configuration instance to use.</param>
    /// <remarks>
    /// Override this method in derived classes to register services needed for tests.
    /// </remarks>
    protected virtual void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    { }

    /// <summary>
    /// Called before each test to initialize the test scope.
    /// </summary>
    public override void BeginTest()
    {
        _testScope = _host.Services.CreateScope();
    }

    /// <summary>
    /// Called after each test to dispose of the test scope.
    /// </summary>
    public override void EndTest()
    {
        _testScope?.Dispose();
        _testScope = null;
    }

    /// <summary>
    /// Releases the unmanaged resources used by the test services and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing"><see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release only unmanaged resources.</param>
    protected override void Dispose(Boolean disposing)
    {
        if (disposing)
        {
            _host?.Dispose();
        }

        base.Dispose(disposing);
    }
}
