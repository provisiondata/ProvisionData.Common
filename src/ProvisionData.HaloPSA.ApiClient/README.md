# ProvisionData.HaloPSA.ApiClient

![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/provisiondata/ProvisionData.HaloPSA.ApiClient/ci.yml?link=https%3A%2F%2Fgithub.com%2Fprovisiondata%2FProvisionData.HaloPSA.ApiClient)
![NuGet Version](https://img.shields.io/nuget/v/ProvisionData.HaloPSA.ApiClient?link=https%3A%2F%2Fgithub.com%2Fprovisiondata%2FProvisionData.HaloPSA.ApiClient)

A .NET API client library for **Halo PSA** (a Professional Services Automation platform). It provides typed .NET models and HTTP client wrappers to interact with the HaloPSA REST API.

> **NOTE**:It is early days for this library and it is still under active development. The API will change as more endpoints are added and feedback is received.
> Specifically, many of the Halo PSA API types are named differently than what is exposed in the UI. As more endpoints are added, the naming conventions will be
> aligned with the UI which means the DTOs and some of their properties will change.

## Features

- **OAuth 2.0 Authentication** with automatic token management via `IAuthTokenProvider`
- **Custom Field Support** - Strongly-typed access to HaloPSA custom fields with configuration-based mapping
- **Extension Method Architecture** - Clean, modular API organized by resource type (Assets, Customers, Tickets, etc.)
- **Dependency Injection Ready** - Follows ASP.NET Core conventions with full DI support
- **Type Safety** - Strongly typed models with JSON serialization using System.Text.Json source generators
- **Comprehensive Logging** - Structured logging throughout using `ILogger<T>`
- **Rate Limiting Handling** - Automatic retry with delay on 429 responses
- **Pagination Support** - Configurable page size for list operations

## Current API Coverage

The client provides extension methods organized by resource type:

- **Customers** (`CustomerExtensions`) - Client/customer management
  - `ListCustomersAsync()` - List all customers
  - `GetCustomerAsync(customerId)` - Get customer details
  
- **Assets** (`AssetExtensions`) - Asset tracking and management  
  - `ListAssetsAsync()` - List all assets
  - `GetAssetAsync(assetId)` - Get asset details
  - Custom field support for asset properties

- **Tickets** (`TicketExtensions`) - Ticket/fault management
  - `ListTicketsAsync()` - List all tickets
  - `CreateTicketAsync(ticket)` - Create new ticket

- **Items** (`ItemExtensions`) - Product/item management

- **Suppliers** (`SupplierExtensions`) - Supplier management

- **Invoices** (`InvoiceExtensions`) - Invoice operations

- **Recurring Invoices** (`RecurringInvoiceExtensions`) - Invoice automation


## Installation

```bash
dotnet add package ProvisionData.HaloPSA.ApiClient
```

## Quick Start

### Dependency Injection Setup

Add the client to your service collection using dependency injection:

```csharp
// Using app settings.json configuration
builder.Services.AddHaloPsaApiClient(builder.Configuration);

// Or configure manually
builder.Services.AddHaloPsaApiClient(options =>
{
    options.AuthUrl = "https://your-instance.halopsa.com/auth/";
    options.ApiUrl = "https://your-instance.halopsa.com/api/";
    options.ClientId = "your-client-id";
    options.ClientSecret = "your-client-secret";
    options.PageSize = 50;
});
```

### Configuration (appsettings.json)

```json
{
  "HaloPsaApiClientOptions": {
    "AuthUrl": "https://your-instance.halopsa.com/auth/",
    "ApiUrl": "https://your-instance.halopsa.com/api/",
    "ClientId": "your-client-id",
    "ClientSecret": "your-client-secret",
    "PageSize": 50
  }
}
```

### Usage in Controllers/Services

```csharp
public class AssetController : ControllerBase
{
    private readonly ApiClient _apiClient;

    public AssetController(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    [HttpGet]
    public async Task<IActionResult> GetAssets()
    {
        var assets = await _apiClient.ListAssetsAsync();
        return Ok(assets);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAsset(Int32 id)
    {
        var asset = await _apiClient.GetAssetAsync(id);
        return Ok(asset);
    }
}
```

## Custom Fields

HaloPSA allows custom fields for various entity types. This library provides strongly-typed access to custom fields through configuration.

### Configuration

Add custom field mappings to your `appsettings.json`. Field IDs are specific to your HaloPSA instance and can be found in the HaloPSA admin panel:

```json
{
  "HaloPsaApiClientOptions": {
    "FieldMappings": {
      "Asset": {
        "Name": 101,
        "SerialNumber": 117,
        "User": 148,
        "Model": 51,
        "PurchaseDate": 24
      }
    }
  }
}
```

### Usage

Once configured, custom fields are accessed as strongly-typed properties:

```csharp
// Initialize field mappings (call once at startup)
serviceProvider.EnsureCustomFieldsHaveBeenMapped();

// Access custom fields
var asset = await apiClient.GetAssetAsync(123);
Console.WriteLine($"Asset: {asset.Name}");
Console.WriteLine($"Serial: {asset.SerialNumber}");
Console.WriteLine($"Model: {asset.Model}");

// Update custom fields
asset.Model = "Dell OptiPlex 7090";
await apiClient.UpdateAssetAsync(asset);
```

For more details on custom fields, see the [Custom Fields Quick Reference](docs/custom-fields-quick-reference.md).

## Extending the Client

### Adding Additional API Endpoints

The client uses an extension method pattern for organizing API endpoints by resource type:

1. **Create an extension class** (e.g., `YourResourceExtensions.cs`):

```csharp
public static class YourResourceExtensions
{
    public static async Task<YourModel> GetYourResourceAsync(
        this ApiClient apiClient, 
        Int32 id, 
        CancellationToken cancellationToken = default)
    {
        var uri = "YourEndpoint"
            .AppendPathSegment(id)
            .ToUri();

        return await apiClient.HttpGetAsync(
            uri, 
            YourJsonContext.Default.YourModel, 
            cancellationToken);
    }
}
```

2. **Create DTOs** in the `Contracts` project with `[JsonPropertyName]` attributes

3. **Create a JSON source generator context** for optimal serialization performance

Ensure to follow existing patterns for authentication, error handling, and logging.

## Advanced Configuration

### Custom HttpClient Configuration

`AddHaloPsaApiClient()` returns an `IHttpClientBuilder`, allowing you to configure the underlying `HttpClient`:

```csharp
builder.Services.AddHaloPsaApiClient(builder.Configuration)
    .ConfigureHttpClient(client =>
    {
        client.Timeout = TimeSpan.FromMinutes(10);
    })
    .AddPolicyHandler(GetRetryPolicy()); // Using Polly for resilience
```

### Custom Field Mappings Initialization

For applications using custom fields, ensure mappings are initialized at startup:

```csharp
var app = builder.Build();

// Initialize custom field mappings
app.Services.EnsureCustomFieldsHaveBeenMapped();

app.Run();
```

### Custom Implementations

You can provide custom implementations of core interfaces:

```csharp
services.AddSingleton<IAuthTokenProvider, YourCustomTokenProvider>();
services.AddSingleton<IFieldMappingProvider, YourCustomFieldMapper>();
services.AddSingleton(TimeProvider.System); // Or a custom TimeProvider for testing
```

## License

This project is licensed under the GNU Affero General Public License v3.0 (AGPL-3.0) - see the [LICENSE.md](LICENSE.md) file for details.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.
