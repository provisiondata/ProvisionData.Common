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

### Result Pattern

> [Result Pattern in C#]( https://adrianbailador.github.io/blog/44-result-pattern-)
> by [Adrian Bailador](https://adrianbailador.github.io/)

The `Result` class provides a way to represent the outcome of operations,
encapsulating success and failure states along with relevant data or error messages.

#### Basic Usage

```csharp
public class UserService
{
    private readonly IUserRepository _repository;

    public Result<User> GetById(int id)
    {
        var user = _repository.Find(id);
        
        if (user is null)
            return Error.NotFound("User.NotFound", $"User with ID {id} was not found");

        return user; // Implicit conversion to Result<User>.Success
    }

    public Result<User> Create(CreateUserRequest request)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(request.Email))
            return Error.Validation("User.EmailRequired", "Email is required");

        if (_repository.ExistsByEmail(request.Email))
            return Error.Conflict("User.EmailExists", "A user with this email already exists");

        // Create user
        var user = new User(request.Name, request.Email);
        _repository.Add(user);

        return user;
    }
}```

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
        return Error.Validation("Order.NoItems", "Order must contain at least one item");
    
    return new Order(request.CustomerId, request.Items);
}

private Result<Order> CheckInventory(Order order)
{
    foreach (var item in order.Items)
    {
        if (!_inventory.IsAvailable(item.ProductId, item.Quantity))
            return Error.Conflict("Order.OutOfStock", $"Product {item.ProductId} is out of stock");
    }
    return order;
}
```

#### Using Match

```csharp
var message = userService.GetById(userId).Match(
    onSuccess: user => $"Welcome, {user.Name}!",
    onFailure: error => $"Error: {error.Description}"
);
```

#### Domain Errors

```csharp
public static class DomainErrors
{
    public static class User
    {
        public static Error NotFound(int id) =>
            Error.NotFound("User.NotFound", $"User with ID {id} was not found");

        public static Error EmailAlreadyExists(string email) =>
            Error.Conflict("User.EmailExists", $"Email {email} is already registered");

        public static Error InvalidEmail =>
            Error.Validation("User.InvalidEmail", "The email format is invalid");

        public static Error PasswordTooWeak =>
            Error.Validation("User.PasswordTooWeak", 
                "Password must be at least 8 characters with uppercase, lowercase, and digits");
    }

    public static class Order
    {
        public static Error NotFound(Guid id) =>
            Error.NotFound("Order.NotFound", $"Order {id} was not found");

        public static Error EmptyCart =>
            Error.Validation("Order.EmptyCart", "Cannot create order with empty cart");

        public static Error InsufficientStock(string productId) =>
            Error.Conflict("Order.InsufficientStock", $"Insufficient stock for product {productId}");

        public static Error PaymentFailed(string reason) =>
            Error.Failure("Order.PaymentFailed", $"Payment failed: {reason}");
    }
}
```

#### Web API Integration

This is packaged separately in `ProvisionData.WebApi` so you need to install that package as well.

```pwsh
Install-Package ProvisionData.Common
```

Or via .NET CLI:

```pwsh
dotnet add package ProvisionData.Common
```

Once installed, you can use the extension methods to convert `Result` instances to appropriate HTTP responses.

```csharp
var app = builder.Build();

app.MapGet("/api/users/{id}", (int id, UserService userService) =>
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
