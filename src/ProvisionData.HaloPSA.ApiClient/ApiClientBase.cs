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
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json.Serialization.Metadata;

namespace ProvisionData.HaloPSA;

/// <summary>
/// Base class for HaloPSA API clients providing HTTP operations and authentication.
/// </summary>
public abstract class ApiClientBase(
    HttpClient httpClient,
    HaloPsaApiClientOptions options,
    IAuthTokenProvider tokenRepository,
    TimeProvider timeProvider,
    ILogger logger,
    IFieldMappingProvider fieldMappingProvider)
{
    // private AuthToken? _token;
    private readonly IAuthTokenProvider _tokenRepository = tokenRepository;
    private readonly IFieldMappingProvider _fieldMappingProvider = fieldMappingProvider;

    /// <summary>
    /// Gets the underlying HTTP client used to send requests to remote servers.
    /// </summary>
    protected HttpClient HttpClient { get; } = httpClient;

    /// <summary>
    /// Gets the configuration options for the HaloPSA API client.
    /// </summary>
    protected HaloPsaApiClientOptions Options { get; } = options;

    /// <summary>
    /// Gets the time provider used to supply time-related functionality for the class.
    /// </summary>
    protected TimeProvider TimeProvider { get; } = timeProvider;

    /// <summary>
    /// Gets the logger used for logging within the API client.
    /// </summary>
    public ILogger Logger { get; } = logger;

    /// <summary>
    /// Ensures that custom fields in the specified DTO are mapped using the field mapping provider.
    /// </summary>
    /// <typeparam name="T">The DTO type that implements <see cref="IHasCustomFields"/>.</typeparam>
    /// <param name="dto">The DTO instance to map fields for.</param>
    /// <returns>The same DTO instance with custom fields mapped.</returns>
    /// <remarks>
    /// If fields are not already mapped (as indicated by <see cref="IHasCustomFields.FieldsAreMapped"/>), 
    /// this method applies the field mapping from the field mapping provider.
    /// </remarks>
    protected T EnsureAssetFieldsMapped<T>(T dto)
        where T : IHasCustomFields
    {
        if (!dto.FieldsAreMapped)
        {
            dto.ApplyFieldMap(_fieldMappingProvider);
        }

        return dto;
    }

    private async Task EnsureAuthorizedAsync(CancellationToken cancellationToken)
    {
        var token = await _tokenRepository.GetTokenAsync(cancellationToken)
            ?? throw new HaloApiException("Failed to obtain authentication token.");

        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
    }

    /// <summary>
    /// Sends an HTTP GET request to the specified URI and deserializes the response to the specified type.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the response into. Must implement <see cref="IHasCustomFields"/> for field mapping support.</typeparam>
    /// <param name="uri">The URI to send the GET request to.</param>
    /// <param name="context">The JSON type information for deserialization.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>The deserialized response of type <typeparamref name="T"/>.</returns>
    /// <exception cref="HaloApiException">Thrown when the request fails, the response cannot be deserialized, or authentication fails.</exception>
    public async Task<T> HttpGetAsync<T>(Uri uri, JsonTypeInfo<T> context, CancellationToken cancellationToken = default)
    {
        var json = await HttpGetAsync(uri, cancellationToken);
        try
        {
            var result = System.Text.Json.JsonSerializer.Deserialize<T>(json, context)
                ?? throw new HaloApiException($"Failed to deserialize response from: {uri}", json);

            if (result is IHasCustomFields hasCustomFields)
            {
                EnsureAssetFieldsMapped(hasCustomFields);
            }

            return result;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to deserialize response from: {URI}", uri);
            throw new HaloApiException($"Failed to deserialize response from: {uri}", json, ex);
        }
    }

    /// <summary>
    /// Sends an HTTP GET request to the specified URI and returns the response as a JSON string.
    /// </summary>
    /// <remarks>
    /// This method includes automatic retry logic for HTTP 429 (Too Many Requests) responses, retrying after 60 seconds.
    /// It will retry up to the maximum number of attempts specified in <see cref="HaloPsaApiClientOptions.MaxRetries"/>.
    /// </remarks>
    /// <param name="uri">The URI to send the GET request to.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>The response body as a JSON string.</returns>
    /// <exception cref="HaloApiException">Thrown when the request fails, authentication fails, or maximum retry attempts are reached.</exception>
    public async Task<String> HttpGetAsync(Uri uri, CancellationToken cancellationToken = default)
    {
        await EnsureAuthorizedAsync(cancellationToken);

        var retryCount = 0;
        while (true)
        {
            try
            {
                if (retryCount >= Options.MaxRetries)
                {
                    throw new HaloApiException($"Maximum retry attempts reached for API call: {uri}");
                }

                retryCount++;
                var response = await HttpClient.GetAsync(uri, cancellationToken);
                if (response.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    // Retry after the specified number of seconds
                    const Int32 retryAfterSeconds = 60;

                    await Task.Delay(retryAfterSeconds * 1000, cancellationToken);

                    continue;
                }

                var json = await response.Content.ReadAsStringAsync(cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    return json;
                }

                Logger.LogError("Failed to GET from {uri} => {HttpStatus}: {HttpMessage}\n\tResponse: {response}",
                    uri, response.StatusCode, response.ReasonPhrase, json);

                response.EnsureSuccessStatusCode();

                return json;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "An error occured while calling: {URI}", uri);
                throw new HaloApiException($"API call failed: {uri}", ex);
            }
        }
    }

    /// <summary>
    /// Sends an HTTP POST request with the specified payload to the given URI and deserializes the response.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the response into. Must implement <see cref="IHasCustomFields"/> for field mapping support.</typeparam>
    /// <param name="uri">The URI to send the POST request to.</param>
    /// <param name="payload">The JSON payload to send in the request body.</param>
    /// <param name="context">The JSON type information for deserialization.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>The deserialized response of type <typeparamref name="T"/>.</returns>
    /// <exception cref="HaloApiException">Thrown when the request fails, the response cannot be deserialized, or authentication fails.</exception>
    public async Task<T> HttpPostAsync<T>(Uri uri, String payload, JsonTypeInfo<T> context, CancellationToken cancellationToken = default)
    {
        var json = await HttpPostAsync(uri, payload, cancellationToken);
        try
        {
            var result = System.Text.Json.JsonSerializer.Deserialize<T>(json, context)
                ?? throw new HaloApiException($"Failed to deserialize response from: {uri}", json);

            if (result is IHasCustomFields hasCustomFields)
            {
                EnsureAssetFieldsMapped(hasCustomFields);
            }

            return result;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to deserialize response from: {URI}", uri);
            throw new HaloApiException($"Failed to deserialize response from: {uri}", json, ex);
        }
    }

    /// <summary>
    /// Sends an HTTP POST request with the specified payload to the given URI and returns the response as a JSON string.
    /// </summary>
    /// <param name="uri">The URI to send the POST request to.</param>
    /// <param name="payload">The JSON payload to send in the request body.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>The response body as a JSON string.</returns>
    /// <exception cref="HaloApiException">Thrown when the request fails or authentication fails.</exception>
    public async Task<String> HttpPostAsync(Uri uri, String payload, CancellationToken cancellationToken = default)
    {
        await EnsureAuthorizedAsync(cancellationToken);

        Logger.LogTrace("HttpPostAsync: {api} => {payload}", uri, payload);

        var response = await HttpClient.PostAsync(uri, new StringContent(payload, System.Text.Encoding.UTF8, "application/json"), cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorResponse = await response.Content.ReadAsStringAsync(cancellationToken);

            Logger.LogError("Failed to POST to {uri} => {HttpStatus}: {HttpMessage}\n\tRequest: {request}\n\tResponse: {response}",
                uri, response.StatusCode, response.ReasonPhrase, payload, errorResponse);

            throw new HaloApiException($"Failed to POST to {uri} => {response.StatusCode}: {response.ReasonPhrase}", payload);
        }

        return await response.Content.ReadAsStringAsync(cancellationToken);
    }

    /// <summary>
    /// Sends an HTTP PUT request with the specified payload to the given URI and deserializes the response.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the response into. Must implement <see cref="IHasCustomFields"/> for field mapping support.</typeparam>
    /// <param name="uri">The URI to send the PUT request to.</param>
    /// <param name="payload">The JSON payload to send in the request body.</param>
    /// <param name="context">The JSON type information for deserialization.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>The deserialized response of type <typeparamref name="T"/>.</returns>
    /// <exception cref="HaloApiException">Thrown when the request fails, the response cannot be deserialized, or authentication fails.</exception>
    public async Task<T> HttpPutAsync<T>(Uri uri, String payload, JsonTypeInfo<T> context, CancellationToken cancellationToken = default)
    {
        var json = await HttpPutAsync(uri, payload, cancellationToken);
        try
        {
            var result = System.Text.Json.JsonSerializer.Deserialize<T>(json, context)
                ?? throw new HaloApiException($"Failed to deserialize response from: {uri}", json);

            if (result is IHasCustomFields hasCustomFields)
            {
                EnsureAssetFieldsMapped(hasCustomFields);
            }

            return result;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to deserialize response from: {URI}", uri);
            throw new HaloApiException($"Failed to deserialize response from: {uri}", json, ex);
        }
    }

    /// <summary>
    /// Sends an HTTP PUT request with the specified payload to the given URI and returns the response as a JSON string.
    /// </summary>
    /// <param name="uri">The URI to send the PUT request to.</param>
    /// <param name="payload">The JSON payload to send in the request body.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>The response body as a JSON string.</returns>
    /// <exception cref="HaloApiException">Thrown when the request fails or authentication fails.</exception>
    public async Task<String> HttpPutAsync(Uri uri, String payload, CancellationToken cancellationToken = default)
    {
        await EnsureAuthorizedAsync(cancellationToken);

        var response = await HttpClient.PutAsync(uri, new StringContent(payload, System.Text.Encoding.UTF8, "application/json"), cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorResponse = await response.Content.ReadAsStringAsync(cancellationToken);

            Logger.LogError("Failed to PUT to {uri} => {HttpStatus}: {HttpMessage}\n\tRequest: {request}\n\tResponse: {response}",
                uri, response.StatusCode, response.ReasonPhrase, payload, errorResponse);

            throw new HaloApiException($"Failed to PUT to {uri} => {response.StatusCode}: {response.ReasonPhrase}", payload);
        }

        return await response.Content.ReadAsStringAsync(cancellationToken);
    }

    /// <summary>
    /// Sends an HTTP GET request for a paginated endpoint using the default page size from options.
    /// </summary>
    /// <param name="uri">The base URI to send the GET request to.</param>
    /// <param name="pagenumber">The page number to retrieve (1-based).</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>The response body as a JSON string.</returns>
    /// <exception cref="HaloApiException">Thrown when the request fails or authentication fails.</exception>
    public async Task<String> HttpGetPagedAsync(Uri uri, Int32 pagenumber, CancellationToken cancellationToken = default)
        => await HttpGetPagedAsync(uri, pagenumber, Options.PageSize, cancellationToken);

    /// <summary>
    /// Sends an HTTP GET request for a paginated endpoint with a specified page size.
    /// </summary>
    /// <remarks>
    /// The method appends pagination query parameters to the URI: pageinate, page_no, and page_size.
    /// </remarks>
    /// <param name="uri">The base URI to send the GET request to.</param>
    /// <param name="pagenumber">The page number to retrieve (1-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>The response body as a JSON string.</returns>
    /// <exception cref="HaloApiException">Thrown when the request fails or authentication fails.</exception>
    public async Task<String> HttpGetPagedAsync(Uri uri, Int32 pagenumber, Int32 pageSize, CancellationToken cancellationToken = default)
    {
        await EnsureAuthorizedAsync(cancellationToken);

        var url = uri.AppendQueryParam("pageinate", true)
                     .AppendQueryParam("page_no", pagenumber)
                     .AppendQueryParam("page_size", pageSize)
                     .ToUri();

        return await HttpGetAsync(url, cancellationToken);
    }
}
