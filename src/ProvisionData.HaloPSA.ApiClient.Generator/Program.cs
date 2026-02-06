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

using ProvisionData.HaloPSA.ModelGenerator.Models;
using ProvisionData.HaloPSA.ModelGenerator.Services;
using System.Diagnostics.CodeAnalysis;

namespace ProvisionData.HaloPSA.ModelGenerator;

public class Program
{
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "GeneratorOptions and ModelChanges properties are simple types")]
    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "GeneratorOptions and ModelChanges properties are simple types")]
    public static async Task Main(String[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        builder.Services.Configure<GeneratorOptions>(builder.Configuration.GetSection(GeneratorOptions.SectionName));
        builder.Services.Configure<ModelChanges>(builder.Configuration.GetSection(ModelChanges.SectionName));

        builder.Services.AddSingleton<DtoGenerator>();
        builder.Services.AddSingleton<IModelChangeValidator, ModelChangeValidator>();
        builder.Services.AddSingleton<IModelChangeProvider, ModelChangeProvider>();
        builder.Services.AddSingleton<CustomFieldsGenerator>();

        var host = builder.Build();

        var generator = host.Services.GetRequiredService<DtoGenerator>();
        var customFieldsGenerator = host.Services.GetRequiredService<CustomFieldsGenerator>();

        // Generate main models from OpenAPI spec
        await generator.GenerateAsync(CancellationToken.None);

        // Generate custom fields partial classes
        await customFieldsGenerator.GenerateAsync(CancellationToken.None);
    }
}
