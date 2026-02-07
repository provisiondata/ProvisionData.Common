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

using Flurl;
using Microsoft.Extensions.DependencyInjection;
using ProvisionData.HaloPSA.DTO;

namespace ProvisionData.HaloPSA.IntegrationTests;

public class ApiClientTests(ApiClientTestFixture fixture, ITestOutputHelper testOutputHelper)
    : ApiClientTestBase(fixture, testOutputHelper)
{
    private readonly ITestOutputHelper _testOutputHelper = testOutputHelper;

    [Fact]
    public async Task Client_ShouldAuthenticate()
    {
        // Arrange
        var client = Services.GetRequiredService<ApiClient>();

        // Act
        var info = await client.GetInstanceInfoAsync(CancellationToken.None);

        // Assert
        info.Should().NotBeNull();
        info.AppName.Should().Be("Halo PSA");
    }

    [Fact]
    public async Task ShouldGet_Client()
    {
        // Arrange

        // Act
        var client = await SUT.GetCustomerAsync(TestData.TestCustomerId, cancellationToken: CancellationToken);

        // Assert
        client.Should().NotBeNull();
        client.Id.Should().Be(TestData.TestCustomerId);
    }

    [Fact]
    public async Task ShouldGet_Invoice_with_Details()
    {
        // Act
        var invoice = await SUT.GetInvoiceAsync(TestData.TestInvoiceId, cancellationToken: CancellationToken);

        // Assert
        invoice.Should().NotBeNull();
        invoice.Id.Should().Be(TestData.TestInvoiceId);
        invoice.Lines.Should().NotBeEmpty();
        invoice.Lines.Single().Description.Should().StartWith("MISCELLANEOUS");
    }

    [Fact]
    public async Task ShouldGet_Asset()
    {
        // Arrange

        // Act
        var asset = await SUT.GetAssetAsync(TestData.TestAssetId, cancellationToken: CancellationToken);

        // Assert
        asset.Should().NotBeNull();
        asset.Id.Should().Be(TestData.TestAssetId);
        asset.SerialNumber.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Should_List_Asset()
    {
        // Arrange

        // Act
        var list = await SUT.ListAssetsAsync(cancellationToken: CancellationToken);

        // Assert
        list.Should().NotBeNull();
        list.Should().NotBeEmpty();
        list.Should().AllBeOfType<AssetDto>();
    }

    [Fact]
    public async Task Should_CreateAsset()
    {
        // Arrange
        var id = Guid.NewGuid().ToString().Replace("-", String.Empty)[..8];
        var serialNumber = Bogus.Commerce.Ean13();

        var asset = AssetDto.Create(TestData.TestCustomerId, TestData.TestAssetTypeId, TestData.TestSiteId, TestData.TestTechnicalOwnerId,
            assetNumber: $"ASSET-{id}", name: $"Test Asset {id}", Bogus.Person.FullName, Bogus.Commerce.ProductName(), serialNumber, DateTime.UtcNow.AddDays(-1));

        // Act
        var result = await SUT.CreateAssetAsync(asset, cancellationToken: CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.Fields.SingleOrDefault(f => f.Id == 117)?.Value.Should().Be(serialNumber);
    }

    [Fact]
    public async Task Should_GetJson()
    {
        var uri = "https://halo.pdsint.net/api/"
            .AppendPathSegment("Field")
            .AppendPathSegment("117")
            .AppendQueryParam("includedetails", "true")
            .AppendQueryParam("kind", "T")
            .ToUri();

        var json = await SUT.HttpGetAsync(uri, CancellationToken);

        _testOutputHelper.WriteLine(json);
    }
}
