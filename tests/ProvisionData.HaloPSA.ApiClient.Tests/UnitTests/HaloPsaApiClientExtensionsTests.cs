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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ProvisionData.HaloPSA.UnitTests;

public class HaloPsaApiClientExtensionsTests
{
    [Fact]
    public void AddHaloPsaApiClient_WithConfiguration_ShouldRegisterServices()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<String, String>
            {
                [$"{HaloPsaApiClientOptions.SectionName}:AuthUrl"] = "https://test.halo.local/auth/",
                [$"{HaloPsaApiClientOptions.SectionName}:ApiUrl"] = "https://test.halo.local/api/",
                [$"{HaloPsaApiClientOptions.SectionName}:ClientId"] = "test-client",
                [$"{HaloPsaApiClientOptions.SectionName}:ClientSecret"] = "test-secret",
                [$"{HaloPsaApiClientOptions.SectionName}:PageSize"] = "100"
            }!)
            .Build();

        services.AddSingleton<IConfiguration>(configuration);

        // Act
        var httpClientBuilder = services.AddHaloPsaApiClient(configuration);

        // Assert
        httpClientBuilder.Should().NotBeNull();

        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetRequiredService<IOptions<HaloPsaApiClientOptions>>();
        var timeProvider = serviceProvider.GetRequiredService<TimeProvider>();
        var client = serviceProvider.GetRequiredService<ApiClient>();

        options.Value.AuthUrl.Should().Be("https://test.halo.local/auth/");
        options.Value.ApiUrl.Should().Be("https://test.halo.local/api/");
        options.Value.ClientId.Should().Be("test-client");
        options.Value.ClientSecret.Should().Be("test-secret");
        options.Value.PageSize.Should().Be(100);
        timeProvider.Should().NotBeNull();
        client.Should().NotBeNull();
    }

    [Fact]
    public void AddHaloPsaApiClient_WithCustomConfigurationSection_ShouldRegisterServices()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<String, String>
            {
                ["CustomSection:AuthUrl"] = "https://custom.halo.local/auth/",
                ["CustomSection:ApiUrl"] = "https://custom.halo.local/api/",
                ["CustomSection:ClientId"] = "custom-client",
                ["CustomSection:ClientSecret"] = "custom-secret"
            }!)
            .Build();

        // Act
        HaloPsaApiClientOptions.SectionName = "CustomSection";
        var httpClientBuilder = services.AddHaloPsaApiClient(configuration);

        // Assert
        httpClientBuilder.Should().NotBeNull();

        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetRequiredService<IOptions<HaloPsaApiClientOptions>>();

        options.Value.AuthUrl.Should().Be("https://custom.halo.local/auth/");
        options.Value.ApiUrl.Should().Be("https://custom.halo.local/api/");
        options.Value.ClientId.Should().Be("custom-client");
        options.Value.ClientSecret.Should().Be("custom-secret");
    }

    [Fact]
    public void AddHaloPsaApiClient_WithAction_ShouldRegisterServices()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();
        services.AddSingleton<IConfiguration>(configuration);

        // Act
        var httpClientBuilder = services.AddHaloPsaApiClient(options =>
        {
            options.AuthUrl = "https://action.halo.local/auth/";
            options.ApiUrl = "https://action.halo.local/api/";
            options.ClientId = "action-client";
            options.ClientSecret = "action-secret";
            options.PageSize = 200;
        });

        // Assert
        httpClientBuilder.Should().NotBeNull();

        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetRequiredService<IOptions<HaloPsaApiClientOptions>>();
        var client = serviceProvider.GetRequiredService<ApiClient>();

        options.Value.AuthUrl.Should().Be("https://action.halo.local/auth/");
        options.Value.ApiUrl.Should().Be("https://action.halo.local/api/");
        options.Value.ClientId.Should().Be("action-client");
        options.Value.ClientSecret.Should().Be("action-secret");
        options.Value.PageSize.Should().Be(200);
        client.Should().NotBeNull();
    }

    [Fact]
    public void AddHaloPsaApiClient_WithConfiguration_ShouldThrowArgumentNullException_WhenServicesIsNull()
    {
        // Arrange
        IServiceCollection services = null!;
        var configuration = new ConfigurationBuilder().Build();

        // Act
        var exception = Record.Exception(() => services.AddHaloPsaApiClient(configuration));

        // Assert
        exception.Should().BeOfType<ArgumentNullException>();
    }

    [Fact]
    public void AddHaloPsaApiClient_WithConfiguration_ShouldThrowArgumentNullException_WhenConfigurationIsNull()
    {
        // Arrange
        var services = new ServiceCollection();
        IConfiguration configuration = null!;

        // Act
        var exception = Record.Exception(() => services.AddHaloPsaApiClient(configuration));

        // Assert
        exception.Should().BeOfType<ArgumentNullException>();
    }

    [Fact]
    public void AddHaloPsaApiClient_WithAction_ShouldThrowArgumentNullException_WhenServicesIsNull()
    {
        // Arrange
        IServiceCollection services = null!;

        // Act
        var exception = Record.Exception(() => services.AddHaloPsaApiClient(configureOptions));

        // Assert
        exception.Should().BeOfType<ArgumentNullException>();

        static void configureOptions(HaloPsaApiClientOptions options)
        {
        }
    }

    [Fact]
    public void AddHaloPsaApiClient_WithAction_ShouldThrowArgumentNullException_WhenConfigureOptionsIsNull()
    {
        // Arrange
        var services = new ServiceCollection();
        Action<HaloPsaApiClientOptions> configureOptions = null!;

        // Act
        var exception = Record.Exception(() => services.AddHaloPsaApiClient(configureOptions));

        // Assert
        exception.Should().BeOfType<ArgumentNullException>();
    }

    [Fact]
    public void AddHaloPsaApiClient_ShouldRegisterTimeProviderOnlyOnce()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<String, String>
            {
                ["ApiClient:ClientId"] = "test"
            }!)
            .Build();

        // Act
        services.AddHaloPsaApiClient(configuration);
        services.AddHaloPsaApiClient(configuration); // Add twice

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var timeProviders = serviceProvider.GetServices<TimeProvider>().ToList();

        // Should only have one TimeProvider registered despite adding twice
        timeProviders.Count.Should().Be(1);
    }
}
