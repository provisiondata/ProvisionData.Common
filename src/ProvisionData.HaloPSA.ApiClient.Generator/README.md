# ProvisionData.HaloPSA.ApiClient.ModelGenerator

A .NET utility that generates strongly-typed C# DTOs and custom field implementations from the HaloPSA OpenAPI specification.

## Overview

The ModelGenerator is a code generation tool that automates the creation of:

1. **DTO Classes** - C# model classes from HaloPSA OpenAPI schemas
2. **JSON Serializer Contexts** - Optimized System.Text.Json source generators for AOT compilation
3. **Custom Fields Partial Classes** - Type-safe property accessors for HaloPSA custom fields

This eliminates manual DTO creation, reduces errors, and ensures consistency across the API client library.

## Architecture

### Generator Components

The generator consists of two main services:

- **`DtoGenerator`** - Generates DTOs and JsonSerializerContexts from OpenAPI specification
- **`CustomFieldsGenerator`** - Generates custom field partial classes from configuration

Both services run sequentially when the application starts.

### Project Type

This is a **.NET console app that runs once and exits. It's designed to be executed manually during development when:

- The HaloPSA OpenAPI specification updates
- Custom field configurations change
- New DTOs need to be added

## Configuration

### appsettings.json

The generator is configured via `appsettings.json`:

```json
{
  "GeneratorOptions": {
    "SpecificationPath": "../../openapi.json",
    "OutputBasePath": "../ProvisionData.HaloPSA.ApiClient.Contracts",
    "RootNamespace": "ProvisionData.HaloPSA",
    "DeletePreviousOutputs": true
  },
  "ModelChanges": {
    "InitializeArrays": false,
    "IgnoredJsonPropertyNames": [],
    "IgnoredJsonPropertyTypes": [
      "AreaAzureTenant",
      "Attachment",
      "..."
    ],
    "Changes": [
      {
        "JsonModelName": "Area",
        "ClientDtoName": "Customer"
      },
      {
        "JsonModelName": "Device",
        "ClientDtoName": "Asset"
      }
    ]
  }
}
```

#### GeneratorOptions Properties

| Property                | Type    | Description                                                       |
| ----------------------- | ------  | ----------------------------------------------------------------- |
| `SpecificationPath`     | String  | Path to the HaloPSA OpenAPI specification JSON file               |
| `OutputBasePath`        | String  | Base directory for generated code (usually the Contracts project) |
| `RootNamespace`         | String  | Root namespace for generated types                                |
| `DeletePreviousOutputs` | Boolean | Whether to delete existing generated files before generation      |

#### ModelChanges Properties

| Property                   | Type          | Description                                                   |
| -------------------------- | ------------- | ------------------------------------------------------------- |
| `InitializeArrays`         | Boolean       | Whether to initialize collection properties to empty arrays   |
| `IgnoredJsonPropertyNames` | String[]      | Property names to skip during generation                      |
| `IgnoredJsonPropertyTypes` | String[]      | Type names to skip during generation                          |
| `Changes`                  | ModelChange[] | Array of type and property customizations                     |

### ModelChange Configuration

Each entry in the `Changes` array customizes how a type or property is generated:

**Model-Level Change** (renames a class):

```json
{
  "JsonModelName": "Device",
  "ClientDtoName": "Asset"
}
```

**Property-Level Change** (renames/customizes a property):

```json
{
  "JsonModelName": "Asset",
  "JsonPropertyName": "inventory_number",
  "ClientPropertyName": "SerialNumber",
  "Required": true,
  "Nullable": false,
  "DefaultValue": " = String.Empty;"
}
```

#### ModelChange Properties

| Property             | Type    | Description                                   |
| -------------------- | ------- | --------------------------------------------- |
| `JsonModelName`      | String  | **Required**. API type name from OpenAPI spec |
| `ClientDtoName`      | String  | Custom C# class name (model-level only)       |
| `JsonPropertyName`   | String  | API property name (property-level only)       |
| `ClientPropertyName` | String  | Custom C# property name                       |
| `ClientPropertyType` | String  | Override inferred C# type                     |
| `DefaultValue`       | String  | Default value initializer (e.g., `" = 0;"`)   |
| `Nullable`           | Boolean | Whether property can be null                  |
| `Required`           | Boolean | Whether property is required                  |
| `Ignore`             | Boolean | Skip this property during generation          |
| `PrivateConstructor` | Boolean | Generate private default constructor          |

### custom-fields-config.json

Defines custom field mappings for DTOs:

