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
/// Generates custom fields partial classes from configuration.
/// </summary>
public class CustomFieldsGenerator
{
    private readonly ILogger<CustomFieldsGenerator> _logger;
    private readonly IModelChangeProvider _modelChangeProvider;
    private readonly GeneratorOptions _options;
    private readonly String _dtoPath;
    private readonly String _dtoNamespace;
    private readonly String _configPath;
    private readonly String _contextsPath;

    public CustomFieldsGenerator(
        ILogger<CustomFieldsGenerator> logger,
        IOptions<GeneratorOptions> options,
        IModelChangeProvider modelChangeProvider)
    {
        _logger = logger;
        _modelChangeProvider = modelChangeProvider;
        _options = options.Value;

        // Look for custom-fields-config.json in the generator project directory
        var generatorDir = AppContext.BaseDirectory;
        _configPath = Path.Combine(generatorDir, GeneratorDefaults.CustomFieldsFilename);
        _contextsPath = Path.Combine(_options.OutputBasePath, GeneratorDefaults.ContextsPath);
        _dtoNamespace = $"{_options.RootNamespace}.DTO";
        _dtoPath = Path.Combine(_options.OutputBasePath, GeneratorDefaults.DtoPath);
    }

    /// <summary>
    /// Generates custom fields partial classes for all configured models.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task GenerateAsync(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(_configPath))
        {
            _logger.LogInformation("Custom fields configuration not found at {Path}. Skipping custom fields generation.", _configPath);
            return;
        }

        _logger.LogInformation("Loading custom fields configuration from {Path}", _configPath);

