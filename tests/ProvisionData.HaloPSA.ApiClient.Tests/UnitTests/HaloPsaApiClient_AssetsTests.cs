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
using System.Net;

namespace ProvisionData.HaloPSA.UnitTests;

public class HaloPsaApiClient_AssetsTests
{
    private readonly Mock<IAuthTokenProvider> _tokenProviderMock;
    private readonly Mock<ILogger<ApiClient>> _loggerMock;
    private readonly Mock<IOptions<HaloPsaApiClientOptions>> _optionsMock;
    private readonly Mock<TimeProvider> _timeProviderMock;
    private readonly Mock<IFieldMappingProvider> _fieldMappingProviderMock;
    private readonly HaloPsaApiClientOptions _options;

    public HaloPsaApiClient_AssetsTests()
    {
        _tokenProviderMock = new Mock<IAuthTokenProvider>();
        _loggerMock = new Mock<ILogger<ApiClient>>();
        _optionsMock = new Mock<IOptions<HaloPsaApiClientOptions>>();
        _timeProviderMock = new Mock<TimeProvider>();
        _fieldMappingProviderMock = new Mock<IFieldMappingProvider>();

        _tokenProviderMock.Setup(tp => tp.GetTokenAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuthToken());

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

    // Example test structure - you'll need to implement proper HTTP mocking
    // This demonstrates the test pattern with Shouldly assertions
    [Fact]
    public async Task ListAssetsAsync_ShouldThrow_HaloPsaApiClientException_WhenDeserializationFails()
    {
        // Arrange
        var httpClient = new HttpClient(new MockHttpMessageHandler("invalid json"));
        var client = new ApiClient(
            httpClient,
            _optionsMock.Object,
            _tokenProviderMock.Object,
            _timeProviderMock.Object,
            _loggerMock.Object,
            _fieldMappingProviderMock.Object
        );

        // Act & Assert
        var exception = await Record.ExceptionAsync(
            async () => await client.ListAssetsAsync(CancellationToken.None)
        );

        exception.Should().BeOfType<HaloApiException>();
    }
}

// Helper class for mocking HTTP responses
internal class MockHttpMessageHandler(String response, HttpStatusCode statusCode = HttpStatusCode.OK) : HttpMessageHandler
{
    private readonly String _response = response;
    private readonly HttpStatusCode _statusCode = statusCode;

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(new HttpResponseMessage
        {
            StatusCode = _statusCode,
            Content = new StringContent(_response)
        });
    }
}