```json
{
  "CustomFieldMappings": {
    "Asset": {
      "Name": {
        "type": "String",
        "required": false,
        "description": "The name or identifier of the asset"
      },
      "SerialNumber": {
        "type": "String",
        "required": true,
        "description": "The hardware serial number of the asset"
      },
      "PurchaseDate": {
        "type": "DateTime",
        "required": false,
        "description": "The date the asset was purchased"
      }
    }
  }
}
```

#### Custom Field Definition Properties

| Property      | Type    | Description                                                     |
| ------------- | ------- | --------------------------------------------------------------- |
| `type`        | String  | C# type: `String`, `Int32`, `DateTime`, `Boolean`, or `Decimal` |
| `required`    | Boolean | Whether the field must be configured at runtime                 |
| `description` | String  | XML documentation for the generated property                    |

## Running the Generator

### From Visual Studio

1. Set the `ProvisionData.HaloPSA.ApiClient.Generator` project as the startup project
2. Press F5 or click Run

### From Command Line

```bash
cd src/ProvisionData.HaloPSA.ApiClient.Generator
dotnet run
```

### Prerequisites

Before running the generator, ensure:

1. **OpenAPI Specification** - Download the latest HaloPSA OpenAPI spec and place it at the configured `SpecificationPath`
2. **Configuration** - Review and update `appsettings.json` with any new types or customizations
3. **Custom Fields** - Update `custom-fields-config.json` with any new custom field definitions

## Generated Output

### Directory Structure

```
OutputBasePath/
├── DTO/
│   └── Generated/
│       ├── Asset.g.cs
│       ├── Asset.CustomFields.g.cs
│       ├── Customer.g.cs
│       ├── Ticket.g.cs
│       └── ...
└── Contexts/
    └── Generated/
        ├── AssetJsonContext.g.cs
        ├── CustomerJsonContext.g.cs
        ├── TicketJsonContext.g.cs
        └── ApiJsonSerializerContext.g.cs
```

### Generated File Types

#### 1. DTO Classes (`{Type}.g.cs`)

Standard C# classes with properties mapped from OpenAPI schemas:

```csharp
// <auto-generated>
// Generated by ProvisionData.HaloPSA.DtoGenerator
// Changes to this file will be overwritten on next generation.
// </auto-generated>

namespace ProvisionData.HaloPSA.DTO;

public partial class Asset
{
    [JsonPropertyName("id")]
    public Int32 Id { get; set; }
    
    [JsonPropertyName("assettype_id")]
    public Int32 AssetTypeId { get; set; }
    
    // ... more properties
}
```

#### 2. Custom Fields Partial Classes (`{Type}.CustomFields.g.cs`)

Partial classes implementing `IHasCustomFields` with strongly-typed property accessors:

```csharp
// <auto-generated>
// Generated by ProvisionData.HaloPSA.DtoGenerator
// Changes to this file will be overwritten on next generation.
// </auto-generated>

namespace ProvisionData.HaloPSA.DTO;

public partial class Asset
{
    private static Int32 _nameFieldId;
    private static Int32 _serialNumberFieldId;
    
    public static void ApplyFieldMappings(IFieldMappingProvider provider)
    {
        if (Mapped) return;
        
        lock (MappingLock)
        {
            if (Mapped) return;
            
            _nameFieldId = provider.GetFieldId(nameof(Asset), nameof(Name));
            _serialNumberFieldId = provider.GetFieldId(nameof(Asset), nameof(SerialNumber));
            
            Mapped = true;
        }
    }
    
    /// <summary>
    /// The name or identifier of the asset
    /// </summary>
    public String? Name
    {
        get => this.GetCustomField<String>(_nameFieldId);
        set => this.SetCustomField(_nameFieldId, value);
    }
    
    // ... more custom field properties
}
```

#### 3. JSON Serializer Contexts (`{Category}JsonContext.g.cs`)

Focused contexts for System.Text.Json source generation (AOT-friendly):

```csharp
// <auto-generated>
// Generated by ProvisionData.HaloPSA.DtoGenerator
// This focused context enables better trimming by only including related types.
// </auto-generated>

using ProvisionData.HaloPSA.DTO;
using System.Text.Json.Serialization;

namespace ProvisionData.HaloPSA.Contexts;

[JsonSerializable(typeof(Asset))]
[JsonSerializable(typeof(AssetList))]
[JsonSerializable(typeof(AssetView))]
[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.Unspecified,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
internal partial class AssetJsonContext : JsonSerializerContext
{
}
```

The generator creates multiple focused contexts (Asset, Customer, Ticket, etc.) plus a combined `ApiJsonSerializerContext` containing all types.

