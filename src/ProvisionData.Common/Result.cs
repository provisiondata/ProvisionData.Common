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

using System.Diagnostics.CodeAnalysis;

namespace ProvisionData;

/// <summary>
/// Represents the result of an operation, either successful or failed.
/// </summary>
public class Result
{
    /// <summary>
    /// Gets a value indicating whether the operation was successful.
    /// </summary>
    public Boolean IsSuccess { get; }

    /// <summary>
    /// Gets a value indicating whether the operation failed.
    /// </summary>
    public Boolean IsFailure => !IsSuccess;

    /// <summary>
    /// Gets the error associated with a failed result.
    /// </summary>
    public Error Error { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class.
    /// </summary>
    /// <param name="isSuccess">Indicates whether the operation was successful.</param>
    /// <param name="error">The error associated with the result. Must be <see cref="Error.None"/> for successful results.</param>
    /// <exception cref="ArgumentException">Thrown when the success state and error state are inconsistent.</exception>
    protected Result(Boolean isSuccess, Error error)
    {
        if (isSuccess && error != Error.None)
            throw new ArgumentException("Success result cannot have an error", nameof(error));
        if (!isSuccess && error == Error.None)
            throw new ArgumentException("Failure result must have an error", nameof(error));

        IsSuccess = isSuccess;
        Error = error;
    }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <returns>A successful <see cref="Result"/>.</returns>
    public static Result Success() => new(true, Error.None);

    /// <summary>
    /// Creates a successful result containing the specified value.
    /// </summary>
    /// <typeparam name="TValue">The type of the value to be stored in the result.</typeparam>
    /// <param name="value">The value to include in the successful result. Can be null for reference types.</param>
    /// <returns>A <see cref="Result{TValue}"/> representing a successful operation with the provided value.</returns>
    public static Result<TValue> Success<TValue>(TValue value) => Result<TValue>.Success(value);

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
}

/// <summary>
/// Represents the result of an operation that returns a value of type <typeparamref name="TValue"/>, either successful or failed.
/// </summary>
/// <typeparam name="TValue">The type of value returned by a successful operation.</typeparam>
public class Result<TValue> : Result
{
    private readonly TValue? _value;

    /// <summary>
    /// Gets the value returned by a successful operation.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when accessing the value of a failed result.</exception>
    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Cannot access value of a failed result");

    /// <summary>
    /// Initializes a new successful instance of the <see cref="Result{TValue}"/> class.
    /// </summary>
    /// <param name="value">The value of the successful result.</param>
    private Result(TValue value) : base(true, Error.None)
    {
        _value = value;
    }

    /// <summary>
    /// Initializes a new failed instance of the <see cref="Result{TValue}"/> class.
    /// </summary>
    /// <param name="error">The error of the failed result.</param>
    private Result(Error error) : base(false, error)
    {
        _value = default;
    }

    /// <summary>
    /// Creates a successful result with the specified value.
    /// </summary>
    /// <param name="value">The value of the successful result.</param>
    /// <returns>A successful <see cref="Result{TValue}"/>.</returns>
    public static Result<TValue> Success(TValue value) => new(value);

    /// <summary>
    /// Creates a failed result with the specified error.
    /// </summary>
    /// <param name="error">The error for the failed result.</param>
    /// <returns>A failed <see cref="Result{TValue}"/>.</returns>
    public new static Result<TValue> Failure(Error error) => new(error);

    /// <summary>
    /// Implicit conversion from a value to a successful <see cref="Result{TValue}"/>.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    public static implicit operator Result<TValue>(TValue value) => Success(value);

    /// <summary>
    /// Implicit conversion from an <see cref="Error"/> to a failed <see cref="Result{TValue}"/>.
    /// </summary>
    /// <param name="error">The error to convert.</param>
    public static implicit operator Result<TValue>(Error error) => Failure(error);
}

/// <summary>
/// Represents the result of a validation operation containing zero or more errors.
/// </summary>
public class ValidationResult : Result
{
    /// <summary>
    /// Gets the collection of errors from the validation operation.
    /// </summary>
    public IReadOnlyList<Error> Errors { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationResult"/> class.
    /// </summary>
    /// <param name="errors">The validation errors.</param>
    [SuppressMessage("Performance", "CA1826:Do not use Enumerable methods on indexable collections", Justification = "<Pending>")]
    private ValidationResult(IReadOnlyList<Error> errors)
        : base(errors.Count == 0, errors.FirstOrDefault() ?? Error.None)
    {
        Errors = errors;
    }

    /// <summary>
    /// Creates a validation result with the specified errors.
    /// </summary>
    /// <param name="errors">The validation errors.</param>
    /// <returns>A <see cref="ValidationResult"/> containing the specified errors.</returns>
    public static ValidationResult WithErrors(params Error[] errors)
        => new(errors);

    /// <summary>
    /// Creates a successful validation result with no errors.
    /// </summary>
    /// <returns>A successful <see cref="ValidationResult"/>.</returns>
    public static ValidationResult Ok() => new([]);
}
