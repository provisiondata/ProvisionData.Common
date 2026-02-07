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
using ProvisionData.Testing;

namespace ProvisionData.HaloPSA.IntegrationTests;

public class ApiClientTestBase(ApiClientTestFixture fixture, ITestOutputHelper testOutputHelper)
    : IntegrationTestBase<ApiClient, ApiClientTestFixture>(fixture, testOutputHelper)
{
    private readonly ApiClientTestFixture _fixture = fixture;

    protected ApiClientTestData TestData => _fixture.TestData;
    protected Faker Bogus => _fixture.Bogus;
}
