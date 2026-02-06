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
/// Utility class providing extension methods for interacting with item resources in the HaloPSA API.
/// </summary>
public static class ItemExtensions
{
    /// <summary>
    /// Lists all items from the HaloPSA API.
    /// </summary>
    /// <param name="haloApiClient">The HaloPSA API client.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of items.</returns>
    public static async Task<IReadOnlyCollection<ItemDto>> ListItemsAsync(this ApiClient haloApiClient, CancellationToken cancellationToken = default)
    {
        var uri = "Item"
            .AppendQueryParam("count", 5000)
            .ToUri();

        var result = await haloApiClient.HttpGetAsync(uri, ItemJsonContext.Default.ListItemDto, cancellationToken);

        return result;
    }

    /// <summary>
    /// Gets a specific item by ID.
    /// </summary>
    /// <param name="haloApiClient">The HaloPSA API client.</param>
    /// <param name="itemId">The item ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The item details.</returns>
    public static async Task<ItemDto> GetItemAsync(this ApiClient haloApiClient, Int32 itemId, CancellationToken cancellationToken = default)
    {
        var uri = "Item"
            .AppendPathSegment(itemId)
            .ToUri();

        var result = await haloApiClient.HttpGetAsync(uri, ItemJsonContext.Default.ItemDto, cancellationToken);

        return result;
    }

    /// <summary>
    /// Creates a new item in HaloPSA.
    /// </summary>
    /// <param name="haloApiClient">The HaloPSA API client.</param>
    /// <param name="item">The item to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created item.</returns>
    public static async Task<ItemDto> CreateItemAsync(this ApiClient haloApiClient, ItemDto item, CancellationToken cancellationToken = default)
    {
        var uri = new Uri("Item");

        var payload = JsonSerializer.Serialize(item, ItemJsonContext.Default.ItemDto);

        var result = await haloApiClient.HttpPostAsync(uri, $"[{payload}]", ItemJsonContext.Default.ItemDto, cancellationToken);

        return result;
    }
}
