# Dapper Column Mapping

## Installation

You can install the ProvisionData.Common package via NuGet Package Manager Console:

```pwsh
Install-Package ProvisionData.Dapper
```

Or via .NET CLI:

```pwsh
dotnet add package ProvisionData.Dapper
```

## Usage

Decorate your class properties with the `[DapperColumn]` attribute to specify custom column mappings for Dapper ORM.

```csharp
public class Invoice : INeedColumnMapping<Invoice>
{
    [DapperColumn("id")]
    public Int32 InvoiceNumber { get; set; }

    public required InvoiceLineItem[] LineItems { get; set; } = [];

    [DapperColumn("customer_fk")]
    public Int32 CustomerId { get; set; }

    [DapperColumn("site_number")]
    public Int32 SiteNumber { get; set; }
}
```

In your project's startup code, register the mappings:

```csharp
// From the current assembly
builder.Services.AddColumnMapping();

// From the specified assembly
builder.Services.AddColumnMapFromAssembly();

// From the assembly containing a specific type
builder.Services.AddColumnMapFromAssemblyContaining<Invoice>();
```

Then once you have built the service provider, apply the mappings:

```csharp
...
var app = builder.Build();

app.Services.MapColumns();
```

If you do not want to clutter up your dependency injection, you can directly apply the mappings like this:

```csharp
ColumnMapping.ApplyMap<Invoice>();
```
