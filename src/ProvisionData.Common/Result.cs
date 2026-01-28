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

public class Result
{
    public Boolean IsSuccess { get; }
    public Boolean IsFailure => !IsSuccess;
    public Error Error { get; }

    protected Result(Boolean isSuccess, Error error)
    {
        if (isSuccess && error != Error.None)
            throw new ArgumentException("Success result cannot have an error", nameof(error));
        if (!isSuccess && error == Error.None)
            throw new ArgumentException("Failure result must have an error", nameof(error));

        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, Error.None);
    public static Result Failure(Error error) => new(false, error);

    // Implicit conversion from Error to Result
    public static implicit operator Result(Error error) => Failure(error);
}

public class Result<TValue> : Result
{
    private readonly TValue? _value;

    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Cannot access value of a failed result");

    private Result(TValue value) : base(true, Error.None)
    {
        _value = value;
    }

    private Result(Error error) : base(false, error)
    {
        _value = default;
    }

    public static Result<TValue> Success(TValue value) => new(value);
    public new static Result<TValue> Failure(Error error) => new(error);

    // Implicit conversions for cleaner syntax
    public static implicit operator Result<TValue>(TValue value) => Success(value);
    public static implicit operator Result<TValue>(Error error) => Failure(error);
}

public class ValidationResult : Result
{
    public IReadOnlyList<Error> Errors { get; }

    [SuppressMessage("Performance", "CA1826:Do not use Enumerable methods on indexable collections", Justification = "<Pending>")]
    private ValidationResult(IReadOnlyList<Error> errors)
        : base(errors.Count == 0, errors.FirstOrDefault() ?? Error.None)
    {
        Errors = errors;
    }

    public static ValidationResult WithErrors(params Error[] errors)
        => new(errors);

    public static ValidationResult Ok() => new([]);
}
