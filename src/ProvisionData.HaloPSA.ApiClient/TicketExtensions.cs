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
/// Provides extension methods for interacting with tickets in the HaloPSA API.
/// </summary>
public static class TicketExtensions
{
    /// <summary>
    /// Lists all faults/tickets from the HaloPSA API.
    /// </summary>
    /// <param name="haloApiClient">The HaloPSA API client.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of tickets.</returns>
    public static async Task<IReadOnlyList<TicketDto>> ListTicketsAsync(this ApiClient haloApiClient, CancellationToken cancellationToken = default)
    {
        var uri = "Tickets"
            .AppendQueryParam("count", 5000)
            .ToUri();

        var result = await haloApiClient.HttpGetAsync(uri, TicketJsonContext.Default.ListTicketDto, cancellationToken);

        return result;
    }

    /// <summary>
    /// Creates a new ticket/ticket in HaloPSA.
    /// </summary>
    /// <param name="haloApiClient">The HaloPSA API client.</param>
    /// <param name="ticket">The ticket to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>the created <see cref="TicketDto"/></returns>
    public static async Task<TicketDto> CreateTicketAsync(this ApiClient haloApiClient, TicketDto ticket, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(ticket);

        var uri = new Uri("Tickets");

        var payload = JsonSerializer.Serialize(ticket, TicketJsonContext.Default.TicketDto);

        var result = await haloApiClient.HttpPostAsync(uri, $"[{payload}]", TicketJsonContext.Default.TicketDto, cancellationToken);

        return result;
    }
}
