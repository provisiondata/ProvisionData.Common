# ProvisionData.Common

A selection of useful classes and utilities commonly used in software by [Provision Data Systems Inc.](https://provisiondata.com)

## Installation

You can install the ProvisionData.Common package via NuGet Package Manager Console:

```pwsh
Install-Package ProvisionData.Common
```

Or via .NET CLI:

```pwsh
dotnet add package ProvisionData.Common
```

## Features

- [Safe IAsyncDisposable and IDisposable pattern](#safe-iasyncdisposable-and-idisposable-pattern)

### Safe IAsyncDisposable and IDisposable pattern

```csharp
public class MyTestFixture : DisposableBase
{
    private HttpClient? _httpClient;           // IDisposable only
    private DbConnection? _dbConnection;       // IAsyncDisposable

    protected override void Dispose(Boolean disposing)
    {
        if (disposing)
        {
            _httpClient?.Dispose();
            _httpClient = null;
        }
 
        base.Dispose(disposing);
    }

    protected override async ValueTask DisposeAsyncCore()
    {
        if (_dbConnection is not null)
        {
            await _dbConnection.DisposeAsync().ConfigureAwait(false);
            _dbConnection = null;
        }

        // HttpClient also implements IAsyncDisposable in modern .NET
        if (_httpClient is IAsyncDisposable asyncDisposable)
        {
            await asyncDisposable.DisposeAsync().ConfigureAwait(false);
        }
        else
        {
            _httpClient?.Dispose();
        }
 
        _httpClient = null;

        await base.DisposeAsyncCore().ConfigureAwait(false);
    }
}
```

**How does a derived class know which dispose to override?**

| Scenario                                                            | Override             |
| ------------------------------------------------------------------- | -------------------- |
| Async resources (e.g., `IAsyncDisposable` fields, async streams)    | `DisposeAsyncCore()` |
| Sync-only resources (e.g., `IDisposable` fields, unmanaged handles) | `Dispose(Boolean)`   |
| Mixed resources                                                     | Both methods         |

The key insight: when `DisposeAsync()` is called, it calls `DisposeAsyncCore()` first (cleaning
managed resources async), then `Dispose(false)` (cleaning only unmanaged resources). This 
prevents double-disposal of managed resources.

#### Trimming and AOT Compatibility

This library includes an `IlDescriptors.xml` file that protects core Error types from being trimmed during ahead-of-time (AOT) compilation or when using the ILLinker. The following types are explicitly preserved:

**Protected Base Types:**
- `Error` and `ErrorJsonConverter`
- `ErrorCode` and `ErrorCodeJsonConverter`
- `Result` and `Result<T>`
- `ResultExtensions`

**Protected Derived Error Types:**
- `ApiError` and `ValidationError`
- `BusinessRuleViolationError` and `ConflictError`
- `ConfigurationError`, `NotFoundError`, and `UnauthorizedError`
- `UnhandledExceptionError`

All public properties and members of these types are preserved to ensure reflection-based JSON serialization and deserialization work correctly in trimmed environments.

**Custom Error Types in Consumer Applications:**

If you create custom `Error` types in your own application and intend to use AOT compilation or trimming, you should create your own `IlDescriptors.xml` file:

```xml
<?xml version="1.0" encoding="utf-8"?>
<linker>
  <assembly fullname="YourAssemblyName">
    <!-- Your custom error types -->
    <type fullname="YourNamespace.CustomError" preserve="all" />
    <type fullname="YourNamespace.CustomError+CustomErrorCode" preserve="all" />
  </assembly>
</linker>
```

Then add it as an `EmbeddedResource` in your `.csproj`:

```xml
<ItemGroup>
  <EmbeddedResource Include="IlDescriptors.xml" />
</ItemGroup>
```

The trimmer will automatically discover and merge all `IlDescriptors.xml` files from your application and all referenced libraries.

---

