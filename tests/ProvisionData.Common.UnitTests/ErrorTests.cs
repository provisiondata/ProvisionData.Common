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
/// Unit tests for the <see cref="Error"/> class and its factory methods.
/// </summary>
public class ErrorTests
{
    [Fact]
    public void None_ShouldHaveEmptyDescription()
    {
        var error = Result.None;

        error.Description.Should().BeEmpty();
    }

    [Fact]
    public void None_ShouldHaveNoneErrorCode()
    {
        var error = Result.None;

        String codeName = error.Code;

        codeName.Should().Be("None");
    }

    [Fact]
    public void ApiError_ShouldCreateApiErrorWithDescription()
    {
        var error = Error.ApiError("API connection failed");

        error.Should().BeOfType<ApiError>();
        error.Description.Should().Be("API connection failed");
        String codeName = error.Code;
        codeName.Should().Be("ApiError");
    }

    [Fact]
    public void BusinessRuleViolation_ShouldCreateBusinessRuleViolationError()
    {
        var error = Error.BusinessRuleViolation("Account balance cannot be negative");

        error.Should().BeOfType<BusinessRuleViolationError>();
        error.Description.Should().Be("Account balance cannot be negative");
        String codeName = error.Code;
        codeName.Should().Be("BusinessRuleViolationError");
    }

    [Fact]
    public void Configuration_ShouldCreateConfigurationError()
    {
        var error = Error.Configuration("Missing connection string");

        error.Should().BeOfType<ConfigurationError>();
        error.Description.Should().Be("Missing connection string");
        String codeName = error.Code;
        codeName.Should().Be("ConfigurationError");
    }

    [Fact]
    public void Conflict_ShouldCreateConflictError()
    {
        var error = Error.Conflict("Resource already exists");

        error.Should().BeOfType<ConflictError>();
        error.Description.Should().Be("Resource already exists");
        String codeName = error.Code;
        codeName.Should().Be("ConflictError");
    }

    [Fact]
    public void NotFound_ShouldCreateNotFoundError()
    {
        var error = Error.NotFound("User not found");

        error.Should().BeOfType<NotFoundError>();
        error.Description.Should().Be("User not found");
        String codeName = error.Code;
        codeName.Should().Be("NotFoundError");
    }

    [Fact]
    public void Unauthorized_ShouldCreateUnauthorizedError()
    {
        var error = Error.Unauthorized("Access denied");

        error.Should().BeOfType<UnauthorizedError>();
        error.Description.Should().Be("Access denied");
        String codeName = error.Code;
        codeName.Should().Be("UnauthorizedError");
    }

    [Fact]
    public void Exception_ShouldCreateUnhandledExceptionError()
    {
        var exception = new InvalidOperationException("Operation failed");

        var error = Error.Exception(exception);

        error.Should().BeOfType<UnhandledExceptionError>();
        error.Description.Should().Be("InvalidOperationException: Operation failed");
        String codeName = error.Code;
        codeName.Should().Be("UnhandledExceptionError");
    }

    [Fact]
    public void Validation_ShouldCreateValidationError()
    {
        var error = Error.Validation("Email format is invalid");

        error.Should().BeOfType<ValidationError>();
        error.Description.Should().Be("Email format is invalid");
        String codeName = error.Code;
        codeName.Should().Be("ValidationError");
    }

    [Fact]
    public void ApiError_ShouldUseSingletonErrorCode()
    {
        var error1 = new ApiError("Description 1");
        var error2 = new ApiError("Description 2");

        error1.Code.Should().BeSameAs(error2.Code);
    }

    [Fact]
    public void BusinessRuleViolationError_ShouldUseSingletonErrorCode()
    {
        var error1 = new BusinessRuleViolationError("Description 1");
        var error2 = new BusinessRuleViolationError("Description 2");

        error1.Code.Should().BeSameAs(error2.Code);
    }

    [Fact]
    public void ConfigurationError_ShouldUseSingletonErrorCode()
    {
        var error1 = new ConfigurationError("Description 1");
        var error2 = new ConfigurationError("Description 2");

        error1.Code.Should().BeSameAs(error2.Code);
    }

