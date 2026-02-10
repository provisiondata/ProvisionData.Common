// Provision Data Application Framework
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

using System.Net.Http.Json;
using System.Text.Json;

namespace ProvisionData.ResultPattern;

public static partial class ResultExtensions
{
    /// <summary>
    /// Sends a GET request to the specified URI and deserializes the response into a <see cref="Result{TValue}"/>.
    /// </summary>
    /// <typeparam name="T">The type of value expected in the successful result.</typeparam>
    /// <param name="httpClient">The HTTP client to use for the request.</param>
    /// <param name="requestUri">The URI the request is sent to.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a Result object with the
    /// deserialized value if successful; otherwise, a failure result containing error information.</returns>
    public static async Task<Result<T>> GetResultAsync<T>(
        this HttpClient httpClient,
        String requestUri,
        CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetAsync(requestUri, cancellationToken);
        return await response.ResultAsync<T>(cancellationToken);
    }

    /// <summary>
    /// Sends a GET request to the specified URI and deserializes the response into a result containing a read-only collection.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection to deserialize from the response content.</typeparam>
    /// <param name="httpClient">The HTTP client to use for the request.</param>
    /// <param name="requestUri">The URI the request is sent to.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a Result object with a read-only
    /// collection of type T if deserialization is successful; otherwise, a failure result containing error information.</returns>
    public static async Task<Result<IReadOnlyCollection<T>>> GetMultipleResultAsync<T>(
        this HttpClient httpClient,
        String requestUri,
        CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetAsync(requestUri, cancellationToken);
        return await response.MultipleResultAsync<T>(cancellationToken);
    }

    /// <summary>
    /// Sends a POST request with JSON content to the specified URI and deserializes the response into a <see cref="Result{TValue}"/>.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request body to serialize.</typeparam>
    /// <typeparam name="TResponse">The type of value expected in the successful result.</typeparam>
    /// <param name="httpClient">The HTTP client to use for the request.</param>
    /// <param name="requestUri">The URI the request is sent to.</param>
    /// <param name="content">The request body to serialize as JSON.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a Result object with the
    /// deserialized value if successful; otherwise, a failure result containing error information.</returns>
    public static async Task<Result<TResponse>> PostAndReturnResultAsync<TRequest, TResponse>(
        this HttpClient httpClient,
        String requestUri,
        TRequest content,
        CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync(requestUri, content, cancellationToken);
        return await response.ResultAsync<TResponse>(cancellationToken);
    }

    /// <summary>
    /// Sends a POST request with JSON content to the specified URI and deserializes the response into a result containing a read-only collection.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request body to serialize.</typeparam>
    /// <typeparam name="TResponse">The type of elements in the collection to deserialize from the response content.</typeparam>
    /// <param name="httpClient">The HTTP client to use for the request.</param>
    /// <param name="requestUri">The URI the request is sent to.</param>
    /// <param name="content">The request body to serialize as JSON.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a Result object with a read-only
    /// collection of type T if deserialization is successful; otherwise, a failure result containing error information.</returns>
    public static async Task<Result<IReadOnlyCollection<TResponse>>> PostAndReturnMultipleResultAsync<TRequest, TResponse>(
        this HttpClient httpClient,
        String requestUri,
        TRequest content,
        CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync(requestUri, content, cancellationToken);
        return await response.MultipleResultAsync<TResponse>(cancellationToken);
    }

    /// <summary>
    /// Sends a PUT request with JSON content to the specified URI and deserializes the response into a <see cref="Result{TValue}"/>.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request body to serialize.</typeparam>
    /// <typeparam name="TResponse">The type of value expected in the successful result.</typeparam>
    /// <param name="httpClient">The HTTP client to use for the request.</param>
    /// <param name="requestUri">The URI the request is sent to.</param>
    /// <param name="content">The request body to serialize as JSON.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a Result object with the
    /// deserialized value if successful; otherwise, a failure result containing error information.</returns>
    public static async Task<Result<TResponse>> PutAndReturnResultAsync<TRequest, TResponse>(
        this HttpClient httpClient,
        String requestUri,
        TRequest content,
        CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PutAsJsonAsync(requestUri, content, cancellationToken);
        return await response.ResultAsync<TResponse>(cancellationToken);
    }

    /// <summary>
    /// Sends a PUT request with JSON content to the specified URI and deserializes the response into a result containing a read-only collection.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request body to serialize.</typeparam>
    /// <typeparam name="TResponse">The type of elements in the collection to deserialize from the response content.</typeparam>
    /// <param name="httpClient">The HTTP client to use for the request.</param>
    /// <param name="requestUri">The URI the request is sent to.</param>
    /// <param name="content">The request body to serialize as JSON.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a Result object with a read-only
    /// collection of type T if deserialization is successful; otherwise, a failure result containing error information.</returns>
    public static async Task<Result<IReadOnlyCollection<TResponse>>> PutAndReturnMultipleResultAsync<TRequest, TResponse>(
        this HttpClient httpClient,
        String requestUri,
        TRequest content,
        CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PutAsJsonAsync(requestUri, content, cancellationToken);
        return await response.MultipleResultAsync<TResponse>(cancellationToken);
    }

