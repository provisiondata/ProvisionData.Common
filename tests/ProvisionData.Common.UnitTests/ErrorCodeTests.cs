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
/// Unit tests for the <see cref="ErrorCode"/> class and its implementations.
/// </summary>
public class ErrorCodeTests
{
    private sealed class TestErrorCode1 : ErrorCode
    {
        public static readonly TestErrorCode1 Instance = new();
        protected override String Name => "TestError1";
    }

    private sealed class TestErrorCode2 : ErrorCode
    {
        public static readonly TestErrorCode2 Instance = new();
        protected override String Name => "TestError2";
    }

    private sealed class TestErrorCode3 : ErrorCode
    {
        public static readonly TestErrorCode3 Instance = new();
        protected override String Name => "TestError3";
    }

    [Fact]
    public void ToString_ShouldReturnName()
    {
        var errorCode = TestErrorCode1.Instance;

        var result = errorCode.ToString();

        result.Should().Be("TestError1", "ToString should return the Name property");
    }

    [Fact]
    public void ImplicitOperatorToString_ShouldReturnName()
    {
        var errorCode = TestErrorCode1.Instance;

        String result = errorCode;

        result.Should().Be("TestError1");
    }

    [Fact]
    public void Equals_WithNull_ShouldReturnFalse()
    {
        var errorCode = TestErrorCode1.Instance;

        var result = errorCode.Equals(null);

        result.Should().BeFalse();
    }

    [Fact]
    public void Equals_WithSameInstance_ShouldReturnTrue()
    {
        var errorCode = TestErrorCode1.Instance;

        var result = errorCode.Equals(errorCode);

        result.Should().BeTrue();
    }

    [Fact]
    public void Equals_WithDifferentInstance_ShouldReturnFalse()
    {
        var errorCode1 = TestErrorCode1.Instance;
        var errorCode2 = TestErrorCode2.Instance;

        var result = errorCode1.Equals(errorCode2);

        result.Should().BeFalse("different singleton instances are not equal");
    }

    [Fact]
    public void Equals_WithDifferentTypeButSameName_ShouldReturnFalse()
    {
        var errorCode1 = TestErrorCode1.Instance;
        var errorCode3 = TestErrorCode3.Instance;

        var result = errorCode1.Equals(errorCode3);

        result.Should().BeFalse("different singleton instances are not equal even if names match");
    }

    [Fact]
    public void GetHashCode_ShouldBeConsistent()
    {
        var errorCode = TestErrorCode1.Instance;

        var hash1 = errorCode.GetHashCode();
        var hash2 = errorCode.GetHashCode();

        hash1.Should().Be(hash2, "hash code should be consistent");
    }

    [Fact]
    public void GetHashCode_ForDifferentInstances_ShouldBeDifferent()
    {
        var errorCode1 = TestErrorCode1.Instance;
        var errorCode2 = TestErrorCode2.Instance;

        var hash1 = errorCode1.GetHashCode();
        var hash2 = errorCode2.GetHashCode();

        hash1.Should().NotBe(hash2, "different singleton instances should have different hash codes");
    }

    [Fact]
    public void Singletons_ShouldBeReferenceSame()
    {
        var instance1 = TestErrorCode1.Instance;
        var instance2 = TestErrorCode1.Instance;

        instance1.Should().BeSameAs(instance2, "singleton instances should be the same reference");
    }

    [Fact]
    public void ApiErrorCode_ShouldHaveCorrectName()
    {
        var error = new ApiError("Test description");

        String name = error.Code;

        name.Should().Be("ApiError");
    }

    [Fact]
    public void BusinessRuleViolationErrorCode_ShouldHaveCorrectName()
    {
        var error = new BusinessRuleViolationError("Test description");

        String name = error.Code;

        name.Should().Be("BusinessRuleViolationError");
    }

    [Fact]
    public void ConfigurationErrorCode_ShouldHaveCorrectName()
    {
        var error = new ConfigurationError("Test description");

        String name = error.Code;

        name.Should().Be("ConfigurationError");
    }

    [Fact]
    public void ConflictErrorCode_ShouldHaveCorrectName()
    {
        var error = new ConflictError("Test description");

        String name = error.Code;

        name.Should().Be("ConflictError");
    }

    [Fact]
    public void NotFoundErrorCode_ShouldHaveCorrectName()
    {
        var error = new NotFoundError("Test description");

        String name = error.Code;

        name.Should().Be("NotFoundError");
    }

    [Fact]
    public void UnauthorizedErrorCode_ShouldHaveCorrectName()
    {
        var error = new UnauthorizedError("Test description");

        String name = error.Code;

        name.Should().Be("UnauthorizedError");
    }

    [Fact]
    public void UnhandledExceptionErrorCode_ShouldHaveCorrectName()
    {
        var error = new UnhandledExceptionError("Test description");

        String name = error.Code;

        name.Should().Be("UnhandledExceptionError");
    }

    [Fact]
    public void ValidationErrorCode_ShouldHaveCorrectName()
    {
        var error = new ValidationError("Test description");

        String name = error.Code;

        name.Should().Be("ValidationError");
    }

    [Fact]
    public void ErrorCodeSingletons_ShouldBeUnique()
    {
        var errors = new Error[]
        {
            new ApiError("test"),
            new BusinessRuleViolationError("test"),
            new ConfigurationError("test"),
            new ConflictError("test"),
            new NotFoundError("test"),
            new UnauthorizedError("test"),
            new UnhandledExceptionError("test"),
            new ValidationError("test")
        };

        var errorCodes = errors.Select(e => e.Code).ToList();
        var distinctCodes = errorCodes.Distinct().ToList();

        distinctCodes.Should().HaveCount(errorCodes.Count, "all error code singletons should be unique references");
    }
}
