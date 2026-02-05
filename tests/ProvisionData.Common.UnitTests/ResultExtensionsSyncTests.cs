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

using FluentAssertions;
using Xunit;

namespace ProvisionData.UnitTests;

/// <summary>
/// Unit tests for the <see cref="ResultExtensions"/> synchronous methods.
/// </summary>
public class ResultExtensionsSyncTests
{
    [Fact]
    public void GetValueOrDefault_WithSuccessResult_ShouldReturnValue()
    {
        var result = Result<Int32>.Success(42);

        var value = result.GetValueOrDefault(0);

        value.Should().Be(42);
    }

    [Fact]
    public void GetValueOrDefault_WithFailureResult_ShouldReturnDefaultValue()
    {
        var result = Result<Int32>.Failure(Error.NotFound("Not found"));

        var value = result.GetValueOrDefault(99);

        value.Should().Be(99);
    }

    [Fact]
    public void GetValueOrDefault_WithSuccessResultNoParameter_ShouldReturnValue()
    {
        var result = Result<Int32>.Success(42);

        var value = result.GetValueOrDefault();

        value.Should().Be(42);
    }

    [Fact]
    public void GetValueOrDefault_WithFailureResultNoParameter_ShouldReturnTypeDefault()
    {
        var result = Result<Int32>.Failure(Error.NotFound("Not found"));

        var value = result.GetValueOrDefault();

        value.Should().Be(0);
    }

    [Fact]
    public void GetValueOrDefault_WithReferenceType_ShouldReturnNull()
    {
        var result = Result<String>.Failure(Error.NotFound("Not found"));

        var value = result.GetValueOrDefault();

        value.Should().BeNull();
    }

    [Fact]
    public void GetValueOrDefault_WithReferenceTypeAndDefault_ShouldReturnDefault()
    {
        var result = Result<String>.Failure(Error.NotFound("Not found"));

        var value = result.GetValueOrDefault("default value");

        value.Should().Be("default value");
    }

    [Fact]
    public void Map_WithSuccessResult_ShouldTransformValue()
    {
        var result = Result<Int32>.Success(5);

        var mapped = result.Map(x => x * 2);

        mapped.IsSuccess.Should().BeTrue();
        mapped.Value.Should().Be(10);
    }

    [Fact]
    public void Map_WithFailureResult_ShouldReturnFailure()
    {
        var result = Result<Int32>.Failure(Error.NotFound("Not found"));

        var mapped = result.Map(x => x * 2);

        mapped.IsFailure.Should().BeTrue();
        mapped.Error.Should().BeOfType<NotFoundError>();
    }

    [Fact]
    public void Bind_WithSuccessResult_ShouldChainOperation()
    {
        var result = Result<Int32>.Success(5);

        var bound = result.Bind(x => 
            x > 0 ? Result<String>.Success(x.ToString()) : Error.Validation("Must be positive"));

        bound.IsSuccess.Should().BeTrue();
        bound.Value.Should().Be("5");
    }

    [Fact]
    public void Bind_WithFailureInBinder_ShouldReturnBinderFailure()
    {
        var result = Result<Int32>.Success(-5);

        var bound = result.Bind(x => 
            x > 0 ? Result<String>.Success(x.ToString()) : Error.Validation("Must be positive"));

        bound.IsFailure.Should().BeTrue();
        bound.Error.Should().BeOfType<ValidationError>();
    }

    [Fact]
    public void Bind_WithFailureResult_ShouldReturnFailure()
    {
        var result = Result<Int32>.Failure(Error.NotFound("Not found"));

        var bound = result.Bind(x => Result<String>.Success(x.ToString()));

        bound.IsFailure.Should().BeTrue();
        bound.Error.Should().BeOfType<NotFoundError>();
    }

    [Fact]
    public void Match_WithSuccessResult_ShouldCallOnSuccess()
    {
        var result = Result<Int32>.Success(5);

        var message = result.Match(
            x => $"Success: {x}",
            error => $"Error: {error.Description}");

        message.Should().Be("Success: 5");
    }

    [Fact]
    public void Match_WithFailureResult_ShouldCallOnFailure()
    {
        var result = Result<Int32>.Failure(Error.NotFound("Item not found"));

        var message = result.Match(
            x => $"Success: {x}",
            error => $"Error: {error.Description}");

        message.Should().Be("Error: Item not found");
    }

    [Fact]
    public void Tap_WithSuccessResult_ShouldExecuteAction()
    {
        var result = Result<Int32>.Success(5);
        var sideEffect = 0;

        var tapped = result.Tap(x => sideEffect = x * 2);

        tapped.IsSuccess.Should().BeTrue();
        tapped.Value.Should().Be(5);
        sideEffect.Should().Be(10);
    }

    [Fact]
    public void Tap_WithFailureResult_ShouldNotExecuteAction()
    {
        var result = Result<Int32>.Failure(Error.NotFound("Not found"));
        var sideEffect = 0;

        var tapped = result.Tap(x => sideEffect = x * 2);

        tapped.IsFailure.Should().BeTrue();
        sideEffect.Should().Be(0);
    }

    [Fact]
    public void ToResult_WithSuccessResult_ShouldCreateTypedResult()
    {
        var result = Result.Success();

        var typedResult = result.ToResult(42);

        typedResult.IsSuccess.Should().BeTrue();
        typedResult.Value.Should().Be(42);
    }

    [Fact]
    public void ToResult_WithFailureResult_ShouldReturnFailure()
    {
        var result = Result.Failure(Error.NotFound("Not found"));

        var typedResult = result.ToResult(42);

        typedResult.IsFailure.Should().BeTrue();
        typedResult.Error.Should().BeOfType<NotFoundError>();
    }

    [Fact]
    public void Chaining_ShouldWorkCorrectly()
    {
        var result = Result<Int32>.Success(5)
            .Map(x => x * 2)
            .Bind(x => x > 5 ? Result<String>.Success(x.ToString()) : Error.Validation("Too small"))
            .Tap(x => Console.WriteLine(x));

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("10");
    }
}
