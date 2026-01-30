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
public class IntegrationTestFixture : DisposableBase, IAsyncTestFixture, IAsyncLifetime
{
    private readonly IHost _host;

    private IServiceScope? _testScope;
    private Int32 _initializationCount;
    private Int32 _beginCount;
    private Int32 _disposalCount;
    private Int32 _endCount;

    /// <summary>
    /// Gets the configuration for the test fixture.
    /// </summary>
    public IConfiguration Configuration { get; }

    /// <summary>
    /// Gets the dependency injection service provider for the current test scope.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the fixture has not been initialized.</exception>
    public IServiceProvider Services => _testScope?.ServiceProvider
        ?? throw new InvalidOperationException("Test scope has not been started. Call BeginTestAsync() before accessing Services.");

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

        ConfigureBuilder(builder.Configuration);

        ConfigureServices(builder.Services, builder.Configuration);

        _host = builder.Build();

        Configuration = _host.Services.GetRequiredService<IConfiguration>();
    }

    /// <summary>
    /// Configures the fixture before tests run. Implementers should override <see cref="InitializeFixtureAsync(IServiceProvider)"/> to customize initialization.
    /// </summary>
    public async ValueTask InitializeAsync()
    {
        _initializationCount++;
        using var scope = _host.Services.CreateScope();
        await InitializeFixtureAsync(scope.ServiceProvider);
    }

    /// <summary>
    /// Provides an opportunity to configure the services instance before tests run.
    /// </summary>
    /// <remarks>Override this method in a derived class to apply custom configuration to the services. This
    /// method is called before the tests are started, allowing for additional setup or service registration.</remarks>
    /// <param name="services">The services to configure. Cannot be null.</param>
    protected virtual ValueTask InitializeFixtureAsync(IServiceProvider services)
    {
        return ValueTask.CompletedTask;
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
    protected virtual void ConfigureBuilder(IConfigurationBuilder builder)
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
    {
    }

    /// <summary>
    /// Called before each test to initialize the test scope.
    /// </summary>
    ValueTask IAsyncTestFixture.BeginTestAsync()
    {
        if (_initializationCount != 1)
        {
            throw new InvalidOperationException("Fixture has not been initialized. Ensure your fixture is overriding InitializeFixtureAsync(IServiceProvider) and not InitializeFixtureAsync() before beginning tests.");
        }

        _beginCount++;
        _testScope = _host.Services.CreateScope();
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Called after each test to dispose of the test scope.
    /// </summary>
    ValueTask IAsyncTestFixture.EndTestAsync()
    {
        _endCount++;
        _testScope?.Dispose();
        _testScope = null;
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Asynchronously disposes resources used by the fixture.
    /// </summary>
    /// <returns>A ValueTask that represents the asynchronous dispose operation.</returns>
    protected override ValueTask DisposeAsyncCore()
    {
        _disposalCount++;
        _host?.Dispose();

        Console.WriteLine($"IntegrationTestFixture disposed. InitializationCount={_initializationCount}, BeginCount={_beginCount}, EndCount={_endCount}, DisposalCount={_disposalCount}");

        return base.DisposeAsyncCore();
    }
}
