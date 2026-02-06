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

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ProvisionData.HaloPSA.UnitTests;

public class HaloPsaApiClientTests
{
    private readonly Mock<IAuthTokenProvider> _tokenProviderMock;
    private readonly Mock<ILogger<ApiClient>> _loggerMock;
    private readonly Mock<IOptions<HaloPsaApiClientOptions>> _optionsMock;
    private readonly Mock<TimeProvider> _timeProviderMock;
    private readonly Mock<IFieldMappingProvider> _fieldMappingProviderMock;
    private readonly HaloPsaApiClientOptions _options;

    public HaloPsaApiClientTests()
    {
        _tokenProviderMock = new Mock<IAuthTokenProvider>();
        _loggerMock = new Mock<ILogger<ApiClient>>();
        _optionsMock = new Mock<IOptions<HaloPsaApiClientOptions>>();
        _timeProviderMock = new Mock<TimeProvider>();
        _fieldMappingProviderMock = new Mock<IFieldMappingProvider>();

        _options = new HaloPsaApiClientOptions
        {
            AuthUrl = "https://test.halo.local/auth/",
            ApiUrl = "https://test.halo.local/api/",
            ClientId = "test-client-id",
            ClientSecret = "test-client-secret",
            PageSize = 50
        };

        _optionsMock.Setup(x => x.Value).Returns(_options);
    }

    [Fact]
    public void Constructor_ShouldInitialize_WithValidParameters()
    {
        // Arrange
        var httpClient = new HttpClient();

        // Act
        var client = new ApiClient(
            httpClient,
            _optionsMock.Object,
            _tokenProviderMock.Object,
            _timeProviderMock.Object,
            _loggerMock.Object,
            _fieldMappingProviderMock.Object
        );

        // Assert
        client.Should().NotBeNull();
    }

    [Fact]
    public void HaloPsaApiClientOptions_ShouldHaveDefaultValues()
    {
        // Arrange & Act
        var options = new HaloPsaApiClientOptions();

        // Assert
        options.PageSize.Should().Be(50);
        options.AuthUrl.Should().Be(String.Empty);
        options.ApiUrl.Should().Be(String.Empty);
        options.ClientId.Should().Be(String.Empty);
        options.ClientSecret.Should().Be(String.Empty);
    }
}
