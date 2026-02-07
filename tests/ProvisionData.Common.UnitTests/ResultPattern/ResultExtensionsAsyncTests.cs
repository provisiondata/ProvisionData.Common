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

using FluentAssertions;
using Xunit;

namespace ProvisionData.UnitTests.ResultPattern;

/// <summary>
/// Unit tests for the <see cref="ResultExtensions"/> async methods.
/// </summary>
public class ResultExtensionsAsyncTests
{
    [Fact]
    public async Task MapAsync_WithSuccessResult_ShouldTransformValue()
    {
        var result = Result<Int32>.Success(5);

        var mapped = await result.MapAsync(async x =>
        {
            await Task.Delay(1);
            return x * 2;
        });

        mapped.IsSuccess.Should().BeTrue();
        mapped.Value.Should().Be(10);
    }

    [Fact]
    public async Task MapAsync_WithFailureResult_ShouldReturnFailure()
    {
        var result = Result<Int32>.Failure(Error.NotFound("Not found"));

        var mapped = await result.MapAsync(async x =>
        {
            await Task.Delay(1);
            return x * 2;
        });

        mapped.IsFailure.Should().BeTrue();
        mapped.Error.Should().BeOfType<NotFoundError>();
    }

    [Fact]
    public async Task MapAsync_WithTask_ShouldTransformValue()
    {
        var resultTask = Task.FromResult(Result<Int32>.Success(5));

        var mapped = await resultTask.MapAsync(x => x * 2);

        mapped.IsSuccess.Should().BeTrue();
        mapped.Value.Should().Be(10);
    }

    [Fact]
    public async Task MapAsync_WithTaskAndAsyncMapper_ShouldTransformValue()
    {
        var resultTask = Task.FromResult(Result<Int32>.Success(5));

        var mapped = await resultTask.MapAsync(async x =>
        {
            await Task.Delay(1);
            return x * 2;
        });

        mapped.IsSuccess.Should().BeTrue();
        mapped.Value.Should().Be(10);
    }

    [Fact]
    public async Task BindAsync_WithSuccessResult_ShouldChainOperation()
    {
        var result = Result<Int32>.Success(5);

        var bound = await result.BindAsync(async x =>
        {
            await Task.Delay(1);
            return x > 0 ? Result<String>.Success(x.ToString()) : Error.Validation("Must be positive");
        });

        bound.IsSuccess.Should().BeTrue();
        bound.Value.Should().Be("5");
    }

    [Fact]
    public async Task BindAsync_WithFailureResult_ShouldReturnFailure()
    {
        var result = Result<Int32>.Failure(Error.NotFound("Not found"));

        var bound = await result.BindAsync(async x =>
        {
            await Task.Delay(1);
            return Result<String>.Success(x.ToString());
        });

        bound.IsFailure.Should().BeTrue();
        bound.Error.Should().BeOfType<NotFoundError>();
    }

    [Fact]
    public async Task BindAsync_WithTask_ShouldChainOperation()
    {
        var resultTask = Task.FromResult(Result<Int32>.Success(5));

        var bound = await resultTask.BindAsync(x =>
            x > 0 ? Result<String>.Success(x.ToString()) : Error.Validation("Must be positive"));

        bound.IsSuccess.Should().BeTrue();
        bound.Value.Should().Be("5");
    }

    [Fact]
    public async Task BindAsync_WithTaskAndAsyncBinder_ShouldChainOperation()
    {
        var resultTask = Task.FromResult(Result<Int32>.Success(5));

        var bound = await resultTask.BindAsync(async x =>
        {
            await Task.Delay(1);
            return x > 0 ? Result<String>.Success(x.ToString()) : Error.Validation("Must be positive");
        });

        bound.IsSuccess.Should().BeTrue();
        bound.Value.Should().Be("5");
    }

