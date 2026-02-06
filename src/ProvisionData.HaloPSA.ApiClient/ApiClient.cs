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

using Flurl;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProvisionData.HaloPSA.DTO;
using System.Text.Json;

namespace ProvisionData.HaloPSA;

/// <summary>
/// HaloPSA API client providing typed access to the HaloPSA REST API.
/// </summary>
public class ApiClient(
        HttpClient httpClient,
        IOptions<HaloPsaApiClientOptions> options,
        IAuthTokenProvider tokenRepository,
        TimeProvider timeProvider,
        ILogger<ApiClient> logger,
        IFieldMappingProvider fieldMappingProvider
    ) : ApiClientBase(httpClient, options.Value, tokenRepository, timeProvider, logger, fieldMappingProvider)
{

    /// <summary>
    /// Gets information about the HaloPSA instance.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The instance information.</returns>
    public async Task<InstanceInfo> GetInstanceInfoAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var uri = Options.ApiUrl
                .AppendPathSegment("InstanceInfo")
                .ToUri();
            var json = await HttpGetAsync(uri, cancellationToken);

            var instanceInfo = JsonSerializer.Deserialize<InstanceInfo>(json, ApiJsonSerializerContext.Default.InstanceInfo)
                ?? throw new HaloApiException("Failed to deserialize InstanceInfo.", json);

            Logger.LogInformation("{Instance}", instanceInfo);

            return instanceInfo;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Something went wrong.");
            throw;
        }
    }
}
