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
/// Provides extension methods for managing recurring invoices using the HaloPSA API.
/// </summary>
public static class RecurringInvoiceExtensions
{
    /// <summary>
    /// Lists all recurring invoices from the HaloPSA API.
    /// </summary>
    /// <param name="haloApiClient">The HaloPSA API client.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of invoice headers.</returns>
    public static async Task<IReadOnlyCollection<InvoiceHeaderDto>> ListRecurringInvoicesAsync(this ApiClient haloApiClient, CancellationToken cancellationToken = default)
    {
        var uri = "RecurringInvoice"
            .AppendQueryParam("count", 5000)
            .AppendQueryParam("pageinate", false)
            .AppendQueryParam("page_size", 5000)
            .ToUri();

        var result = await haloApiClient.HttpGetAsync(uri, InvoiceJsonContext.Default.ListInvoiceHeaderDto, cancellationToken);

        return result;
    }

    /// <summary>
    /// Gets a specific recurring invoice by ID.
    /// </summary>
    /// <param name="haloApiClient">The HaloPSA API client.</param>
    /// <param name="recurringinvoiceId">The recurring invoice ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The invoice header.</returns>
    public static async Task<InvoiceHeaderDto> GetRecurringInvoiceAsync(this ApiClient haloApiClient, Int32 recurringinvoiceId, CancellationToken cancellationToken = default)
    {
        var uri = "RecurringInvoice"
            .AppendPathSegment(recurringinvoiceId)
            .ToUri();
        var result = await haloApiClient.HttpGetAsync(uri, InvoiceJsonContext.Default.InvoiceHeaderDto, cancellationToken);

        return result;
    }

    /// <summary>
    /// Creates a new recurring invoice in HaloPSA.
    /// </summary>
    /// <param name="haloApiClient">The HaloPSA API client.</param>
    /// <param name="request">The invoice header to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created invoice header.</returns>
    public static async Task<InvoiceHeaderDto> CreateRecurringInvoiceAsync(this ApiClient haloApiClient, InvoiceHeaderDto request, CancellationToken cancellationToken = default)
    {
        var uri = new Uri("RecurringInvoice");

        var payload = JsonSerializer.Serialize(request, InvoiceJsonContext.Default.InvoiceHeaderDto);

        var result = await haloApiClient.HttpPostAsync(uri, $"[{payload}]", InvoiceJsonContext.Default.InvoiceHeaderDto, cancellationToken);

        await CreateRecurringInvoiceScheduleAsync(haloApiClient, result, cancellationToken);

        return result;
    }

    /// <summary>
    /// Creates the schedule for a recurring invoice.
    /// </summary>
    /// <param name="haloApiClient">The HaloPSA API client.</param>
    /// <param name="invoice">The invoice header to create the schedule for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public static async Task CreateRecurringInvoiceScheduleAsync(this ApiClient haloApiClient, InvoiceHeaderDto invoice, CancellationToken cancellationToken = default)
    {
        var schedule = new StdRequest()
        {
            Id = invoice.Id,
            Period = invoice.Schedule.Period,
            Lastcreated = invoice.Schedule.Lastcreated,
            NextCreationDate = invoice.Schedule.NextCreationDate,
            Startdate = invoice.Schedule.Startdate
        };

        var uri = new Uri("Template");

        var payload = JsonSerializer.Serialize(schedule, InvoiceJsonContext.Default.StdRequest);

        var result = await haloApiClient.HttpPostAsync(uri, $"[{payload}]", InvoiceJsonContext.Default.StdRequest, cancellationToken);
    }
}