    /// <summary>
    /// Sends a PATCH request with JSON content to the specified URI and deserializes the response into a <see cref="Result{TValue}"/>.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request body to serialize.</typeparam>
    /// <typeparam name="TResponse">The type of value expected in the successful result.</typeparam>
    /// <param name="httpClient">The HTTP client to use for the request.</param>
    /// <param name="requestUri">The URI the request is sent to.</param>
    /// <param name="content">The request body to serialize as JSON.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a Result object with the
    /// deserialized value if successful; otherwise, a failure result containing error information.</returns>
    public static async Task<Result<TResponse>> PatchAndReturnResultAsync<TRequest, TResponse>(
        this HttpClient httpClient,
        String requestUri,
        TRequest content,
        CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PatchAsJsonAsync(requestUri, content, cancellationToken);
        return await response.ResultAsync<TResponse>(cancellationToken);
    }

    /// <summary>
    /// Sends a PATCH request with JSON content to the specified URI and deserializes the response into a result containing a read-only collection.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request body to serialize.</typeparam>
    /// <typeparam name="TResponse">The type of elements in the collection to deserialize from the response content.</typeparam>
    /// <param name="httpClient">The HTTP client to use for the request.</param>
    /// <param name="requestUri">The URI the request is sent to.</param>
    /// <param name="content">The request body to serialize as JSON.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a Result object with a read-only
    /// collection of type T if deserialization is successful; otherwise, a failure result containing error information.</returns>
    public static async Task<Result<IReadOnlyCollection<TResponse>>> PatchAndReturnMultipleResultAsync<TRequest, TResponse>(
        this HttpClient httpClient,
        String requestUri,
        TRequest content,
        CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PatchAsJsonAsync(requestUri, content, cancellationToken);
        return await response.MultipleResultAsync<TResponse>(cancellationToken);
    }

    /// <summary>
    /// Sends a DELETE request to the specified URI and deserializes the response into a <see cref="Result{TValue}"/>.
    /// </summary>
    /// <typeparam name="T">The type of value expected in the successful result.</typeparam>
    /// <param name="httpClient">The HTTP client to use for the request.</param>
    /// <param name="requestUri">The URI the request is sent to.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a Result object with the
    /// deserialized value if successful; otherwise, a failure result containing error information.</returns>
    public static async Task<Result<T>> DeleteAndReturnResultAsync<T>(
        this HttpClient httpClient,
        String requestUri,
        CancellationToken cancellationToken = default)
    {
        var response = await httpClient.DeleteAsync(requestUri, cancellationToken);
        return await response.ResultAsync<T>(cancellationToken);
    }

    /// <summary>
    /// Sends a DELETE request to the specified URI and deserializes the response into a result containing a read-only collection.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection to deserialize from the response content.</typeparam>
    /// <param name="httpClient">The HTTP client to use for the request.</param>
    /// <param name="requestUri">The URI the request is sent to.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a Result object with a read-only
    /// collection of type T if deserialization is successful; otherwise, a failure result containing error information.</returns>
    public static async Task<Result<IReadOnlyCollection<T>>> DeleteAndReturnMultipleResultAsync<T>(
        this HttpClient httpClient,
        String requestUri,
        CancellationToken cancellationToken = default)
    {
        var response = await httpClient.DeleteAsync(requestUri, cancellationToken);
        return await response.MultipleResultAsync<T>(cancellationToken);
    }

    /// <summary>
    /// Returns a <see cref="Result{TValue}"/> with a single value from an HttpResponseMessage
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="response"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<Result<T>> ResultAsync<T>(this HttpResponseMessage response, CancellationToken cancellationToken = default)
    {
        return await response.Content.ReadFromJsonAsync<Result<T>>(cancellationToken)
            ?? await ErrorAsync(response, cancellationToken);
    }

    /// <summary>
    /// Asynchronously deserializes the HTTP response content into a result containing a read-only collection of type T.
    /// </summary>
    /// <remarks>If the response content cannot be deserialized into the expected result type, the method
    /// attempts to extract error information. If neither deserialization succeeds, a generic error result is
    /// returned.</remarks>
    /// <typeparam name="T">The type of elements in the collection to deserialize from the response content.</typeparam>
    /// <param name="response">The HTTP response message whose content will be deserialized.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation. The default value is None.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a Result object with a read-only
    /// collection of type T if deserialization is successful; otherwise, a failure result containing error information.</returns>
    public static async Task<Result<IReadOnlyCollection<T>>> MultipleResultAsync<T>(this HttpResponseMessage response, CancellationToken cancellationToken = default)
    {
        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        var result = JsonSerializer.Deserialize<Result<IReadOnlyCollection<T>>>(json);
        if (result is not null)
        {
            return result;
        }

        var error = JsonSerializer.Deserialize<Result>(json);
        if (error is not null)
        {
            return Result<IReadOnlyCollection<T>>.Failure(error.Error);
        }

        return new ApiError("The server response was not understood");
    }

    private static async Task<Error> ErrorAsync(HttpResponseMessage response, CancellationToken cancellationToken = default)
    {
        var json = await response.Content.ReadAsStringAsync(cancellationToken);

        try
        {
            var result = JsonSerializer.Deserialize<Result>(json);
            if (result is not null)
            {
                return result.Error;
            }

            return new ApiError(FormatErrorMessage(response));
        }
        catch (Exception ex)
        {
            return new ApiError(FormatErrorMessage(response));
        }
    }

    private static String FormatErrorMessage(HttpResponseMessage response)
    {
        return $"Request to {response.RequestMessage?.RequestUri} failed with {response.StatusCode}: {response.RequestMessage}.";
    }
}
