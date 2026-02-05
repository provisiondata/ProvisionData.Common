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

using System.Text.Json;

namespace ProvisionData.UnitTests;

/// <summary>
/// Unit tests for JSON serialization of Result, Result&lt;T&gt;, and Error types.
/// </summary>
public class SerializationTests
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = false
    };

    #region Result<T> Success Tests

    [Fact]
    public void ResultOfT_Success_WithInt_ShouldRoundTrip()
    {
        var original = Result.Success(42);
        var json = JsonSerializer.Serialize(original, _jsonOptions);

        var deserialized = JsonSerializer.Deserialize<Result<Int32>>(json, _jsonOptions);

        deserialized.Should().NotBeNull();
        deserialized!.IsSuccess.Should().BeTrue();
        deserialized.Value.Should().Be(42);
    }

    [Fact]
    public void ResultOfT_Success_WithString_ShouldRoundTrip()
    {
        var original = Result.Success("Hello, World!");
        var json = JsonSerializer.Serialize(original, _jsonOptions);

        var deserialized = JsonSerializer.Deserialize<Result<String>>(json, _jsonOptions);

        deserialized.Should().NotBeNull();
        deserialized!.IsSuccess.Should().BeTrue();
        deserialized.Value.Should().Be("Hello, World!");
    }

    [Fact]
    public void ResultOfT_Success_WithNullString_ShouldRoundTrip()
    {
        Result<String?> original = Result.Success<String?>(null);
        var json = JsonSerializer.Serialize(original, _jsonOptions);

        var deserialized = JsonSerializer.Deserialize<Result<String?>>(json, _jsonOptions);

        deserialized.Should().NotBeNull();
        deserialized!.IsSuccess.Should().BeTrue();
        deserialized.Value.Should().BeNull();
    }

    [Fact]
    public void ResultOfT_Success_WithRecord_ShouldRoundTrip()
    {
        var record = new TestRecord(123, "John Doe", "john@example.com");
        var original = Result.Success(record);
        var json = JsonSerializer.Serialize(original, _jsonOptions);

        var deserialized = JsonSerializer.Deserialize<Result<TestRecord>>(json, _jsonOptions);

        deserialized.Should().NotBeNull();
        deserialized!.IsSuccess.Should().BeTrue();
        deserialized.Value.Should().BeEquivalentTo(record);
    }

    [Fact]
    public void ResultOfT_Success_WithList_ShouldRoundTrip()
    {
        var items = new List<String> { "one", "two", "three" };
        var original = Result.Success(items);
        var json = JsonSerializer.Serialize(original, _jsonOptions);

        var deserialized = JsonSerializer.Deserialize<Result<List<String>>>(json, _jsonOptions);

        deserialized.Should().NotBeNull();
        deserialized!.IsSuccess.Should().BeTrue();
        deserialized.Value.Should().BeEquivalentTo(items);
    }

    [Fact]
    public void ResultOfT_Success_WithDictionary_ShouldRoundTrip()
    {
        var dict = new Dictionary<String, Int32> { ["a"] = 1, ["b"] = 2, ["c"] = 3 };
        var original = Result.Success(dict);
        var json = JsonSerializer.Serialize(original, _jsonOptions);

        var deserialized = JsonSerializer.Deserialize<Result<Dictionary<String, Int32>>>(json, _jsonOptions);

        deserialized.Should().NotBeNull();
        deserialized!.IsSuccess.Should().BeTrue();
        deserialized.Value.Should().BeEquivalentTo(dict);
    }

    #endregion

    #region Result<T> Failure Tests

    [Fact]
    public void ResultOfT_Failure_WithApiError_ShouldRoundTrip()
    {
        Result<Int32> original = new ApiError("Service unavailable");
        var json = JsonSerializer.Serialize(original, _jsonOptions);

        var deserialized = JsonSerializer.Deserialize<Result<Int32>>(json, _jsonOptions);

        deserialized.Should().NotBeNull();
        deserialized!.IsFailure.Should().BeTrue();
        deserialized.Error.Should().BeOfType<ApiError>();
        deserialized.Error.Description.Should().Be("Service unavailable");
        deserialized.Error.Code.Should().Be(new ApiError("test").Code);
    }

    [Fact]
    public void ResultOfT_Failure_WithValidationError_ShouldRoundTrip()
    {
        Result<String> original = new ValidationError("Email format invalid");
        var json = JsonSerializer.Serialize(original, _jsonOptions);

        var deserialized = JsonSerializer.Deserialize<Result<String>>(json, _jsonOptions);

        deserialized.Should().NotBeNull();
        deserialized!.IsFailure.Should().BeTrue();
        deserialized.Error.Should().BeOfType<ValidationError>();
        deserialized.Error.Description.Should().Be("Email format invalid");
        deserialized.Error.Code.Should().Be(new ValidationError("test").Code);
    }

    [Fact]
    public void ResultOfT_Failure_WithNotFoundError_ShouldRoundTrip()
    {
        Result<TestRecord> original = new NotFoundError("Customer not found");
        var json = JsonSerializer.Serialize(original, _jsonOptions);

        var deserialized = JsonSerializer.Deserialize<Result<TestRecord>>(json, _jsonOptions);

        deserialized.Should().NotBeNull();
        deserialized!.IsFailure.Should().BeTrue();
        deserialized.Error.Should().BeOfType<NotFoundError>();
        deserialized.Error.Description.Should().Be("Customer not found");
    }

    [Fact]
    public void ResultOfT_Failure_AllErrorTypes_ShouldPreserveErrorCodeSingletons()
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

        foreach (var error in errors)
        {
            Result<Int32> result = error;
            var json = JsonSerializer.Serialize(result, _jsonOptions);
            var deserialized = JsonSerializer.Deserialize<Result<Int32>>(json, _jsonOptions);

            deserialized.Should().NotBeNull();
            deserialized!.IsFailure.Should().BeTrue();
            deserialized.Error.GetType().Should().Be(error.GetType());
            deserialized.Error.Code.Should().Be(error.Code,
                $"{error.GetType().Name} should preserve ErrorCode value equality");
        }
    }

    #endregion

    #region Error Serialization Tests

    [Fact]
    public void Error_ApiError_ShouldRoundTrip()
    {
        var original = new ApiError("HTTP 500 error");
        var json = JsonSerializer.Serialize(original, _jsonOptions);

        var deserialized = JsonSerializer.Deserialize<ApiError>(json, _jsonOptions);

        deserialized.Should().NotBeNull();
        deserialized!.Description.Should().Be("HTTP 500 error");
        deserialized.Code.Should().Be(new ApiError("test").Code);
    }

    [Fact]
    public void Error_BusinessRuleViolationError_ShouldRoundTrip()
    {
        var original = new BusinessRuleViolationError("Cannot delete active subscription");
        var json = JsonSerializer.Serialize(original, _jsonOptions);

        var deserialized = JsonSerializer.Deserialize<BusinessRuleViolationError>(json, _jsonOptions);

        deserialized.Should().NotBeNull();
        deserialized!.Description.Should().Be("Cannot delete active subscription");
        deserialized.Code.Should().Be(new BusinessRuleViolationError("test").Code);
    }

    [Fact]
    public void Error_ConfigurationError_ShouldRoundTrip()
    {
        var original = new ConfigurationError("Missing connection string");
        var json = JsonSerializer.Serialize(original, _jsonOptions);

        var deserialized = JsonSerializer.Deserialize<ConfigurationError>(json, _jsonOptions);

        deserialized.Should().NotBeNull();
        deserialized!.Description.Should().Be("Missing connection string");
        deserialized.Code.Should().Be(new ConfigurationError("test").Code);
    }

    [Fact]
    public void Error_ConflictError_ShouldRoundTrip()
    {
        var original = new ConflictError("Version mismatch detected");
        var json = JsonSerializer.Serialize(original, _jsonOptions);

        var deserialized = JsonSerializer.Deserialize<ConflictError>(json, _jsonOptions);

        deserialized.Should().NotBeNull();
        deserialized!.Description.Should().Be("Version mismatch detected");
        deserialized.Code.Should().Be(new ConflictError("test").Code);
    }

    [Fact]
    public void Error_NotFoundError_ShouldRoundTrip()
    {
        var original = new NotFoundError("Resource ID 123 not found");
        var json = JsonSerializer.Serialize(original, _jsonOptions);

        var deserialized = JsonSerializer.Deserialize<NotFoundError>(json, _jsonOptions);

        deserialized.Should().NotBeNull();
        deserialized!.Description.Should().Be("Resource ID 123 not found");
        deserialized.Code.Should().Be(new NotFoundError("test").Code);
    }

    [Fact]
    public void Error_UnauthorizedError_ShouldRoundTrip()
    {
        var original = new UnauthorizedError("Invalid API key");
        var json = JsonSerializer.Serialize(original, _jsonOptions);

        var deserialized = JsonSerializer.Deserialize<UnauthorizedError>(json, _jsonOptions);

        deserialized.Should().NotBeNull();
        deserialized!.Description.Should().Be("Invalid API key");
        deserialized.Code.Should().Be(new UnauthorizedError("test").Code);
    }

    [Fact]
    public void Error_UnhandledExceptionError_ShouldRoundTrip()
    {
        var original = new UnhandledExceptionError("NullReferenceException at line 42");
        var json = JsonSerializer.Serialize(original, _jsonOptions);

        var deserialized = JsonSerializer.Deserialize<UnhandledExceptionError>(json, _jsonOptions);

        deserialized.Should().NotBeNull();
        deserialized!.Description.Should().Be("NullReferenceException at line 42");
        deserialized.Code.Should().Be(new UnhandledExceptionError("test").Code);
    }

    [Fact]
    public void Error_ValidationError_ShouldRoundTrip()
    {
        var original = new ValidationError("Field 'name' is required");
        var json = JsonSerializer.Serialize(original, _jsonOptions);

        var deserialized = JsonSerializer.Deserialize<ValidationError>(json, _jsonOptions);

        deserialized.Should().NotBeNull();
        deserialized!.Description.Should().Be("Field 'name' is required");
        deserialized.Code.Should().Be(new ValidationError("test").Code);
    }

    [Fact]
    public void Error_WithSpecialCharacters_ShouldRoundTrip()
    {
        var original = new ConfigurationError("Path: <>&\"'\\\n\t");
        var json = JsonSerializer.Serialize(original, _jsonOptions);

        var deserialized = JsonSerializer.Deserialize<ConfigurationError>(json, _jsonOptions);

        deserialized.Should().NotBeNull();
        deserialized!.Description.Should().Be("Path: <>&\"'\\\n\t");
    }

    [Fact]
    public void Error_AsBaseType_ShouldDeserializeToCorrectType()
    {
        Error original = new NotFoundError("Item not found");
        var json = JsonSerializer.Serialize(original, _jsonOptions);

        var deserialized = JsonSerializer.Deserialize<Error>(json, _jsonOptions);

        deserialized.Should().NotBeNull();
        deserialized.Should().BeOfType<NotFoundError>();
        deserialized!.Description.Should().Be("Item not found");
    }

    #endregion

    #region Extension Method Tests After Deserialization

    [Fact]
    public void ResultOfT_GetValueOrDefault_WorksAfterDeserialization()
    {
        var original = Result.Success(99);
        var json = JsonSerializer.Serialize(original, _jsonOptions);
        var deserialized = JsonSerializer.Deserialize<Result<Int32>>(json, _jsonOptions);

        deserialized!.GetValueOrDefault().Should().Be(99);
    }

    [Fact]
    public void ResultOfT_Failure_GetValueOrDefault_WorksAfterDeserialization()
    {
        Result<Int32> original = new ApiError("Failed");
        var json = JsonSerializer.Serialize(original, _jsonOptions);
        var deserialized = JsonSerializer.Deserialize<Result<Int32>>(json, _jsonOptions);

        deserialized.Should().NotBeNull();
        deserialized.GetValueOrDefault().Should().Be(0);
        deserialized.GetValueOrDefault(42).Should().Be(42);
    }

    [Fact]
    public void Error_IsErrorType_WorksAfterDeserialization()
    {
        var original = new ValidationError("test");
        var json = JsonSerializer.Serialize(original, _jsonOptions);
        var deserialized = JsonSerializer.Deserialize<ValidationError>(json, _jsonOptions);

        deserialized!.IsErrorType<ValidationError>().Should().BeTrue();
        deserialized.IsErrorType<NotFoundError>().Should().BeFalse();
    }

    #endregion

    #region Edge Cases and Consistency Tests

    [Fact]
    public void ResultOfT_MultipleSerializations_ShouldProduceSameJson()
    {
        Result<String> result = new NotFoundError("Not found");

        var json1 = JsonSerializer.Serialize(result, _jsonOptions);
        var json2 = JsonSerializer.Serialize(result, _jsonOptions);

        json1.Should().Be(json2, "serialization should be deterministic");
    }

    [Fact]
    public void ResultOfT_MultipleDeserializations_ShouldPreserveErrorCodeEquality()
    {
        Result<Int32> result = new ConflictError("Conflict");
        var json = JsonSerializer.Serialize(result, _jsonOptions);

        var deserialized1 = JsonSerializer.Deserialize<Result<Int32>>(json, _jsonOptions);
        var deserialized2 = JsonSerializer.Deserialize<Result<Int32>>(json, _jsonOptions);

        deserialized1!.Error.Code.Should().Be(deserialized2!.Error.Code,
            "ErrorCode should maintain value equality across deserializations");
    }

    [Fact]
    public void ResultOfT_IsSuccess_ShouldMatchAfterRoundTrip()
    {
        var success = Result.Success(42);
        Result<Int32> failure = new NotFoundError("Not found");

        var successJson = JsonSerializer.Serialize(success, _jsonOptions);
        var failureJson = JsonSerializer.Serialize(failure, _jsonOptions);

        var deserializedSuccess = JsonSerializer.Deserialize<Result<Int32>>(successJson, _jsonOptions);
        var deserializedFailure = JsonSerializer.Deserialize<Result<Int32>>(failureJson, _jsonOptions);

        deserializedSuccess!.IsSuccess.Should().BeTrue();
        deserializedSuccess.IsFailure.Should().BeFalse();
        deserializedFailure!.IsSuccess.Should().BeFalse();
        deserializedFailure.IsFailure.Should().BeTrue();
    }

    #endregion
}

/// <summary>
/// Test record for serialization tests.
/// </summary>
/// <param name="Id">The ID.</param>
/// <param name="Name">The name.</param>
/// <param name="Email">The email address.</param>
public record TestRecord(Int32 Id, String Name, String Email);
