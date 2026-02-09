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

namespace ProvisionData.ResultPattern;

/// <summary>
/// Unit tests for the <see cref="Error"/> class and its factory methods.
/// </summary>
public class ErrorTests(ResultPatternIntegrationTestFixture fixture, ITestOutputHelper output)
    : ResultPatternUnitTestBase(fixture, output)
{
    [Fact]
    public void Error_ShouldSerializeAndDeserializeCorrectly()
    {
        var error = new NotFoundError("There is no resource at this location");

        var json = System.Text.Json.JsonSerializer.Serialize(error);

        var deserialized = System.Text.Json.JsonSerializer.Deserialize<Error>(json);

        deserialized.Should().NotBeNull();
        deserialized.Should().BeOfType<NotFoundError>();

        // Assign to a string variable to ensure ErrorCode can be implicitly converted to string
        String codeName = deserialized.Code;
        codeName.Should().Be("NotFoundError");
        deserialized.Description.Should().Be("API connection failed");
    }

    [Fact]
    public void None_ShouldHaveDescription()
    {
        var error = Result.None;

        error.Description.Should().Be("None");
    }

    [Fact]
    public void None_ShouldHaveNoneErrorCode()
    {
        var error = Result.None;

        String codeName = error.Code;

        codeName.Should().Be("None");
    }

    [Fact]
    public void NotFound_ShouldCreateNotFoundError()
    {
        var error = new NotFoundError("User not found");

        error.Should().BeOfType<NotFoundError>();
        error.Description.Should().Be("User not found");
        String codeName = error.Code;
        codeName.Should().Be("NotFoundError");
    }

    [Fact]
    public void NotFoundError_ShouldUseSingletonErrorCode()
    {
        var error1 = new NotFoundError("Description 1");
        var error2 = new NotFoundError("Description 2");

        error1.Code.Should().BeSameAs(error2.Code);
    }

    [Fact]
    public void Errors_WithSameTypeAndDescription_ShouldNotBeEqual_DifferentInstances()
    {
        var error1 = new NotFoundError("Invalid input");
        var error2 = new NotFoundError("Invalid input");

        error1.Should().NotBe(error2, "different Error instances should not be equal (reference equality)");
    }

    [Fact]
    public void Errors_SameInstance_ShouldBeEqual()
    {
        var error = new NotFoundError("Invalid input");
        var sameError = error;

        error.Should().Be(sameError, "same Error instance should be equal to itself");
    }

    [Fact]
    public void Errors_WithSameTypeButDifferentDescription_ShouldNotBeEqual()
    {
        var error1 = new NotFoundError("Invalid input");
        var error2 = new NotFoundError("Different description");

        error1.Should().NotBe(error2);
    }

    [Fact]
    public void Errors_WithDifferentTypes_ShouldNotBeEqual()
    {
        var error1 = new TransactionError(TransactionFailureReason.CardExpired, "XDV83401@FVAD", "Description");
        var error2 = new NotFoundError("Description");

        error1.Should().NotBe(error2);
    }

    [Fact]
    public void ErrorCodeSingletons_ShouldAllBeUnique()
    {
        var errors = new[]
        {
            Result.None,
            new NotFoundError("test"),
            new TransactionError(TransactionFailureReason.InsufficientFunds,"PJKR037659@PJHGR", "test")
        };

        var codes = errors.Select(e => e.Code).ToList();
        var distinctCodes = codes.Distinct().ToList();

        distinctCodes.Should().HaveCount(codes.Count, "all error code singletons should be unique references");
    }

    [Fact]
    public void Error_CodePropertyCanBeUsedAsString()
    {
        var error = new TransactionError(TransactionFailureReason.InsufficientFunds, "LKJ34567@UHBV", "Test");

        String codeAsString = error.Code;

        codeAsString.Should().Be("TransactionError");
    }

    [Fact]
    public void Error_ToStringOnCode_ShouldReturnName()
    {
        var error = new TransactionError(TransactionFailureReason.InsufficientFunds, "ASD98765@PLMN", "Test");

        var codeString = error.Code.ToString();

        codeString.Should().Be("TransactionError", "ToString should return the error type name");
    }

    [Fact]
    public void IsErrorType_WithMatchingType_ShouldReturnTrue()
    {
        var error = new NotFoundError("Test");

        var result = error.IsErrorType<NotFoundError>();

        result.Should().BeTrue();
    }

    [Fact]
    public void IsErrorType_WithNonMatchingType_ShouldReturnFalse()
    {
        var error = new NotFoundError("Test");

        var result = error.IsErrorType<TransactionError>();

        result.Should().BeFalse();
    }

    [Fact]
    public void IsErrorType_WithBaseErrorType_ShouldReturnTrue()
    {
        var error = new NotFoundError("Test");

        var result = error.IsErrorType<Error>();

        result.Should().BeTrue();
    }
}
