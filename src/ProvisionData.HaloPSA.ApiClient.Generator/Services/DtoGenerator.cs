// Provision Data Libraries
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

using Microsoft.Extensions.Options;
using ProvisionData.HaloPSA.ModelGenerator.Models;
using System.Text;
using System.Text.Json;

namespace ProvisionData.HaloPSA.ModelGenerator.Services;

/// <summary>
/// Generates DTOs and JsonSerializerContext files from an OpenAPI specification.
/// </summary>
public partial class DtoGenerator
{
    private readonly ILogger<DtoGenerator> _logger;
    private readonly IModelChangeProvider _changeProvider;
    private readonly GeneratorOptions _options;
    private readonly String _dtoPath;
    private readonly String _dtoNamespace;
    private readonly String _contextsPath;
    private readonly String _contextsNamespace;

    /// <summary>
    /// Category definitions for grouping types into focused JsonSerializerContexts.
    /// Each category is defined by a name and a set of type name prefixes.
    /// </summary>
    private static readonly Dictionary<String, String[]> TypeCategories = new()
    {
        ["Asset"] = ["Asset", "Device"],
        ["Customer"] = ["Customer", "Area", "Client", "Site"],
        ["Ticket"] = ["Ticket", "Fault", "Action"],
        ["Invoice"] = ["Invoice", "Quotation", "Payment"],
        ["Contract"] = ["Contract"],
        ["Item"] = ["Item", "Stock", "Consignment"],
        ["User"] = ["User", "Uname", "Agent"],
        ["Vendor"] = ["Vendor", "Company", "Organisation", "Supplier"],
        ["Configuration"] = ["Configuration", "Config", "Setting", "Setup", "Module", "Field"],
        ["Integration"] = ["Integration", "Webhook", "Azure", "Microsoft", "Slack", "Teams"],
    };

    public DtoGenerator(ILogger<DtoGenerator> logger, IOptions<GeneratorOptions> options, IModelChangeProvider changeProvider)
    {
        _logger = logger;
        _changeProvider = changeProvider;
        _options = options.Value;

        _dtoPath = Path.Combine(_options.OutputBasePath, GeneratorDefaults.DtoPath);
        _dtoNamespace = $"{_options.RootNamespace}.DTO";
        _contextsNamespace = $"{_options.RootNamespace}.Contexts";

        _contextsPath = Path.Combine(_options.OutputBasePath, GeneratorDefaults.ContextsPath);
    }

