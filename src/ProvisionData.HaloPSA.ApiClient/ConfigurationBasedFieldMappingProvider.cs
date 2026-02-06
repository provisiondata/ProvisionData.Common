// Provision Data HaloPSA API Client
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

using Microsoft.Extensions.Configuration;

namespace ProvisionData.HaloPSA;

/// <summary>
/// Provides field mappings from application configuration (appsettings.json).
/// </summary>
public class ConfigurationBasedFieldMappingProvider : IFieldMappingProvider
{
    private static readonly String SectionName = $"{HaloPsaApiClientOptions.SectionName}:FieldMappings";

    private readonly IConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigurationBasedFieldMappingProvider"/> class.
    /// </summary>
    /// <param name="configuration">The configuration instance.</param>
    public ConfigurationBasedFieldMappingProvider(IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        _configuration = configuration;
    }

    /// <inheritdoc/>
    public Int32 GetFieldId(String modelType, String fieldName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(modelType);
        ArgumentException.ThrowIfNullOrWhiteSpace(fieldName);

        var key = $"{SectionName}:{modelType}:{fieldName}";
        var value = _configuration[key];

        return Int32.TryParse(value, out var id) ? id : 0;
    }
}
