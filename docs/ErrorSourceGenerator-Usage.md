# Error Source Generator Usage Guide

## Overview

The `ErrorSourceGenerator` automatically generates boilerplate code for custom error types in the Result Pattern library. It uses **ModuleInitializers** to automatically register errors across assemblies with zero developer ceremony.

## Quick Start

### 1. Define a Custom Error

```csharp
public sealed partial class PaymentError : Error
{
    public String TransactionId { get; init; }
    public PaymentFailureReason Reason { get; init; }
}
```

### 2. That's It!

The generator automatically creates:
- `PaymentErrorCode` class
- Constructor with all properties + description
- ModuleInitializer for automatic registration

## What Gets Generated

For the `PaymentError` above, the generator creates:

### Error Code Class
```csharp
internal sealed class PaymentErrorCode : ProvisionData.ResultPattern.ErrorCode
{
    public static readonly PaymentErrorCode Instance = new();
    protected override System.String Name => "PaymentError";
}
```

### Constructor
```csharp
public sealed partial class PaymentError
{
    public PaymentError(
        string transactionId,
        PaymentFailureReason reason,
        System.String description)
        : base(PaymentErrorCode.Instance, description)
    {
        this.TransactionId = transactionId;
        this.Reason = reason;
    }
}
```

### ModuleInitializer (Automatic Registration)
```csharp
internal static class ErrorTypeRegistryInitializer
{
    [System.Runtime.CompilerServices.ModuleInitializer]
    internal static void Initialize()
    {
        ProvisionData.ResultPattern.ErrorTypeRegistry.Register<PaymentError>();
    }
}
```

## Usage Example

```csharp
var error = new PaymentError(
    transactionId: "TXN-12345",
    reason: PaymentFailureReason.InsufficientFunds,
    description: "Payment failed due to insufficient funds");

// Use in Result
return Result<Payment>.Failure(error);
```

## Pattern Requirements

1. **Must inherit from `Error`**
2. **Must be `partial`**
3. **Must be `sealed`**
4. **Properties must have `{ get; init; }`** (not `required`)

## Why No `required` Keyword?

The `required` keyword conflicts with constructor-based initialization. Since the generator creates a constructor that sets all properties, the constructor parameters make the properties effectively required.

```csharp
// ❌ Don't do this
public sealed partial class MyError : Error
{
    public required String Value { get; init; }
}

// ✅ Do this instead
public sealed partial class MyError : Error
{
    public String Value { get; init; }
}
```

## Cross-Assembly Registration

The ModuleInitializer approach means errors are **automatically registered** when their assembly loads, regardless of which assembly they're defined in.

```csharp
// In any assembly that references ProvisionData.ResultPattern
public sealed partial class OrderNotFoundError : Error
{
    public String OrderId { get; init; }
}

// Automatically registered in ErrorTypeRegistry - no manual registration needed!
var allErrors = ErrorTypeRegistry.ErrorTypes; // Includes OrderNotFoundError
```

## Development Workflow

When working on the generator itself:

1. Run `.\build-generator.ps1` to rebuild the generator
2. Changes will be picked up in consuming projects

To view generated files:
```powershell
dotnet build YourProject.csproj `
    /p:EmitCompilerGeneratedFiles=true `
    /p:CompilerGeneratedFilesOutputPath=obj\GeneratedFiles
```

## Advanced Scenarios

### Errors with No Additional Properties

```csharp
public sealed partial class DatabaseConnectionError : Error
{
    // No additional properties
}

// Usage
var error = new DatabaseConnectionError(
    description: "Failed to connect to database");
```

### Errors with Complex Types

```csharp
public sealed partial class ValidationError : Error
{
    public Dictionary<String, String[]> Errors { get; init; }
}

// Usage
var error = new ValidationError(
    errors: new Dictionary<String, String[]> { ["Email"] = ["Invalid format"] },
    description: "Validation failed");
```

## Best Practices

1. **Keep error properties immutable** - Use `{ get; init; }` not `{ get; set; }`
2. **Document your properties** - Add XML comments
3. **Group related errors** - Keep domain errors in the same namespace
4. **Use descriptive names** - Error class names should end with "Error"

## Troubleshooting

### Generator changes not reflected

Run:
```powershell
.\build-generator.ps1
```

Or manually:
```powershell
dotnet clean
dotnet build --no-incremental
```

### Errors not in ErrorTypeRegistry

Ensure:
1. Assembly with errors has been loaded
2. Error classes are `partial` and inherit from `Error`
3. Project references the generator correctly

## Technical Details

### ModuleInitializer

The `[ModuleInitializer]` attribute (introduced in .NET 5) ensures the registration code runs automatically when the assembly is loaded, before any code in that assembly executes.

Benefits:
- **Zero ceremony** - No manual registration needed
- **Cross-assembly** - Works across any assembly
- **Compile-time safety** - Type errors caught at compile time
- **Performance** - Runs once per assembly load

### Why This Pattern?

This pattern provides:
- **Type safety** - Compile-time checking of error properties
- **Discoverability** - All errors in `ErrorTypeRegistry`
- **Consistency** - Uniform error creation pattern
- **Minimal boilerplate** - Generator does the heavy lifting
