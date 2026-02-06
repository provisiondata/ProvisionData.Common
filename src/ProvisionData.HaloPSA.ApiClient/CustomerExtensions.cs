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
using ProvisionData.HaloPSA.Contexts;
using ProvisionData.HaloPSA.DTO;
using System.Text.Json;

namespace ProvisionData.HaloPSA;

/// <summary>
/// Provides extension methods for interacting with customer resources in the HaloPSA API.
/// </summary>
public static class CustomerExtensions
{
    /// <summary>
    /// Lists all clients from the HaloPSA API.
    /// </summary>
    /// <param name="haloApiClient">The HaloPSA API client.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of <see cref="CustomerViewDto"/> representing clients.</returns>
    public static async Task<CustomerListDto?> ListCustomersAsync(this ApiClient haloApiClient, CancellationToken cancellationToken = default)
    {
        var uri = "Client"
            .AppendQueryParam("count", 5000)
            .ToUri();

        var json = await haloApiClient.HttpGetAsync(uri, cancellationToken);

        var list = JsonSerializer.Deserialize(json, CustomerJsonContext.Default.CustomerListDto)
            ?? throw new InvalidOperationException("Failed to deserialize ListClientsResponse.");

        return list;
    }

    /// <summary>
    /// Gets a customer by ID.
    /// </summary>
    /// <param name="haloApiClient">The HaloPSA API client.</param>
    /// <param name="customerId">The customer ID.</param>
    /// <param name="includeDetails">Whether to include detailed information.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The customer representing the customer.</returns>
    public static async Task<CustomerDto> GetCustomerAsync(this ApiClient haloApiClient, Int32 customerId, Boolean includeDetails = false, CancellationToken cancellationToken = default)
    {
        // https://halo.pdsint.net/api/Client/172?includedetails=true
        var clientUri = "Client"
            .AppendPathSegment(customerId)
            .AppendQueryParam("includedetails", includeDetails)
            .ToUri();

        var customer = await haloApiClient.HttpGetAsync(clientUri, CustomerJsonContext.Default.CustomerDto, cancellationToken);

        return customer;
    }

    /// <summary>
    /// Gets a client by name.
    /// </summary>
    /// <param name="haloApiClient">The HaloPSA API client.</param>
    /// <param name="name">The client name to search for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The customer representing the client.</returns>
    public static async Task<CustomerDto?> GetCustomerAsync(this ApiClient haloApiClient, String name, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(haloApiClient);
        ArgumentNullException.ThrowIfNull(name);

        var uri = "Client"
            .AppendQueryParam("search_name_only", name)
            .ToUri();

        var customer = await haloApiClient.HttpGetAsync(uri, CustomerJsonContext.Default.CustomerDto, cancellationToken);

        return customer;
    }
}