## Type Categorization

The generator automatically categorizes types into focused JsonSerializerContexts based on naming prefixes:

| Category      | Prefixes                                                         |
| ------------- | ---------------------------------------------------------------- |
| Asset         | `Asset`, `Device`                                                |
| Customer      | `Customer`, `Area`, `Client`, `Site`                             |
| Ticket        | `Ticket`, `Fault`, `Action`                                      |
| Invoice       | `Invoice`, `Quotation`, `Payment`                                |
| Contract      | `Contract`                                                       |
| Item          | `Item`, `Stock`, `Consignment`                                   |
| User          | `User`, `Uname`, `Agent`                                         |
| Vendor        | `Vendor`, `Company`, `Organisation`, `Supplier`                  |
| Configuration | `Configuration`, `Config`, `Setting`, `Setup`, `Module`, `Field` |
| Integration   | `Integration`, `Webhook`, `Azure`, `Microsoft`, `Slack`, `Teams` |

Types that don't match any category are placed in a `Common` context.

## Include Pattern

The generator uses an **include pattern** - only types with entries in `ModelChanges.Changes` are generated:

✅ **Included** (has Changes entry):

```json
{
  "JsonModelName": "Device",
  "ClientDtoName": "Asset"
}
```

❌ **Excluded** (no Changes entry):

```
Skipping generation of UnmappedType: not included in ModelChanges configuration.
```

This keeps the generated codebase focused on actively used types.

## Workflow

### Typical Development Workflow

1. **Download OpenAPI Spec** - Get the latest specification from your HaloPSA instance
2. **Update Configuration** - Add/modify entries in `appsettings.json` for new or changed types
3. **Update Custom Fields** - Add new custom field definitions to `custom-fields-config.json`
4. **Run Generator** - Execute the worker service
5. **Review Generated Code** - Check the output files in the Contracts project
6. **Compile & Test** - Build the solution and run tests
7. **Commit Changes** - Commit both configuration and generated files

### When to Run the Generator

- HaloPSA API adds new endpoints or types
- Existing API types change structure
- Custom field requirements change
- Property names need to be made more idiomatic

## Best Practices

### Configuration

- **Use meaningful names** - Map cryptic API names to clear C# names
- **Group related changes** - Organize Changes array by model type
- **Document decisions** - Add comments explaining non-obvious mappings
- **Review ignored types** - Periodically review `IgnoredJsonPropertyTypes` for types that should be included

### Custom Fields

- **Start minimal** - Only define custom fields you actively use
- **Mark required fields** - Use `required: true` for fields that must be configured at construction
- **Add descriptions** - Provide XML documentation for all custom properties
- **Validate locally** - Run tests after generating to ensure field mappings work

### Generated Code

- **Never edit `.g.cs` files** - All changes will be overwritten
- **Use partial classes** - Extend generated DTOs with manual partial classes for custom logic
- **Review diffs** - When regenerating, review Git diffs to catch unexpected changes

## Troubleshooting

### Common Issues

**Generator doesn't produce output**
- Check `SpecificationPath` points to valid OpenAPI JSON
- Verify `OutputBasePath` directory exists and is writable
- Review logs for validation errors

**Type not generated**
- Ensure type has an entry in `ModelChanges.Changes`
- Check type isn't in `IgnoredJsonPropertyTypes`
- Verify OpenAPI spec contains the type

**Custom field compilation errors**
- Ensure DTO implements `IHasCustomFields`
- Verify custom field type names match available types
- Check required fields are marked correctly

**JsonSerializerContext errors**
- Ensure all referenced types are included in context
- Check for circular references in type graphs
- Verify namespace imports are correct

### Logging

The generator uses `ILogger<T>` for structured logging. Review console output for:

- Generation progress
- Skipped types with reasons
- File write confirmations
- Error messages with stack traces

## Dependencies

- **Microsoft.Extensions.Hosting** - Worker Service infrastructure
- **Humanizer.Core** - String manipulation and naming conventions
- **System.Text.Json** - JSON parsing and serialization

## See Also

- [Custom Fields Quick Reference](../../docs/custom-fields-quick-reference.md) - Usage guide for consumers
- [Generator Custom Fields Implementation Guide](../../docs/generator-custom-fields-implementation-guide.md) - Implementation details
- [Generator Model Changes](../../docs/GENERATOR_MODEL_CHANGES.md) - ModelChanges configuration reference

## Contributing

When extending the generator:

1. Add new features as separate services
2. Follow the existing template-based generation pattern
3. Update configuration schema with new options
4. Add comprehensive XML documentation
5. Update this README with new features
