// Provision Data HaloPSA API Client
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

public class IntegrationTestFixture : TestFixtureBase
{
    private readonly IHost _host;

    public override IConfiguration Configuration { get; }
    public override IServiceProvider Services => _testScope?.ServiceProvider
        ?? throw new InvalidOperationException("Test scope has not been started. Call BeginTest() before accessing Services.");

    private IServiceScope? _testScope;

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
    }

    protected virtual void ConfigureSettings(HostApplicationBuilderSettings settings)
    {
    }

    protected virtual void ConfigureTest(IConfigurationBuilder builder)
    {
        var basePath = Directory.GetCurrentDirectory();
        builder.SetBasePath(basePath)
            // appsettings.Testing.json: Excluded by .gitignore to prevent sensitive data from being checked in
            .AddJsonFile("appsettings.Testing.json", optional: false);
    }

    protected virtual void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    { }

    public override void BeginTest()
    {
        _testScope = _host.Services.CreateScope();
    }

    public override void EndTest()
    {
        _testScope?.Dispose();
        _testScope = null;
    }

    protected override void Dispose(Boolean disposing)
    {
        if (disposing)
        {
            _host?.Dispose();
        }

        base.Dispose(disposing);
    }
}
