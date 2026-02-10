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

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net.Http.Json;
using System.Text.Json;

namespace ProvisionData.ResultPattern.Infrastructure;

public sealed class ErrorCodeRegistryTests(ResultPatternIntegrationTestFixture fixture, ITestOutputHelper output)
    : ResultPatternUnitTestBase(fixture, output)
{
    private sealed class FakeError : Error
    {
        public FakeError() : base(FakeErrorCode.Instance, "Fake") { }
    }

    private sealed class FakeErrorCode : ErrorCode
    {
        public static readonly FakeErrorCode Instance = new();
        protected override String Name => "Fake";
    }

    [Fact]
    public void Registry_Returns_Static_Instance_When_Available()
    {
        var code = ErrorCodeRegistry.GetFor<FakeError>();

        Assert.Same(FakeErrorCode.Instance, code);
    }

    private sealed class NoInstanceError : Error
    {
        public NoInstanceError() : base(new NoInstanceErrorCode(), "NoInstance") { }
    }

    private sealed class NoInstanceErrorCode : ErrorCode
    {
        protected override String Name => "NoInstance";
    }

    [Fact]
    public void Registry_Falls_Back_To_New_When_No_Static_Instance()
    {
        var code1 = ErrorCodeRegistry.GetFor<NoInstanceError>();
        var code2 = ErrorCodeRegistry.GetFor<NoInstanceError>();

        Assert.NotNull(code1);
        Assert.IsType<NoInstanceErrorCode>(code1);

        // Should be cached — same instance returned
        Assert.Same(code1, code2);
    }

    public sealed class ErrorJsonConverterTests
    {
        private readonly JsonSerializerOptions _options = new()
        {
            Converters = { new ErrorJsonConverter() }
        };

        [Fact]
        public void Error_Serializes_And_Deserializes_Correctly()
        {
            var original = new Error(NotFoundError.NotFoundErrorCode.Instance, "Customer not found");

            var json = JsonSerializer.Serialize(original, _options);
            var roundTrip = JsonSerializer.Deserialize<Error>(json, _options);

            Assert.NotNull(roundTrip);
            Assert.Equal(original.Description, roundTrip!.Description);
            Assert.Equal(original.Code.GetType(), roundTrip.Code.GetType());
        }
    }

    public sealed class ResultJsonConverterTests(ResultPatternIntegrationTestFixture fixture, ITestOutputHelper output)
        : ResultPatternUnitTestBase(fixture, output)
    {
        private readonly JsonSerializerOptions _options = new()
        {
            Converters =
        {
            new ErrorJsonConverter(),
            new ResultJsonConverter()
        }
        };

        [Fact]
        public void Result_Success_RoundTrips()
        {
            var original = Result.Success();

            var json = JsonSerializer.Serialize(original, _options);
            var roundTrip = JsonSerializer.Deserialize<Result>(json, _options);

            Assert.True(roundTrip!.IsSuccess);
            Assert.Equal(Result.None, roundTrip.Error);
        }

        [Fact]
        public void Result_Failure_RoundTrips()
        {
            var original = Result.Failure(new Error(NotFoundError.NotFoundErrorCode.Instance, "Missing"));

            var json = JsonSerializer.Serialize(original, _options);
            var roundTrip = JsonSerializer.Deserialize<Result>(json, _options);

            Assert.False(roundTrip!.IsSuccess);
            Assert.Equal(original.Error.Description, roundTrip.Error.Description);
            Assert.Equal(original.Error.Code.GetType(), roundTrip.Error.Code.GetType());
        }
    }

    public sealed class ResultOfTJsonConverterTests
    {
        private readonly JsonSerializerOptions _options = new()
        {
            Converters =
        {
            new ErrorJsonConverter(),
            new ResultJsonConverter(),
            new ResultOfTJsonConverterFactory()
        }
        };

        [Fact]
        public void ResultOfT_Success_RoundTrips()
        {
            var original = Result<Int32>.Success(42);

            var json = JsonSerializer.Serialize(original, _options);
            var roundTrip = JsonSerializer.Deserialize<Result<Int32>>(json, _options);

            Assert.True(roundTrip!.IsSuccess);
            Assert.Equal(42, roundTrip.Value);
        }

        [Fact]
        public void ResultOfT_Failure_RoundTrips()
        {
            var original = Result<Int32>.Failure(new Error(NotFoundError.NotFoundErrorCode.Instance, "Missing"));

            var json = JsonSerializer.Serialize(original, _options);
            var roundTrip = JsonSerializer.Deserialize<Result<Int32>>(json, _options);

            Assert.False(roundTrip!.IsSuccess);
            Assert.Equal(original.Error.Description, roundTrip.Error.Description);
            Assert.Equal(original.Error.Code.GetType(), roundTrip.Error.Code.GetType());
        }
    }

    public sealed class ResultPatternIntegrationTests(WebApplicationFactory<Program> factory)
        : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client = factory.CreateClient();

        [Fact]
        public async Task Controller_Returns_Result_And_HttpClient_Deserializes_It()
        {
            var result = await _client.GetFromJsonAsync<Result<Int32>>("/api/test/success", TestContext.Current.CancellationToken);

            Assert.NotNull(result);
            Assert.True(result!.IsSuccess);
            Assert.Equal(123, result.Value);
        }

        [Fact]
        public async Task Controller_Returns_Error_And_HttpClient_Deserializes_It()
        {
            var result = await _client.GetFromJsonAsync<Result<Int32>>("/api/test/failure", TestContext.Current.CancellationToken);

            Assert.NotNull(result);
            Assert.False(result!.IsSuccess);
            Assert.Equal("Missing", result.Error.Description);
            Assert.IsType<NotFoundError.NotFoundErrorCode>(result.Error.Code);
        }
    }

    [Fact]
    public void ErrorTypeRegistry_Should_ContainCustomErrors()
    {
        // ModuleInitializer automatically registers errors when the assembly loads!
        // No manual registration needed - the generator creates the initializer.

        var errorTypes = ErrorCodeRegistry.LookupTable.Keys.ToList();
        var errorCodeTypes = ErrorCodeRegistry.LookupTable.Values.ToList();

        // These should now pass because ModuleInitializer registered them:
        Assert.Contains(typeof(OrderNotFoundError), errorTypes);
        Assert.Contains(typeof(InventoryInsufficientError), errorTypes);
        Assert.Contains(typeof(DatabaseConnectionError), errorTypes);

        // Verify we have the expected count (at least our 3 custom errors + TransactionError from the library)
        Assert.True(errorTypes.Count >= 4, $"Expected at least 4 error types, but found {errorTypes.Count}");
    }
}

