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
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace ProvisionData.HaloPSA;

// TODO: Add ITokenStorage for token persistence (e.g., to a redis or database).

internal class AuthTokenProvider(
    IOptions<HaloPsaApiClientOptions> optionsAccessor,
    IHttpClientFactory httpClientFactory,
    TimeProvider timeProvider
    ) : IAuthTokenProvider
{
    private readonly HaloPsaApiClientOptions _options = optionsAccessor.Value;
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly TimeProvider _timeProvider = timeProvider;

    private AuthToken? _token;

    public async Task<AuthToken?> GetTokenAsync(CancellationToken cancellationToken)
    {
        if (_token?.IsExpired(_timeProvider.GetUtcNow()) != false)
        {
            _token = await ActuallyGetTokenAsync(cancellationToken);
        }

        return _token;
    }

    private async Task<AuthToken> ActuallyGetTokenAsync(CancellationToken cancellationToken)
    {
        var json = String.Empty;
        try
        {
            var form = new Dictionary<String, String>
                {
                    {"client_id", _options.ClientId},
                    {"client_secret", _options.ClientSecret},
                    {"grant_type", "client_credentials"},
                    {"scope", "all"},
                };

            var uri = _options.AuthUrl
                .AppendPathSegment("token")
                .ToUri();

            using var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.PostAsync(uri, new FormUrlEncodedContent(form), cancellationToken);

            json = await response.Content.ReadAsStringAsync(cancellationToken);

            response.EnsureSuccessStatusCode();

            var token = JsonSerializer.Deserialize(json, AuthJsonSerializerContext.Default.AuthToken)
                ?? throw new HaloApiException("Failed to deserialize access token.", json);

            token.IssuedAt = _timeProvider.GetUtcNow();

            return token;
        }
        catch (Exception ex)
        {
            throw new HaloApiException("Failed to get access token.", json, ex);
        }
    }
}
