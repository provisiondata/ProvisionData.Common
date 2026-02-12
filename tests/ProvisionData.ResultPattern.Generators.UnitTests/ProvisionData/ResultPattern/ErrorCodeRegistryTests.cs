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

using ProvisionData.ResultPattern.Infrastructure;
using System.Reflection;

namespace ProvisionData.ResultPattern;

public class ErrorCodeRegistryTests
{
    [Fact]
    public void ErrorTypeRegistry_Should_ContainCustomErrors()
    {
        // Module initializers should automatically register the errors when the assembly loads.
        // No manual registration needed - the generator creates the module initializers and
        // the compiler emits code to call them when the assembly is loaded.

        var errorTypes = ErrorCodeRegistry.LookupTable.Keys.ToList();

        // These should now pass because ModuleInitializer registered them:
        errorTypes.Should().Contain(typeof(CustomError));
        errorTypes.Should().Contain(typeof(DatabaseConnectionError));
        errorTypes.Should().Contain(typeof(InventoryInsufficientError));
        errorTypes.Should().Contain(typeof(OrderNotFoundError));
        errorTypes.Should().Contain(typeof(TransactionError));

        // Verify we have the expected count (at least our 5 custom errors)
        errorTypes.Count.Should().BeGreaterThanOrEqualTo(5, because: $"Expected at least 5 error types to be registered, but only found {errorTypes.Count}");
    }

    private static readonly Type ErrorType = typeof(Error);
    private static readonly Type ErrorCodeType = typeof(ErrorCode);
    private const String NotEmpty = "This is not an empty string.";

    [Theory]
    [InlineData(typeof(CustomError))]
    [InlineData(typeof(DatabaseConnectionError))]
    [InlineData(typeof(InventoryInsufficientError))]
    [InlineData(typeof(OrderNotFoundError))]
    [InlineData(typeof(TransactionError))]
    public void Error_Reflection_WorksAsExpected(Type errorType)
    {
        // Validate that the provided type is a subclass of Error
        ErrorType.IsAssignableFrom(errorType).Should().BeTrue($"Expected {errorType.FullName} to be a subclass of {ErrorType.FullName}");

        // Find the Code property
        var errorCodeProp = errorType.GetProperty("Code", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        errorCodeProp.Should().NotBeNull($"Expected {errorType.FullName} to have a 'Code' property");

        // Create an instance of the errorType
        var error = Activator.CreateInstance(errorType, NotEmpty);

        // Get the value of the Code property
        var codeValue = errorCodeProp.GetValue(error);
        codeValue.Should().NotBeNull($"Expected {errorType.FullName} to have a non-null 'Code' property value");

        // Get the type of the ErrorCode
        var errorCodeType = codeValue.GetType();
        ErrorCodeType.IsAssignableFrom(errorCodeType).Should().BeTrue($"Expected {errorCodeType.FullName} to be a subclass of {ErrorCodeType.FullName}");
        // Get the Instance property
        var instanceProp = errorCodeType.GetField("Instance", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
        instanceProp.Should().NotBeNull($"Expected {errorCodeType.FullName} to have a static 'Instance' field");

        // Get the value of the Instance property
        var value = instanceProp.GetValue(null) as ErrorCode;
        value.Should().NotBeNull(because: $"{errorCodeType.FullName} Instance property should return an instance of the error code.");

        var fallback = Activator.CreateInstance(errorCodeType) as ErrorCode;
        fallback.Should().NotBeNull(because: $"{errorCodeType.FullName} type should have a parameterless constructor that can be used as a fallback if the Instance property is not properly implemented.");

    }
}
