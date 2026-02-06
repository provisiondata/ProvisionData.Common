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

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using ProvisionData.HaloPSA.DTO;
using System.Diagnostics.CodeAnalysis;

namespace ProvisionData.HaloPSA;

/// <summary>
/// Utility methods for configuring the HaloPSA API client.
/// </summary>
public static class ConfigurationExtensions
{
    /// <summary>
    /// Adds the HaloPSA API client to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration instance.</param>
    /// <returns>An <see cref="IHttpClientBuilder"/> for further configuration.</returns>
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "HaloPsaApiClientOptions properties are simple types")]
    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "HaloPsaApiClientOptions properties are simple types")]
    public static IHttpClientBuilder AddHaloPsaApiClient(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        // Configure ReaderOptions from configuration
        services.Configure<HaloPsaApiClientOptions>(configuration.GetSection(HaloPsaApiClientOptions.SectionName));

        // Register singleton services if not already registered
        services.TryAddSingleton(TimeProvider.System);
        services.TryAddSingleton<IAuthTokenProvider, AuthTokenProvider>();
        services.TryAddSingleton<IFieldMappingProvider, ConfigurationBasedFieldMappingProvider>();

        // Register HttpClient with factory pattern
        return services.AddHttpClient<ApiClient>((sp, httpClient) =>
        {
            // Additional HttpClient configuration can be done here if needed
            var options = sp.GetRequiredService<IOptions<HaloPsaApiClientOptions>>().Value;
            httpClient.BaseAddress = new Uri(options.ApiUrl);
            httpClient.Timeout = TimeSpan.FromMinutes(5);
        });
    }

    /// <summary>
    /// Ensures that custom field mappings are applied for models implementing custom fields within the service provider
    /// context.
    /// </summary>
    /// <remarks>This method configures field mappings for models that implement custom fields by invoking the
    /// mapping provider registered in the service provider. Call this extension before using services that depend on
    /// custom field mappings to ensure correct behavior.</remarks>
    /// <param name="serviceProvider">The service provider instance used to resolve field mapping dependencies. Cannot be null.</param>
    /// <returns>The same service provider instance after custom field mappings have been applied.</returns>
    public static IServiceProvider EnsureCustomFieldsHaveBeenMapped(this IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);

        var mapper = serviceProvider.GetRequiredService<IFieldMappingProvider>();

        // Apply field mappings for models that implement IHasCustomFields
        AssetDto.ApplyFieldMappings(mapper);

        return serviceProvider;
    }

    /// <summary>
    /// Adds the HaloPSA API client to the service collection with manual configuration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureOptions">Action to configure the ReaderOptions.</param>
    /// <returns>An <see cref="IHttpClientBuilder"/> for further configuration.</returns>
    public static IHttpClientBuilder AddHaloPsaApiClient(
        this IServiceCollection services,
        Action<HaloPsaApiClientOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configureOptions);

        services.AddOptions();

        // Configure ReaderOptions using action
        services.Configure(configureOptions);

        // Register singleton services if not already registered
        services.TryAddSingleton(TimeProvider.System);
        services.TryAddSingleton<IAuthTokenProvider, AuthTokenProvider>();
        services.TryAddSingleton<IFieldMappingProvider, ConfigurationBasedFieldMappingProvider>();

        // Register HttpClient with factory pattern
        return services.AddHttpClient<ApiClient>((sp, httpClient) =>
        {
            var options = sp.GetRequiredService<IOptions<HaloPsaApiClientOptions>>().Value;
            httpClient.BaseAddress = new Uri(options.ApiUrl);
            httpClient.Timeout = TimeSpan.FromMinutes(5);
        });
    }
}