    [Fact]
    public void ConflictError_ShouldUseSingletonErrorCode()
    {
        var error1 = new ConflictError("Description 1");
        var error2 = new ConflictError("Description 2");

        error1.Code.Should().BeSameAs(error2.Code);
    }

    [Fact]
    public void NotFoundError_ShouldUseSingletonErrorCode()
    {
        var error1 = new NotFoundError("Description 1");
        var error2 = new NotFoundError("Description 2");

        error1.Code.Should().BeSameAs(error2.Code);
    }

    [Fact]
    public void UnauthorizedError_ShouldUseSingletonErrorCode()
    {
        var error1 = new UnauthorizedError("Description 1");
        var error2 = new UnauthorizedError("Description 2");

        error1.Code.Should().BeSameAs(error2.Code);
    }

    [Fact]
    public void UnhandledExceptionError_ShouldUseSingletonErrorCode()
    {
        var error1 = new UnhandledExceptionError("Description 1");
        var error2 = new UnhandledExceptionError("Description 2");

        error1.Code.Should().BeSameAs(error2.Code);
    }

    [Fact]
    public void ValidationError_ShouldUseSingletonErrorCode()
    {
        var error1 = new ValidationError("Description 1");
        var error2 = new ValidationError("Description 2");

        error1.Code.Should().BeSameAs(error2.Code);
    }

    [Fact]
    public void Errors_WithSameTypeAndDescription_ShouldBeEqual()
    {
        var error1 = new ValidationError("Invalid input");
        var error2 = new ValidationError("Invalid input");

        error1.Should().Be(error2);
    }

    [Fact]
    public void Errors_WithSameTypeButDifferentDescription_ShouldNotBeEqual()
    {
        var error1 = new ValidationError("Invalid input");
        var error2 = new ValidationError("Different description");

        error1.Should().NotBe(error2);
    }

    [Fact]
    public void Errors_WithDifferentTypes_ShouldNotBeEqual()
    {
        var error1 = new ValidationError("Description");
        var error2 = new NotFoundError("Description");

        error1.Should().NotBe(error2);
    }

    [Fact]
    public void ErrorCodeSingletons_ShouldAllBeUnique()
    {
        var errors = new[]
        {
            Result.None,
            Error.ApiError("test"),
            Error.BusinessRuleViolation("test"),
            Error.Configuration("test"),
            Error.Conflict("test"),
            Error.NotFound("test"),
            Error.Unauthorized("test"),
            Error.Exception(new Exception("test")),
            Error.Validation("test")
        };

        var codes = errors.Select(e => e.Code).ToList();
        var distinctCodes = codes.Distinct().ToList();

        distinctCodes.Should().HaveCount(codes.Count, "all error code singletons should be unique references");
    }

    [Fact]
    public void Error_CodePropertyCanBeUsedAsString()
    {
        var error = new ValidationError("Test");

        String codeAsString = error.Code;

        codeAsString.Should().Be("ValidationError");
    }

    [Fact]
    public void Error_ToStringOnCode_ShouldReturnName()
    {
        var error = new ValidationError("Test");

        var codeString = error.Code.ToString();

        codeString.Should().Be("ValidationError", "ToString should return the error type name");
    }

    [Fact]
    public void IsErrorType_WithMatchingType_ShouldReturnTrue()
    {
        var error = new ValidationError("Test");

        var result = error.IsErrorType<ValidationError>();

        result.Should().BeTrue();
    }

    [Fact]
    public void IsErrorType_WithNonMatchingType_ShouldReturnFalse()
    {
        var error = new ValidationError("Test");

        var result = error.IsErrorType<NotFoundError>();

        result.Should().BeFalse();
    }

    [Fact]
    public void IsErrorType_WithBaseErrorType_ShouldReturnTrue()
    {
        var error = new ValidationError("Test");

        var result = error.IsErrorType<Error>();

        result.Should().BeTrue();
    }
}
