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

using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace ProvisionData.ResultPattern;

/// <summary>
/// Represents the result of an operation, either successful or failed.
/// </summary>
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
public class Result
{
    /// <summary>
    /// Gets a special <see cref="Error"/> instance representing no error.
    /// </summary>
    public static readonly Error None = new(NoneErrorCode.Instance, "None");

    /// <summary>
    /// Gets a value indicating whether the operation was successful.
    /// </summary>
    public Boolean IsSuccess { get; init; }

    /// <summary>
    /// Gets the error associated with a failed result.
    /// </summary>
    public Error Error { get; init; } = None;

    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class.
    /// </summary>
    /// <param name="isSuccess">Indicates whether the operation was successful.</param>
    /// <param name="error">The error associated with the result. Must be <see cref="Result.None"/> for successful results.</param>
    /// <exception cref="ArgumentException">Thrown when the success state and error state are inconsistent.</exception>
    [JsonConstructor]
    protected Result(Boolean isSuccess, Error error)
    {
        if (isSuccess && error != None)
        {
            throw new ArgumentException("Success result cannot have an error", nameof(error));
        }

        if (!isSuccess && error == None)
        {
            throw new ArgumentException("Failure result must have an error", nameof(error));
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <returns>A successful <see cref="Result"/>.</returns>
    public static Result Success() => new(true, None);

    /// <summary>
    /// Creates a successful result containing the specified value.
    /// </summary>
    /// <typeparam name="TDto">The type of the value to be stored in the result.</typeparam>
    /// <param name="value">The value to include in the successful result. Can be null for reference types.</param>
    /// <returns>A <see cref="Result{TValue}"/> representing a successful operation with the provided value.</returns>
    public static Result<TDto> Success<TDto>(TDto value) => Result<TDto>.Success(value);

    /// <summary>
    /// Creates a failed result with the specified error.
    /// </summary>
    /// <param name="error">The error for the failed result.</param>
    /// <returns>A failed <see cref="Result"/>.</returns>
    public static Result Failure(Error error) => new(false, error);

    /// <summary>
    /// Implicit conversion from <see cref="Error"/> to <see cref="Result"/>.
    /// </summary>
    /// <param name="error">The error to convert.</param>
    public static implicit operator Result(Error error) => Failure(error);

    internal sealed class NoneErrorCode : ErrorCode
    {
        public static readonly NoneErrorCode Instance = new();
        protected override String Name => "None";
    }
}

/// <summary>
/// Represents the result of an operation that returns a value of type <typeparamref name="TDto"/>, either successful or failed.
/// </summary>
/// <typeparam name="TDto">The type of value returned by a successful operation.</typeparam>
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
public class Result<TDto> : Result
{
    /// <summary>
    /// Gets or sets the value for JSON serialization.
    /// </summary>
    [JsonPropertyName("value")]
    [JsonInclude]
    protected TDto? ValueStorage { get; init; }

    /// <summary>
    /// Gets the value returned by a successful operation.
    /// </summary>
    [JsonIgnore]
    public TDto Value => IsSuccess
        ? ValueStorage!
        : default!;

    /// <summary>
    /// Initializes a new successful instance of the <see cref="Result{TValue}"/> class.
    /// </summary>
    /// <param name="isSuccess">Indicates whether the operation was successful.</param>
    /// <param name="value">The value of the successful result.</param>
    /// <param name="error">The error of the failed result. Must be <see cref="Result.None"/> for successful results.</param>
    [JsonConstructor]
    private Result(Boolean isSuccess, TDto value, Error error) : base(isSuccess, error)
    {
        ValueStorage = value;
    }

    ///// <summary>
    ///// Initializes a new failed instance of the <see cref="Result{TDto}"/> class.
    ///// </summary>
    ///// <param name="error">The error of the failed result.</param>
    //private Result(Error error) : base(false, error)
    //{
    //    ValueStorage = default;
    //}

    /// <summary>
    /// Creates a successful result with the specified value.
    /// </summary>
    /// <param name="value">The value of the successful result.</param>
    /// <returns>A successful <see cref="Result{TValue}"/>.</returns>
    public static Result<TDto> Success(TDto value) => new(true, value, None);

    /// <summary>
    /// Creates a failed result with the specified error.
    /// </summary>
    /// <param name="error">The error for the failed result.</param>
    /// <returns>A failed <see cref="Result{TValue}"/>.</returns>
    public static new Result<TDto> Failure(Error error) => new(false, default!, error);

    /// <summary>
    /// Implicit conversion from a value to a successful <see cref="Result{TValue}"/>.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    public static implicit operator Result<TDto>(TDto value) => Success(value);

    /// <summary>
    /// Implicit conversion from an <see cref="Error"/> to a failed <see cref="Result{TValue}"/>.
    /// </summary>
    /// <param name="error">The error to convert.</param>
    public static implicit operator Result<TDto>(Error error) => Failure(error);
}
