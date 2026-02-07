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

- [Result Pattern](#result-pattern) for handling operation outcomes
- [Safe IAsyncDisposable and IDisposable pattern](#safe-iasyncdisposable-and-idisposable-pattern)

### Result Pattern

> [Result Pattern in C#](https://adrianbailador.github.io/blog/44-result-pattern-)
> by [Adrian Bailador](https://adrianbailador.github.io/)

The `Result` class provides a way to represent the outcome of operations,
encapsulating success and failure states along with relevant data or error messages.

#### Basic Usage

```csharp
public class UserService
{
    private readonly IUserRepository _repository;

    public Result<User> GetById(Int32 id)
    {
        var user = _repository.Find(id);
        
        if (user is null)
            return Error.NotFound($"User with ID {id} was not found");

        return user; // Implicit conversion to Result<User>.Success
    }

    public Result<User> Create(CreateUserRequest request)
    {
        // Validation happens in ASP.NET pipeline using FluentValidation
        // Domain only handles business rules
        
        if (_repository.ExistsByEmail(request.Email))
            return Error.Conflict("A user with this email already exists");

        var user = new User(request.Name, request.Email);
        _repository.Add(user);

        return user;
    }
}
```

#### Error Codes

Error codes use singleton instances with reference equality. Each error type has a unique code that can be used programmatically:

```csharp
// Programmatic use - implicit String conversion
String errorName = error.Code;  // "NotFoundError"

// Debugging - ToString() for logging
Console.WriteLine(error.Code.ToString());  // "NotFoundError"

// Type checking
if (result.Error.IsErrorType<NotFoundError>())
{
    // Handle not found scenario
}
```

**Note**: The `.ToString()` method is primarily for debugging and logging. For programmatic use, rely on the implicit `String` operator or `IsErrorType<T>()` method.

#### Chaining Operations

```csharp
public Result<OrderConfirmation> ProcessOrder(CreateOrderRequest request)
{
    return ValidateOrder(request)
        .Bind(order => CheckInventory(order))
        .Bind(order => ProcessPayment(order))
        .Bind(order => CreateShipment(order))
        .Map(shipment => new OrderConfirmation(shipment.TrackingNumber));
}

private Result<Order> ValidateOrder(CreateOrderRequest request)
{
    if (request.Items.Count == 0)
        return Error.Validation("Order must contain at least one item");
    
    return new Order(request.CustomerId, request.Items);
}

private Result<Order> CheckInventory(Order order)
{
    foreach (var item in order.Items)
    {
        if (!_inventory.IsAvailable(item.ProductId, item.Quantity))
            return Error.Conflict($"Product {item.ProductId} is out of stock");
    }
    return order;
}
```

#### Async Operations

The Result pattern includes comprehensive async support for modern C# codebases:

```csharp
// Example 1: Async data access
public async Task<Result<User>> GetUserAsync(Int32 userId)
{
    return await _repository.FindAsync(userId)
        .MapAsync(user => user ?? Error.NotFound("User not found"));
}

// Example 2: Async pipeline with external services
public async Task<Result<OrderConfirmation>> ProcessOrderAsync(CreateOrderRequest request)
{
    return await ValidateOrderAsync(request)
        .BindAsync(async order => await CheckInventoryAsync(order))
        .BindAsync(async order => await ProcessPaymentAsync(order))
        .TapAsync(async order => await SendConfirmationEmailAsync(order))
        .MapAsync(async order => await CreateConfirmationAsync(order));
}

private async Task<Result<Order>> CheckInventoryAsync(Order order)
{
    foreach (var item in order.Items)
    {
        var isAvailable = await _inventoryService.CheckAvailabilityAsync(item.ProductId, item.Quantity);
        if (!isAvailable)
            return Error.Conflict($"Product {item.ProductId} is out of stock");
    }
    return order;
}

// Example 3: Async Match for handling results
var message = await userService.GetUserAsync(userId)
    .MatchAsync(
        onSuccess: async user => 
        {
            await _analytics.TrackUserAccessAsync(user.Id);
            return $"Welcome, {user.Name}!";
        },
        onFailure: async error => 
        {
            await _logger.LogErrorAsync(error.Description);
            return $"Error: {error.Description}";
        });
```

**When to use async methods:**
- `MapAsync` - When transforming the result value requires async operations (e.g., calling external APIs, database queries)
- `BindAsync` - When the next step in the pipeline is async and can fail (returns `Task<Result<T>>`)
- `MatchAsync` - When handling success/failure cases requires async operations (e.g., logging, analytics)
- `TapAsync` - When side effects are async but shouldn't affect the result (e.g., sending notifications, caching)

#### Using Match

```csharp
var message = userService.GetById(userId).Match(
    onSuccess: user => $"Welcome, {user.Name}!",
    onFailure: error => $"Error: {error.Description}"
);
```

#### Getting Values Safely

```csharp
// With explicit default
var user = result.GetValueOrDefault(User.Guest);

// With type default (null for reference types)
var userId = result.GetValueOrDefault();

// Using Match for custom logic
var user = result.Match(
    onSuccess: u => u,
    onFailure: error => 
    {
        _logger.LogError(error.Description);
        return User.Guest;
    });
```

#### Domain Errors

```csharp
public static class DomainErrors
{
    public static class User
    {
        public static Error NotFound(Int32 id) =>
            Error.NotFound($"User with ID {id} was not found");

        public static Error EmailAlreadyExists(String email) =>
            Error.Conflict($"Email {email} is already registered");

        public static Error InvalidEmail =>
            Error.Validation("The email format is invalid");

        public static Error PasswordTooWeak =>
            Error.Validation(
                "Password must be at least 8 characters with uppercase, lowercase, and digits");
    }

    public static class Order
    {
        public static Error NotFound(Guid id) =>
            Error.NotFound($"Order {id} was not found");

        public static Error EmptyCart =>
            Error.Validation("Cannot create order with empty cart");

        public static Error InsufficientStock(String productId) =>
            Error.Conflict($"Insufficient stock for product {productId}");
    }
}
```

#### Creating Custom Error Types

The `Error` class is fully extensible, allowing you to create domain-specific error types with additional properties while maintaining type safety and automatic JSON serialization.

##### Basic Custom Error

Create a custom error by inheriting from `Error` and defining an internal singleton `ErrorCode`:

```csharp
public sealed class CustomerNotFoundError : Error
{
    public String CustomerId { get; }

    public CustomerNotFoundError(String description, String customerId)
        : base(CustomerNotFoundErrorCode.Instance, description)
    {
        CustomerId = customerId;
    }

    internal sealed class CustomerNotFoundErrorCode : ErrorCode
    {
        public static readonly CustomerNotFoundErrorCode Instance = new();
        protected override String Name => nameof(CustomerNotFoundError);
    }
}
```

**Key requirements:**
- Inherit from `Error` as a `sealed class`
- Define an internal `ErrorCode` class with a singleton `Instance`
- Override `Name` property to return the error type name
- Call base constructor with `ErrorCode` and description
- Add any additional domain-specific properties with getters

##### Custom Error with Additional Properties

Custom errors can include domain-specific data that will automatically serialize:

```csharp
public sealed class PaymentDeclinedError : Error
{
    public String DeclineReason { get; }
    public String TransactionId { get; }

    public PaymentDeclinedError(String description, String declineReason, String transactionId) 
        : base(PaymentDeclinedErrorCode.Instance, description)
    {
        DeclineReason = declineReason;
        TransactionId = transactionId;
    }

    internal sealed class PaymentDeclinedErrorCode : ErrorCode
    {
        public static readonly PaymentDeclinedErrorCode Instance = new();
        protected override String Name => nameof(PaymentDeclinedError);
    }
}
```

##### Using Custom Errors

Custom errors work seamlessly with the Result pattern:

```csharp
public class CustomerService
{
    public Result<Customer> GetCustomer(String customerId)
    {
        var customer = _repository.FindById(customerId);
        
        if (customer is null)
            return new CustomerNotFoundError(
                $"Customer with ID '{customerId}' was not found",
                customerId);

        return customer;
    }

    public Result<PaymentConfirmation> ProcessPayment(PaymentRequest request)
    {
        var result = _paymentGateway.Charge(request);
        
        if (!result.Success)
            return new PaymentDeclinedError(
                "Payment was declined by the payment processor",
                result.DeclineReason,
                result.TransactionId);

        return new PaymentConfirmation(result.TransactionId);
    }
}
```

##### Type-Safe Error Handling

Use pattern matching or `IsErrorType<T>()` to handle custom errors:

```csharp
// Using pattern matching
var result = customerService.GetCustomer(customerId);
var message = result.Match(
    onSuccess: customer => $"Welcome, {customer.Name}!",
    onFailure: error => error switch
    {
        CustomerNotFoundError notFound => 
            $"No customer found with ID: {notFound.CustomerId}",
        PaymentDeclinedError declined => 
            $"Payment declined: {declined.DeclineReason} (Ref: {declined.TransactionId})",
        _ => $"Error: {error.Description}"
    });

// Using IsErrorType<T>()
if (result.IsFailure && result.Error.IsErrorType<CustomerNotFoundError>())
{
    var notFoundError = (CustomerNotFoundError)result.Error;
    _logger.LogWarning("Customer lookup failed for ID: {CustomerId}", notFoundError.CustomerId);
}
```

##### JSON Serialization

Custom errors automatically serialize and deserialize without any configuration:

```csharp
// Serialization preserves custom properties
Result<Order> result = new PaymentDeclinedError(
    "Card declined",
    "Insufficient funds",
    "TXN-12345");

var json = JsonSerializer.Serialize(result);
// {
//   "IsSuccess": false,
//   "Error": {
//     "$type": "MyApp.PaymentDeclinedError, MyApp",
//     "Code": { "$type": "...", "Name": "PaymentDeclinedError" },
//     "Description": "Card declined",
//     "DeclineReason": "Insufficient funds",
//     "TransactionId": "TXN-12345"
//   }
// }

// Deserialization restores exact type
var deserialized = JsonSerializer.Deserialize<Result<Order>>(json);
deserialized.Error.GetType(); // PaymentDeclinedError
((PaymentDeclinedError)deserialized.Error).TransactionId; // "TXN-12345"
```

**How it works:**
- The `ErrorJsonConverter` uses reflection to discover all properties on your custom error type
- During serialization, it writes the fully-qualified type name as `$type` discriminator
- During deserialization, it loads the type and invokes the constructor with matching parameter names
- All public properties (including custom ones) are automatically included

**Cross-assembly support:**
Custom errors defined in any assembly will serialize correctly, even if the consuming application has no knowledge of them at compile time. The type discriminator ensures the correct type is reconstructed during deserialization.

#### Exception Handling

The `Error.Exception()` method is available to convert exceptions to errors, but its use should be **rare and discouraged**.

**Why it exists:**
In Blazor applications, unhandled exceptions can crash the entire app, forcing a page reload. Converting exceptions to errors provides a recovery path.

**Important limitations:**
```csharp
try
{
    await externalService.CallAsync();
}
catch (Exception ex)
{
    // ⚠️ This deliberately loses stack trace and inner exceptions
    return Error.Exception(ex);  
}
```

**The conversion is deliberately minimal:**
- ✅ Captures exception type name and message only
- ❌ Loses stack trace (security/privacy concern)
- ❌ Loses inner exceptions
- ❌ Loses custom exception properties

**Why these limitations:**
1. **Performance** - Error values must be lightweight for high-throughput scenarios
2. **Security** - Error descriptions may be visible to end users; stack traces can leak internal details
3. **Serialization** - Full exceptions are not serializable across API boundaries

**Best practice:**
Always log the full exception separately before converting:

```csharp
try
{
    await riskyOperation();
}
catch (Exception ex)
{
    _logger.LogError(ex, "Operation failed for user {UserId}", userId);
    return Error.Exception(ex);  // Only for user-facing message
}
```

#### Input Validation vs Business Rules

The Result pattern is designed for **business rule violations**, not input validation.

**Use FluentValidation for input validation:**
```csharp
public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).MinimumLength(8);
    }
}
```

ASP.NET pipeline validates input and returns 400 BadRequest with detailed validation errors **before** the domain logic executes.

**Use Result<T> for business logic:**
```csharp
public async Task<Result<User>> Handle(CreateUserCommand command)
{
    // Input is already validated by pipeline
    
    // Domain only checks business rules
    if (await _userRepository.EmailExistsAsync(command.Email))
        return Error.Conflict("Email already registered");
        
    return await _userRepository.CreateAsync(command);
}
```

This separation ensures:
- Multiple validation errors caught at API boundary
- Type-safe domain logic with single error per operation
- Clean architecture with clear responsibility boundaries

#### Web API Integration

This is packaged separately in `ProvisionData.WebApi` so you need to install that package as well.

```pwsh
Install-Package ProvisionData.WebApi
```

Or via .NET CLI:

```pwsh
dotnet add package ProvisionData.WebApi
```

Once installed, you can use the extension methods to convert `Result` instances to appropriate HTTP responses.

```csharp
var app = builder.Build();

app.MapGet("/api/users/{id}", (Int32 id, UserService userService) =>
{
    return userService.GetById(id).ToApiResult();
});

app.MapPost("/api/users", (CreateUserRequest request, UserService userService) =>
{
    return userService.Create(request)
        .ToCreatedResult($"/api/users/{request.Email}");
});

app.MapPost("/api/orders", (CreateOrderRequest request, OrderService orderService) =>
{
    return orderService.ProcessOrder(request).Match(
        onSuccess: confirmation => Results.Ok(confirmation),
        onFailure: error => error switch
        {
            ValidationError => Results.BadRequest(new { error.Code, error.Description }),
            ConflictError => Results.Conflict(new { error.Code, error.Description }),
            _ => Results.Problem(error.Description)
        }
    );
});
```

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