        try
        {
            var json = await File.ReadAllTextAsync(_configPath, cancellationToken);
            var config = JsonSerializer.Deserialize<CustomFieldsConfiguration>(json);

            if (config == null || config.CustomFieldMappings.Count == 0)
            {
                _logger.LogInformation("No custom field mappings found in configuration.");
                return;
            }

            foreach (var (modelName, fieldDefinitions) in config.CustomFieldMappings)
            {
                // Skip special keys like "_comments"
                if (modelName.StartsWith('_'))
                {
                    continue;
                }

                if (fieldDefinitions.Count == 0)
                {
                    _logger.LogWarning("Model {ModelName} has no custom field definitions. Skipping.", modelName);
                    continue;
                }

                await GenerateCustomFieldsForModelAsync(modelName, fieldDefinitions, cancellationToken);
            }

            _logger.LogInformation("Custom fields generation completed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating custom fields: {ErrorMessage}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Generates custom fields partial class for a specific model.
    /// </summary>
    private async Task GenerateCustomFieldsForModelAsync(
        String modelName,
        Dictionary<String, CustomFieldDefinition> fieldDefinitions,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Generating custom fields for model: {ModelName}", modelName);

        // Validate all field definitions
        foreach (var (fieldName, definition) in fieldDefinitions)
        {
            definition.Validate(modelName, fieldName);
        }

        // Build template model
        var templateModel = new CustomFieldsTemplateModel
        {
            ModelName = modelName,
            Fields = fieldDefinitions
                .Select(kvp => CustomFieldModel.FromDefinition(kvp.Key, kvp.Value))
                .OrderBy(f => f.Name)
                .ToList()
        };

        // Generate code
        var code = GenerateCode(templateModel);

        // Write to file
        EnsureOutputPathCreated();
        var fileName = $"{modelName}.CustomFields.g.cs";
        var filePath = Path.Combine(_dtoPath, fileName);
        await File.WriteAllTextAsync(filePath, code, cancellationToken);

        _logger.LogInformation("Generated custom fields file: {FileName} with {FieldCount} fields",
            fileName, templateModel.Fields.Count);
    }

    private void EnsureOutputPathCreated()
    {
        if (!Directory.Exists(_dtoPath))
        {
            Directory.CreateDirectory(_dtoPath);
        }
    }

    /// <summary>
    /// Generates the C# code for custom fields partial class.
    /// </summary>
    private String GenerateCode(CustomFieldsTemplateModel model)
    {
        var sb = new StringBuilder();

        // File header
        sb.AppendLine("// <auto-generated>");
        sb.AppendLine("// Generated by ProvisionData.HaloPSA.DtoGenerator");
        sb.AppendLine("// Changes to this file will be overwritten on next generation.");
        sb.AppendLine("// </auto-generated>");
        sb.AppendLine();
        sb.AppendLine("#nullable enable");
        sb.AppendLine();
        sb.AppendLine($"namespace {_dtoNamespace};");
        sb.AppendLine();

        // DTO declaration
        sb.AppendLine("/// <summary>");
        sb.AppendLine($"/// Custom fields implementation for {model.ModelName}.");
        sb.AppendLine("/// </summary>");
        sb.AppendLine($"public partial class {model.ModelName}");
        sb.AppendLine("{");

        // Static initialization region
        GenerateStaticInitialization(sb, model);

        // Custom field properties region
        GenerateCustomFieldProperties(sb, model);

        sb.AppendLine("}");
        sb.AppendLine();
        sb.AppendLine("#nullable restore");

        return sb.ToString();
    }

    /// <summary>
    /// Generates the static initialization region.
    /// </summary>
    private static void GenerateStaticInitialization(StringBuilder sb, CustomFieldsTemplateModel model)
    {
        sb.AppendLine("    #region Custom Fields - Static Initialization");
        sb.AppendLine();

        sb.AppendLine("#if NET9_0_OR_GREATER");
        sb.AppendLine("    private static readonly Lock MappingLock = new();");
        sb.AppendLine("#else");
        sb.AppendLine("    private static readonly Object MappingLock = new();");
        sb.AppendLine("#endif");

        sb.AppendLine("    private static volatile Boolean Mapped;");

        // Field ID declarations
        foreach (var field in model.Fields)
        {
            sb.AppendLine($"    private static Int32 {field.FieldIdVariable};");
        }

        sb.AppendLine();

        // ApplyFieldMappings method
        GenerateApplyFieldMappingsMethod(sb, model);

        // IHasCustomFields implementation
        sb.AppendLine("    /// <inheritdoc/>");
        sb.AppendLine("    public Boolean FieldsAreMapped => Mapped;");
        sb.AppendLine();
        sb.AppendLine("    /// <inheritdoc/>");
        sb.AppendLine("    public void ApplyFieldMap(IFieldMappingProvider provider) => ApplyFieldMappings(provider);");
        sb.AppendLine();
        sb.AppendLine("    /// <inheritdoc/>");
        sb.AppendLine($"    public String ModelTypeName => nameof({model.ModelName});");
        sb.AppendLine();
        sb.AppendLine("    /// <inheritdoc/>");
        sb.AppendLine("    public IEnumerable<String>? FieldNames");
        sb.AppendLine("        => [");
        for (Int32 i = 0; i < model.Fields.Count; i++)
        {
            var field = model.Fields[i];
            var comma = i < model.Fields.Count - 1 ? "," : "";
            sb.AppendLine($"            nameof({field.Name}){comma}");
        }

        sb.AppendLine("        ];");
        sb.AppendLine();

        // Debug methods
        GenerateDebugMethods(sb, model);

        sb.AppendLine("    #endregion");
        sb.AppendLine();
    }

    /// <summary>
    /// Generates the ApplyFieldMappings method.
    /// </summary>
    private static void GenerateApplyFieldMappingsMethod(StringBuilder sb, CustomFieldsTemplateModel model)
    {
        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// Applies field mappings from the provided configuration.");
        sb.AppendLine("    /// This method is thread-safe and will only execute once.");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    /// <param name=\"provider\">The field mapping provider containing configuration.</param>");
        sb.AppendLine("    /// <exception cref=\"InvalidOperationException\">");
        sb.AppendLine("    /// Thrown when required field mappings are not configured.");
        sb.AppendLine("    /// </exception>");
        sb.AppendLine("    /// <remarks>");
        sb.AppendLine("    /// <para>This method uses double-checked locking for thread safety.</para>");

        if (model.HasRequiredFields)
        {
            sb.AppendLine($"    /// <para>Required fields: {model.GetRequiredFieldsList()}</para>");
        }

        sb.AppendLine("    /// </remarks>");
        sb.AppendLine("    public static void ApplyFieldMappings(IFieldMappingProvider provider)");
        sb.AppendLine("    {");
        sb.AppendLine("        if (!Mapped)");
        sb.AppendLine("        {");
        sb.AppendLine("            lock (MappingLock)");
        sb.AppendLine("            {");
        sb.AppendLine("                if (!Mapped)");
        sb.AppendLine("                {");

        // Initialize field IDs
        foreach (var field in model.Fields)
        {
            sb.AppendLine($"                    {field.FieldIdVariable} = provider.GetFieldId(nameof({model.ModelName}), nameof({field.Name}));");
        }

        // Required field validation
        if (model.HasRequiredFields)
        {
            sb.AppendLine();
            sb.AppendLine("                    // Validate required field mappings");
            sb.AppendLine($"                    if ({model.GetRequiredFieldsCondition()})");
            sb.AppendLine("                    {");
            sb.AppendLine("                        throw new InvalidOperationException(");
            sb.AppendLine($"                            $\"Required field mappings not configured for {{nameof({model.ModelName})}}. \" +");
            sb.AppendLine($"                            $\"Check configuration section 'HaloPsaApiClient:FieldMappings:{{nameof({model.ModelName})}}'. \" +");
            sb.AppendLine($"                            $\"Required fields: {model.GetRequiredFieldsList()}\");");
            sb.AppendLine("                    }");
        }

        sb.AppendLine();
        sb.AppendLine("                    Mapped = true;");
        sb.AppendLine("                }");
        sb.AppendLine("            }");
        sb.AppendLine("        }");
        sb.AppendLine("    }");
        sb.AppendLine();
    }

    /// <summary>
    /// Generates debug-only methods for testing.
    /// </summary>
    private static void GenerateDebugMethods(StringBuilder sb, CustomFieldsTemplateModel model)
    {
        sb.AppendLine("#if DEBUG");
        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// Resets field mappings for testing purposes.");
        sb.AppendLine("    /// Only available in DEBUG builds.");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    /// <remarks>");
        sb.AppendLine("    /// This method is intended for unit testing only. It allows tests to reset");
        sb.AppendLine("    /// the static field mapping state between test runs.");
        sb.AppendLine("    /// </remarks>");
        sb.AppendLine("    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]");
        sb.AppendLine("    public static void ResetFieldMappingsForTesting()");
        sb.AppendLine("    {");
        sb.AppendLine("        lock (MappingLock)");
        sb.AppendLine("        {");
        sb.AppendLine("            Mapped = false;");

        foreach (var field in model.Fields)
        {
            sb.AppendLine($"            {field.FieldIdVariable} = 0;");
        }

        sb.AppendLine("        }");
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// Gets current field mapping state for testing/debugging purposes.");
        sb.AppendLine("    /// Only available in DEBUG builds.");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]");
        sb.AppendLine("    public static IReadOnlyDictionary<String, Int32> GetCurrentFieldMappingsForTesting()");
        sb.AppendLine("    {");
        sb.AppendLine("        return new Dictionary<String, Int32>");
        sb.AppendLine("        {");

        for (var i = 0; i < model.Fields.Count; i++)
        {
            var field = model.Fields[i];
            var comma = i < model.Fields.Count - 1 ? "," : "";
            sb.AppendLine($"            [nameof({field.Name})] = {field.FieldIdVariable}{comma}");
        }

        sb.AppendLine("        };");
        sb.AppendLine("    }");
        sb.AppendLine("#endif");
        sb.AppendLine();
    }

    /// <summary>
    /// Generates custom field property definitions.
    /// </summary>
    private static void GenerateCustomFieldProperties(StringBuilder sb, CustomFieldsTemplateModel model)
    {
        sb.AppendLine("    #region Custom Field Properties");
        sb.AppendLine();

        foreach (var field in model.Fields)
        {
            sb.AppendLine("    /// <summary>");
            sb.AppendLine($"    /// Gets or sets the {field.Name} custom field.");
            sb.AppendLine("    /// </summary>");

            if (!String.IsNullOrWhiteSpace(field.Description))
            {
                sb.AppendLine("    /// <remarks>");
                sb.AppendLine($"    /// {field.Description}");
                sb.AppendLine("    /// </remarks>");
            }

            sb.AppendLine("    [JsonIgnore]");
            sb.AppendLine($"    public {field.CSharpType} {field.Name}");
            sb.AppendLine("    {");
            sb.AppendLine($"        get => Fields.{field.GetterMethod}({field.FieldIdVariable});");
            sb.AppendLine($"        set => Fields.SetField({field.SetterParameters});");
            sb.AppendLine("    }");
            sb.AppendLine();
        }

        sb.AppendLine("    #endregion");
    }
}
