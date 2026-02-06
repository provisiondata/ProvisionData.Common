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

using Bogus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ProvisionData.Testing;
using System.Diagnostics.CodeAnalysis;

namespace ProvisionData.HaloPSA.IntegrationTests;

public class ApiClientTestFixture : IntegrationTestFixture
{
    private ApiClientTestData? _testData;

    public ApiClientTestData TestData
        => _testData ??= Services.GetRequiredService<IOptions<ApiClientTestData>>().Value;

    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "I want a new Faker for each fixture instance.")]
    private Lazy<Faker> LazyFaker => new(() => new Faker());
    public Faker Bogus => LazyFaker.Value;

    protected override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ApiClientTestData>(configuration.GetSection(nameof(ApiClientTestData)));
        services.AddHaloPsaApiClient(configuration);
    }

    protected override ValueTask InitializeFixtureAsync(IServiceProvider services)
    {
        services.EnsureCustomFieldsHaveBeenMapped();

        return ValueTask.CompletedTask;
    }
}
