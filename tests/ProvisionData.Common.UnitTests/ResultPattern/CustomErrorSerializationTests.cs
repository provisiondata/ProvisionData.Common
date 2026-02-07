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

using System.Text.Json;

namespace ProvisionData.UnitTests.ResultPattern;

/// <summary>
/// Demonstrates that custom Error types can be created and serialized
/// without modifying the base Error class.
/// </summary>
public class CustomErrorSerializationTests
{
    // Custom error type defined in this test assembly (simulating external assembly)
    public sealed class CustomerNotFoundError(String description, String customerId)
        : Error(CustomerNotFoundErrorCode.Instance, description)
    {
        public String CustomerId { get; } = customerId;

        internal sealed class CustomerNotFoundErrorCode : ErrorCode
        {
            public static readonly CustomerNotFoundErrorCode Instance = new();
            protected override String Name => nameof(CustomerNotFoundError);
        }
    }

    // Another custom error with additional properties
    public sealed class PaymentDeclinedError(String description, String declineReason)
        : Error(PaymentDeclinedErrorCode.Instance, description)
    {
        public String DeclineReason { get; } = declineReason;

        internal sealed class PaymentDeclinedErrorCode : ErrorCode
        {
            public static readonly PaymentDeclinedErrorCode Instance = new();
            protected override String Name => nameof(PaymentDeclinedError);
        }
    }

    [Fact]
    public void CustomError_ShouldSerializeAndDeserialize()
    {
        var original = new CustomerNotFoundError("Customer ID 12345 not found", "12345");
        var json = JsonSerializer.Serialize(original);

        var deserialized = JsonSerializer.Deserialize<CustomerNotFoundError>(json);

        deserialized.Should().NotBeNull();
        deserialized!.Description.Should().Be("Customer ID 12345 not found");
        deserialized.Code.Should().Be(original.Code);
    }

    [Fact]
    public void CustomError_AsBaseType_ShouldDeserializeToCorrectType()
    {
        Error original = new CustomerNotFoundError("Customer not found", "ABC123");
        var json = JsonSerializer.Serialize(original);

        var deserialized = JsonSerializer.Deserialize<Error>(json);

        deserialized.Should().NotBeNull();
        deserialized.Should().BeOfType<CustomerNotFoundError>();
        deserialized!.Description.Should().Be("Customer not found");
    }

    [Fact]
    public void CustomError_InResultOfT_ShouldRoundTrip()
    {
        Result<String> original = new CustomerNotFoundError("Customer ID 999 not found", "999");
        var json = JsonSerializer.Serialize(original);

        var deserialized = JsonSerializer.Deserialize<Result<String>>(json);

        deserialized.Should().NotBeNull();
        deserialized.IsSuccess.Should().BeFalse();
        deserialized.Error.Should().BeOfType<CustomerNotFoundError>();
        deserialized.Error.Description.Should().Be("Customer ID 999 not found");
    }

    [Fact]
    public void CustomError_WithAdditionalProperties_ShouldSerialize()
    {
        var original = new PaymentDeclinedError("Payment declined", "Insufficient funds");
        var json = JsonSerializer.Serialize(original);

        var deserialized = JsonSerializer.Deserialize<PaymentDeclinedError>(json);

        deserialized.Should().NotBeNull();
        deserialized!.Description.Should().Be("Payment declined");
        deserialized.DeclineReason.Should().Be("Insufficient funds");
    }

    [Fact]
    public void MultipleCustomErrors_ShouldMaintainTypeIntegrity()
    {
        var errors = new Error[]
        {
            new CustomerNotFoundError("Customer 1 not found", "C001"),
            new PaymentDeclinedError("Payment failed", "Card expired"),
            new NotFoundError("Resource not found"), // Built-in error mixed with custom
            new CustomerNotFoundError("Customer 2 not found", "C002")
        };

        foreach (var error in errors)
        {
            var json = JsonSerializer.Serialize(error);
            var deserialized = JsonSerializer.Deserialize<Error>(json);

            deserialized.Should().NotBeNull();
            deserialized!.GetType().Should().Be(error.GetType(),
                $"error type {error.GetType().Name} should be preserved");
        }
    }

    [Fact]
    public void CustomError_ErrorCodeSingleton_ShouldBePreservedAfterDeserialization()
    {
        var original = new CustomerNotFoundError("test", "TEST123");
        var json = JsonSerializer.Serialize(original);

        var deserialized = JsonSerializer.Deserialize<CustomerNotFoundError>(json);

        deserialized!.Code.Should().Be(original.Code,
            "ErrorCode value equality should be preserved through serialization");
        deserialized.CustomerId.Should().Be("TEST123");
    }
}
