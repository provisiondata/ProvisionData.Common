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
/// Provides extension methods for interacting with asset resources in the HaloPSA API.
/// </summary>
public static partial class AssetExtensions
{
    /// <summary>
    /// Lists all assets from the HaloPSA API.
    /// </summary>
    /// <param name="haloApiClient">The HaloPSA API client.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of device assetView items.</returns>
    public static async Task<IReadOnlyCollection<AssetDto>?> ListAssetsAsync(this ApiClient haloApiClient, CancellationToken cancellationToken = default)
    {
        var uri = "Asset"
            .AppendQueryParam("count", 5000)
            .ToUri();

        var json = await haloApiClient.HttpGetAsync(uri, cancellationToken);
        var assetView = JsonSerializer.Deserialize(json, AssetJsonContext.Default.AssetViewDto)
            ?? throw new HaloApiException("Failed to deserialize AssetView.", json);

        return assetView?.Assets is not null ? assetView.Assets.AsReadOnly() : new List<AssetDto>().AsReadOnly();
    }

    public static async Task<IReadOnlyCollection<AssetDto>> QueryAssetsAsync(ApiClient haloApiClient, Int32 customerId, CancellationToken cancellationToken = default)
    {
        Uri uri = "Asset"
            .AppendQueryParam("client_id", customerId)
            .AppendQueryParam("columns_id", 9)
            .AppendQueryParam("count", 200)
            .ToUri();

        var json = await haloApiClient.HttpGetAsync(uri, cancellationToken);
        var assetView = JsonSerializer.Deserialize(json, AssetJsonContext.Default.AssetViewDto)
            ?? throw new HaloApiException("Failed to deserialize AssetView.", json);

        return assetView?.Assets is not null ? assetView.Assets.AsReadOnly() : new List<AssetDto>().AsReadOnly();
    }

    /// <summary>
    /// Gets a specific asset by ID.
    /// </summary>
    /// <param name="haloApiClient">The HaloPSA API client.</param>
    /// <param name="assetId">The asset ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The device details.</returns>
    public static async Task<AssetDto> GetAssetAsync(this ApiClient haloApiClient, Int32 assetId, CancellationToken cancellationToken = default)
    {
        var uri = "Asset"
            .AppendPathSegment(assetId)
            .ToUri();

        var json = await haloApiClient.HttpGetAsync(uri, cancellationToken);

        var asset = JsonSerializer.Deserialize(json, AssetJsonContext.Default.AssetDto)
            ?? throw new HaloApiException("Failed to deserialize AssetsList.", json);

        return asset;
    }

    /// <summary>
    /// Creates a new asset in HaloPSA.
    /// </summary>
    /// <param name="haloApiClient">The HaloPSA API client.</param>
    /// <param name="asset">The asset to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created device.</returns>
    public static async Task<AssetDto> CreateAssetAsync(this ApiClient haloApiClient, AssetDto asset, CancellationToken cancellationToken = default)
    {
        var uri = new Uri("Asset", UriKind.Relative);

        var payload = JsonSerializer.Serialize(asset, AssetJsonContext.Default.AssetDto);

        var result = await haloApiClient.HttpPostAsync<AssetDto>(uri, $"[{payload}]", AssetJsonContext.Default.AssetDto, cancellationToken);

        return result;
    }
}
