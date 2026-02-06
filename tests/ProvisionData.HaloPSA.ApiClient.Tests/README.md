# ProvisionData.HaloPSA.ApiClient.Tests

XUnit test project for the HaloPSA API Client library, providing both unit and integration test coverage.

## Testing Stack

- **XUnit v3** - Testing framework
- **Moq** - Mocking framework for creating test doubles
- **FluentAssertions** - Assertion library with fluent, readable syntax
- **Bogus** - Fake data generator for test data
- **Microsoft.Extensions.TimeProvider.Testing** - Time provider testing utilities
- **ProvisionData.Testing.Integration** - Custom integration testing framework
- **MartinCostello.Logging.XUnit.v3** & **Meziantou.Extensions.Logging.Xunit** - XUnit logging extensions

## Running Tests

```bash
# Run all tests
dotnet test

# Run tests with detailed output
dotnet test --verbosity detailed

# Run tests with code coverage
dotnet test --collect:"XPlat Code Coverage"
```

## Test Structure

Tests are organized into two main categories:

### Unit Tests (`UnitTests/`)

- **`HaloPsaApiClientTests.cs`** - Core `ApiClient` initialization and configuration tests
- **`HaloPsaApiClient_AssetsTests.cs`** - Asset management endpoint tests
- **`HaloPsaApiClientExtensionsTests.cs`** - Dependency injection extension method tests
- **`JsonExtensionsTests.cs`** - JSON utility extension tests
- **`ModelChangeValidatorTests.cs`** - Model generator validation tests

### Integration Tests (`IntegrationTests/`)

- **`ApiClientTests.cs`** - Full integration tests against a live HaloPSA instance
- **`ApiClientTestFixture.cs`** - Shared test fixture with dependency injection setup
- **`ApiClientTestBase.cs`** - Base class for integration tests
- **`ApiClientTestData.cs`** - Configuration model for test data (customer IDs, etc.)

## Writing Tests

### Unit Test Example: Testing with Moq and FluentAssertions

```csharp
[Fact]
public void Constructor_ShouldInitialize_WithValidParameters()
{
    // Arrange
    var tokenProviderMock = new Mock<IAuthTokenProvider>();
    var loggerMock = new Mock<ILogger<ApiClient>>();
    var optionsMock = new Mock<IOptions<HaloPsaApiClientOptions>>();
    var timeProviderMock = new Mock<TimeProvider>();
    var fieldMappingProviderMock = new Mock<IFieldMappingProvider>();

    var options = new HaloPsaApiClientOptions
    {
        AuthUrl = "https://test.halo.local/auth/",
        ApiUrl = "https://test.halo.local/api/",
        ClientId = "test-client-id",
        ClientSecret = "test-client-secret",
        PageSize = 50
    };

    optionsMock.Setup(x => x.Value).Returns(options);

    // Act
    var client = new ApiClient(
        tokenProviderMock.Object,
        loggerMock.Object,
        optionsMock.Object,
        timeProviderMock.Object,
        fieldMappingProviderMock.Object
    );

    // Assert
    client.Should().NotBeNull();
}
```

### Integration Test Example

```csharp
public class ApiClientTests(ApiClientTestFixture fixture, ITestOutputHelper testOutputHelper)
    : ApiClientTestBase(fixture, testOutputHelper)
{
    [Fact]
    public async Task Client_ShouldAuthenticate()
    {
        // Arrange
        var client = Services.GetRequiredService<ApiClient>();

        // Act
        var info = await client.GetInstanceInfoAsync(CancellationToken.None);

        // Assert
        info.Should().NotBeNull();
        info.AppName.Should().Be("Halo PSA");
    }
}
```

### Global Usings

The following namespaces are globally imported via the project file:

- `Xunit`
- `Moq`
- `FluentAssertions`

## Integration Test Configuration

Integration tests require a `appsettings.Testing.json` file (only copied in Debug builds):

```json
{
  "HaloPsaApiClientOptions": {
    "AuthUrl": "https://your-instance.halopsa.com/auth/",
    "ApiUrl": "https://your-instance.halopsa.com/api/",
    "ClientId": "your-client-id",
    "ClientSecret": "your-client-secret",
    "PageSize": 50
  },
  "ApiClientTestData": {
    "TestCustomerId": 123,
    "TestAssetId": 456
  }
}
```

> **NOTE**: The `appsettings.Testing.json` file should not be committed to source control as it contains sensitive credentials.

## Test Coverage Goals

- **Unit Tests**: Test all public methods with mocked dependencies
- **Integration Tests**: Validate end-to-end functionality against a live HaloPSA instance
- Test error handling and edge cases
- Verify proper logging behavior
- Validate authentication token management
- Ensure custom field mapping works correctly