    /// <summary>
    /// Generates all code from the OpenAPI specification.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task GenerateAsync(CancellationToken cancellationToken = default)
    {
        if (_options.IsValid() == false)
        {
            _logger.LogError("DtoGenerator options are not valid. Please check the configuration.");
            return;
        }

        if (!File.Exists(_options.SpecificationPath))
        {
            _logger.LogError("OpenAPI Specification not found at path: {Path}", _options.SpecificationPath);
            return;
        }

        EnsurePathsAreCreated();

        _logger.LogInformation("Starting code generation from OpenAPI specification at path: {SpecificationPath}", _options.SpecificationPath);

        try
        {
            var json = await File.ReadAllTextAsync(_options.SpecificationPath, cancellationToken);

            await GenerateAsync(json, cancellationToken);

            _logger.LogInformation("DTO generation completed successfully. Output written to: {OutputBasePath}", _options.OutputBasePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during DTO generation: {ErrorMessage}", ex.Message);
        }
    }

    private async Task GenerateAsync(String json, CancellationToken cancellationToken = default)
    {
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        if (root.TryGetProperty("components", out var components) && components.ValueKind == JsonValueKind.Object
            && components.TryGetProperty("schemas", out var schemas) && schemas.ValueKind == JsonValueKind.Object)
        {
            var generatedDtos = new List<String>();
            var counter = 0;

            foreach (var typeElement in schemas.EnumerateObject())
            {
                if (_changeProvider.ShouldSkip(typeElement.Name))
                {
                    _logger.LogInformation("Skipping generation of {SchemaName}: not included in ModelChanges configuration.", typeElement.Name);
                    continue;
                }

                var schema = typeElement.Value;
                GeneratedCode? code = null;

                // Only generate for known types
                if (schema.TryGetProperty("type", out var typeProp) && typeProp.GetString() == "object")
                {
                    code = GenerateDtoCode(typeElement.Name, schema);
                    generatedDtos.Add(code.ClientDtoName);
                    counter++;
                }
                else if (schema.TryGetProperty("enum", out var enumProp) && enumProp.ValueKind == JsonValueKind.Array)
                {
                    code = GenerateEnumCode(typeElement.Name, enumProp);
                    counter++;
                }
                else
                {
                    _logger.LogWarning("Skipping {SchemaName} as it is not an object or enum type.", typeElement.Name);
                    continue;
                }

                var filePath = Path.Combine(_dtoPath, $"{code.ClientDtoName}.g.cs");
                await File.WriteAllTextAsync(filePath, code.Code, cancellationToken);
                _logger.LogInformation("Wrote {File}", filePath);
            }

            _logger.LogInformation("Generated {Count} DTOs/enums from OpenAPI specification.", counter);

            // Generate JsonSerializerContext files
            await GenerateJsonSerializerContextsAsync(generatedDtos, cancellationToken);
        }
        else
        {
            _logger.LogError("{OpenApiSchema} does not contain components.schemas with object definitions.",
                Path.GetFileName(_options.SpecificationPath));
        }
    }

    /// <summary>
    /// Generates multiple focused JsonSerializerContext files for better trimming support.
    /// </summary>
    /// <param name="generatedDtos">List of all generated class names.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    private async Task GenerateJsonSerializerContextsAsync(List<String> generatedDtos, CancellationToken cancellationToken)
    {
        // Group types by category
        var categorizedTypes = new Dictionary<String, List<String>>();
        var uncategorizedTypes = new List<String>();

        foreach (var className in generatedDtos)
        {
            var category = GetTypeCategory(className);
            if (category is not null)
            {
                if (!categorizedTypes.TryGetValue(category, out List<String>? value))
                {
                    value = [];
                    categorizedTypes[category] = value;
                }

                value.Add(className);
            }
            else
            {
                uncategorizedTypes.Add(className);
            }
        }

        // Add uncategorized types to a "Common" category
        if (uncategorizedTypes.Count > 0)
        {
            categorizedTypes["Common"] = uncategorizedTypes;
        }

        // Generate focused context for each category
        foreach (var (category, types) in categorizedTypes)
        {
            await GenerateFocusedContextAsync(category, types, cancellationToken);
        }

        _logger.LogInformation("Generated {Count} focused JsonSerializerContext files plus combined context.",
            categorizedTypes.Count);
    }

    /// <summary>
    /// Determines which category a type belongs to based on its name.
    /// </summary>
    /// <param name="typeName">The type name to categorize.</param>
    /// <returns>The category name, or null if uncategorized.</returns>
    private static String? GetTypeCategory(String typeName)
    {
        foreach (var (category, prefixes) in TypeCategories)
        {
            foreach (var prefix in prefixes)
            {
                if (typeName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    return category;
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Generates a focused JsonSerializerContext for a specific category of types.
    /// </summary>
    /// <param name="category">The category name.</param>
    /// <param name="types">The types in this category.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    private async Task GenerateFocusedContextAsync(String category, List<String> types, CancellationToken cancellationToken)
    {
        var contextName = $"{category}JsonContext";
        var sb = new StringBuilder();

        sb.AppendLine("// <auto-generated>");
        sb.AppendLine("// Generated by ProvisionData.HaloPSA.DtoGenerator");
        sb.AppendLine("// This focused context enables better trimming by only including related types.");
        sb.AppendLine("// </auto-generated>");
        sb.AppendLine();
        sb.AppendLine($"using {_dtoNamespace};");
        sb.AppendLine("using System.Text.Json.Serialization;");
        sb.AppendLine();
        sb.AppendLine($"namespace {_contextsNamespace};");
        sb.AppendLine();

        // Add JsonSerializable attributes for each type
        foreach (var typeName in types.OrderBy(t => t))
        {
            var className = _changeProvider.GetDtoName(typeName);
            sb.AppendLine($"[JsonSerializable(typeof({className}))]"); // {_dtoNamespace}.
            sb.AppendLine($"[JsonSerializable(typeof(List<{className}>))]"); // {_dtoNamespace}.
        }

        sb.AppendLine("[JsonSourceGenerationOptions(");
        sb.AppendLine("    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,");
        sb.AppendLine("    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]");
        sb.AppendLine($"public partial class {contextName} : JsonSerializerContext");
        sb.AppendLine("{");
        sb.AppendLine("}");

        var filePath = Path.Combine(_contextsPath, $"{contextName}.g.cs");
        await File.WriteAllTextAsync(filePath, sb.ToString(), cancellationToken);
        _logger.LogInformation("Generated focused context: {ContextName} with {TypeCount} types", contextName, types.Count);
    }

    private GeneratedCode GenerateEnumCode(String jsonModelName, JsonElement enumProp)
    {
        var className = _changeProvider.GetDtoName(jsonModelName);

        var sb = GetHeader();

        sb.AppendLine($"public enum {className}");
        sb.AppendLine("{");

        var elements = enumProp.EnumerateArray().ToArray();
        for (var i = 0; i < elements.Length; i++)
        {
            var value = elements[i].GetInt32();
            if (value == 0)
            {
                sb.Append($"    None = {value}");
            }
            else if (value > 0)
            {
                sb.Append($"    Value_{value} = {value}");
            }
            else
            {
                sb.Append($"    Undefined_{Math.Abs(value)} = {value}");
            }

            if (i < elements.Length - 1)
            {
                sb.AppendLine(",");
            }
            else
            {
                sb.AppendLine();
            }
        }

        sb.AppendLine("}");
        sb.AppendLine();

        return new GeneratedCode()
        {
            JsonModelName = jsonModelName,
            ClientDtoName = className,
            Code = sb.ToString()
        };
    }

    private GeneratedCode GenerateDtoCode(String jsonModelName, JsonElement schema)
    {
        var className = _changeProvider.GetDtoName(jsonModelName);
        var category = GetTypeCategory(className);
        var classChange = _changeProvider.GetChange(jsonModelName);

        var hasCustomFields = false;
        var changes = new List<ModelChange>();

        if (schema.TryGetProperty("properties", out var properties) && properties.ValueKind == JsonValueKind.Object)
        {
            foreach (var prop in properties.EnumerateObject())
            {
                var change = _changeProvider.GetChange(jsonModelName, prop);
                if (change.ClientPropertyType == "List<IdValueDto>")
                {
                    hasCustomFields = true;
                }

                changes.Add(change);
            }
        }

        var customFieldsInterface = hasCustomFields ? " : IHasCustomFields" : String.Empty;

        var sb = GetHeader();

        sb.AppendLine("#nullable enable");
        sb.AppendLine();
        sb.AppendLine($"// JSON Type Name: {jsonModelName}, Category: {category}");
        sb.AppendLine($"public partial class {className}{customFieldsInterface}");
        sb.AppendLine("{");

        if (classChange?.PrivateConstructor == true)
        {
            sb.AppendLine("    // JSON Deserialization Constructor");
            sb.AppendLine($"    protected {className}() {{ }}");
            sb.AppendLine();
        }

        if (changes.Count > 0)
        {
            foreach (var change in changes)
            {
                if (change.Ignore)
                {
                    sb.AppendLine($"    // Property: {change.JsonPropertyName} Type: {change.JsonPropertyType} ignored by configuration.");
                    sb.AppendLine();
                }
                else
                {

                    var reqired = change.Required ? "required " : String.Empty;
                    var nullableSuffix = String.Empty;

                    if (change.Nullable == true)
                    {
                        nullableSuffix = "?";
                        sb.AppendLine("    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]");
                    }

                    sb.AppendLine($"    [JsonPropertyName(\"{change.JsonPropertyName}\")] // Json Property Type: {change.JsonPropertyType}");
                    sb.AppendLine($"    public {reqired}{change.ClientPropertyType}{nullableSuffix} {change.ClientPropertyName} {{ get; set; }}{change.DefaultValue}");
                    sb.AppendLine();

                }
            }
        }
        else
        {
            sb.AppendLine($"        // NOTE: The schema for {jsonModelName} had no properties defined.");
        }

        sb.AppendLine("}");

        sb.AppendLine();
        sb.AppendLine("#nullable disable");
        sb.AppendLine();

        sb.AppendLine("// Mapperly Attributes");
        foreach (var change in changes.OrderBy(c => c.ClientPropertyName))
        {
            if (String.IsNullOrWhiteSpace(change.ClientPropertyName)
                || change.Ignore)
            {
                continue;
            }

            sb.AppendLine($"// [MapperIgnoreTarget(nameof({_dtoNamespace}.{change.ClientDtoName}.{change.ClientPropertyName}))]");
        }

        return new GeneratedCode
        {
            JsonModelName = jsonModelName,
            ClientDtoName = className,
            Code = sb.ToString()
        };
    }

    private StringBuilder GetHeader()
    {
        var sb = new StringBuilder();
        sb.AppendLine("// <auto-generated>");
        sb.AppendLine("// Generated by ProvisionData.HaloPSA.DtoGenerator");
        sb.AppendLine("// </auto-generated>");
        sb.AppendLine();
        sb.AppendLine($"namespace {_dtoNamespace};");
        sb.AppendLine();
        return sb;
    }

    private void EnsurePathsAreCreated()
    {
        if (!Directory.Exists(_dtoPath))
        {
            _logger.LogInformation("DTOs directory does not exist. Creating at: {Path}", _dtoPath);
            Directory.CreateDirectory(_dtoPath);
        }
        else if (_options.DeletePreviousOutputs)
        {
            DeleteGeneratedFilesFrom(_dtoPath);
        }

        if (!Directory.Exists(_contextsPath))
        {
            _logger.LogInformation("Contexts directory does not exist. Creating at: {Path}", _contextsPath);
            Directory.CreateDirectory(_contextsPath);
        }
        else
        {
            DeleteGeneratedFilesFrom(_contextsPath);
        }
    }

    private void DeleteGeneratedFilesFrom(String path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        var files = Directory.GetFiles(path, "*.g.cs", SearchOption.TopDirectoryOnly);
        foreach (var file in files)
        {
            File.Delete(file);
        }

        _logger.LogInformation("Deleted {Count} previously generated files in directory: {Path}",
            files.Length, path);
    }
}