    [Fact]
    public async Task MatchAsync_WithSuccessResult_ShouldCallOnSuccess()
    {
        var result = Result<Int32>.Success(5);

        var message = await result.MatchAsync(
            async x =>
            {
                await Task.Delay(1);
                return $"Success: {x}";
            },
            async error =>
            {
                await Task.Delay(1);
                return $"Error: {error.Description}";
            });

        message.Should().Be("Success: 5");
    }

    [Fact]
    public async Task MatchAsync_WithFailureResult_ShouldCallOnFailure()
    {
        var result = Result<Int32>.Failure(Error.NotFound("Item not found"));

        var message = await result.MatchAsync(
            async x =>
            {
                await Task.Delay(1);
                return $"Success: {x}";
            },
            async error =>
            {
                await Task.Delay(1);
                return $"Error: {error.Description}";
            });

        message.Should().Be("Error: Item not found");
    }

    [Fact]
    public async Task MatchAsync_WithTask_ShouldHandleBothCases()
    {
        var resultTask = Task.FromResult(Result<Int32>.Success(5));

        var message = await resultTask.MatchAsync(
            x => $"Success: {x}",
            error => $"Error: {error.Description}");

        message.Should().Be("Success: 5");
    }

    [Fact]
    public async Task MatchAsync_WithTaskAndAsyncHandlers_ShouldHandleBothCases()
    {
        var resultTask = Task.FromResult(Result<Int32>.Success(5));

        var message = await resultTask.MatchAsync(
            async x =>
            {
                await Task.Delay(1);
                return $"Success: {x}";
            },
            async error =>
            {
                await Task.Delay(1);
                return $"Error: {error.Description}";
            });

        message.Should().Be("Success: 5");
    }

    [Fact]
    public async Task TapAsync_WithSuccessResult_ShouldExecuteAction()
    {
        var result = Result<Int32>.Success(5);
        var sideEffect = 0;

        var tapped = await result.TapAsync(async x =>
        {
            await Task.Delay(1);
            sideEffect = x * 2;
        });

        tapped.IsSuccess.Should().BeTrue();
        tapped.Value.Should().Be(5);
        sideEffect.Should().Be(10);
    }

    [Fact]
    public async Task TapAsync_WithFailureResult_ShouldNotExecuteAction()
    {
        var result = Result<Int32>.Failure(Error.NotFound("Not found"));
        var sideEffect = 0;

        var tapped = await result.TapAsync(async x =>
        {
            await Task.Delay(1);
            sideEffect = x * 2;
        });

        tapped.IsFailure.Should().BeTrue();
        sideEffect.Should().Be(0);
    }

    [Fact]
    public async Task TapAsync_WithTask_ShouldExecuteAction()
    {
        var resultTask = Task.FromResult(Result<Int32>.Success(5));
        var sideEffect = 0;

        var tapped = await resultTask.TapAsync(x => sideEffect = x * 2);

        tapped.IsSuccess.Should().BeTrue();
        tapped.Value.Should().Be(5);
        sideEffect.Should().Be(10);
    }

    [Fact]
    public async Task TapAsync_WithTaskAndAsyncAction_ShouldExecuteAction()
    {
        var resultTask = Task.FromResult(Result<Int32>.Success(5));
        var sideEffect = 0;

        var tapped = await resultTask.TapAsync(async x =>
        {
            await Task.Delay(1);
            sideEffect = x * 2;
        });

        tapped.IsSuccess.Should().BeTrue();
        tapped.Value.Should().Be(5);
        sideEffect.Should().Be(10);
    }

    [Fact]
    public async Task AsyncChaining_ShouldWorkCorrectly()
    {
        var result = await Task.FromResult(Result<Int32>.Success(5))
            .MapAsync(async x =>
            {
                await Task.Delay(1);
                return x * 2;
            })
            .BindAsync(async x =>
            {
                await Task.Delay(1);
                return x > 5 ? Result<String>.Success(x.ToString()) : Error.Validation("Too small");
            })
            .TapAsync(async x =>
            {
                await Task.Delay(1);
                Console.WriteLine(x);
            });

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("10");
    }
}
