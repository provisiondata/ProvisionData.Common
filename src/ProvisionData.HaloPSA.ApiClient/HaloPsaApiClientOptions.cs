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

namespace ProvisionData.HaloPSA;

/// <summary>
/// Configuration ReaderOptions for the HaloPSA API client.
/// </summary>
public class HaloPsaApiClientOptions
{
    /// <summary>
    /// Gets or sets the configuration section name used when binding from IConfiguration.
    /// </summary>
    public static String SectionName { get; set; } = nameof(HaloPsaApiClientOptions);

    /// <summary>
    /// Gets or sets the authentication URL for the HaloPSA OAuth endpoint.
    /// </summary>
    public String AuthUrl { get; set; } = String.Empty;

    /// <summary>
    /// Gets or sets the base API URL for the HaloPSA REST API.
    /// </summary>
    public String ApiUrl { get; set; } = String.Empty;

    /// <summary>
    /// Gets or sets the OAuth client ID.
    /// </summary>
    public String ClientId { get; set; } = String.Empty;

    /// <summary>
    /// Gets or sets the OAuth client secret.
    /// </summary>
    public String ClientSecret { get; set; } = String.Empty;

    /// <summary>
    /// Gets or sets the default page size for paginated API requests.
    /// </summary>
    public Int32 PageSize { get; set; } = 50;

    /// <summary>
    /// The maximum number of retries for TooManyRequests failures.
    /// </summary>
    public Int32 MaxRetries { get; set; } = 10;
}
