// ProvisionData.Common
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

namespace ProvisionData;

/// <summary>
/// Extension methods for working with <see cref="Result"/> and <see cref="Result{T}"/> types.
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Transforms the value of a successful result using the specified mapping function.
    /// </summary>
    /// <typeparam name="TIn">The type of the input result value.</typeparam>
    /// <typeparam name="TOut">The type of the output result value.</typeparam>
    /// <param name="result">The result to transform.</param>
    /// <param name="mapper">A function to transform the value.</param>
    /// <returns>A successful result with the transformed value, or a failed result with the original error.</returns>
    public static Result<TOut> Map<TIn, TOut>(
        this Result<TIn> result,
        Func<TIn, TOut> mapper)
    {
        return result.IsSuccess
            ? Result<TOut>.Success(mapper(result.Value))
            : Result<TOut>.Failure(result.Error);
    }

    /// <summary>
    /// Chains operations that return results, allowing for sequential composition of result-returning functions.
    /// </summary>
    /// <typeparam name="TIn">The type of the input result value.</typeparam>
    /// <typeparam name="TOut">The type of the output result value.</typeparam>
    /// <param name="result">The result to bind.</param>
    /// <param name="binder">A function that takes a value and returns a result.</param>
    /// <returns>The result returned by the binder function, or a failed result with the original error.</returns>
    /// <example>
    /// <code>
    /// public Result&lt;OrderConfirmation&gt; ProcessOrder(CreateOrderRequest request)
    /// {
    ///     return ValidateOrder(request)
    ///         .Bind(order =&gt; CheckInventory(order))
    ///         .Bind(order =&gt; ProcessPayment(order))
    ///         .Bind(order =&gt; CreateShipment(order))
    ///         .Map(shipment =&gt; new OrderConfirmation(shipment.TrackingNumber));
    /// }
    /// </code>
    /// </example>
    public static Result<TOut> Bind<TIn, TOut>(
        this Result<TIn> result,
        Func<TIn, Result<TOut>> binder)
    {
        return result.IsSuccess
            ? binder(result.Value)
            : Result<TOut>.Failure(result.Error);
    }

    /// <summary>
    /// Handles both success and failure cases of a result, returning a single value.
    /// </summary>
    /// <typeparam name="TIn">The type of the result value.</typeparam>
    /// <typeparam name="TOut">The type of the return value.</typeparam>
    /// <param name="result">The result to match on.</param>
    /// <param name="onSuccess">A function to handle the success case.</param>
    /// <param name="onFailure">A function to handle the failure case.</param>
    /// <returns>The value returned by either the success or failure handler.</returns>
    public static TOut Match<TIn, TOut>(
        this Result<TIn> result,
        Func<TIn, TOut> onSuccess,
        Func<Error, TOut> onFailure)
    {
        return result.IsSuccess
            ? onSuccess(result.Value)
            : onFailure(result.Error);
    }

    /// <summary>
    /// Executes an action on a successful result without transforming the value.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="result">The result to tap into.</param>
    /// <param name="action">An action to execute if the result is successful.</param>
    /// <returns>The original result, allowing for method chaining.</returns>
    public static Result<T> Tap<T>(
        this Result<T> result,
        Action<T> action)
    {
        if (result.IsSuccess)
            action(result.Value);
        return result;
    }

    /// <summary>
    /// Converts a result to a typed result with the specified value.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="result">The result to convert.</param>
    /// <param name="value">The value for the successful result.</param>
    /// <returns>A successful <see cref="Result{T}"/> with the specified value, or a failed result with the original error.</returns>
    public static Result<T> ToResult<T>(this Result result, T value)
    {
        return result.IsSuccess
            ? Result<T>.Success(value)
            : Result<T>.Failure(result.Error);
    }

    /// <summary>
    /// Gets the value from a successful result or throws an exception if the result failed.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="result">The result to extract the value from.</param>
    /// <returns>The value of a successful result.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the result is failed.</exception>
    public static T GetValueOrThrow<T>(this Result<T> result)
    {
        if (result.IsFailure)
            throw new InvalidOperationException($"Result failed: {result.Error.Description}");
        return result.Value;
    }
}

