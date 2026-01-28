// ProvisionData.Common
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

using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ProvisionData.Testing;

/// <summary>
/// Provides a base class for integration tests that use a test fixture with dependency injection.
/// </summary>
/// <typeparam name="TSut">The type of the System Under Test (SUT) to be tested.</typeparam>
/// <typeparam name="TFixture">The type of the test fixture providing services and configuration.</typeparam>
public abstract class IntegrationTestBase<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TSut, TFixture> :
    IClassFixture<TFixture>,
    IDisposable
    where TSut : notnull
    where TFixture : class, ITestFixture
{
    private readonly TFixture _fixture;
    private readonly Lazy<TSut> _lazySut;
    private Boolean _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="IntegrationTestBase{TSut, TFixture}"/> class.
    /// </summary>
    /// <param name="fixture">The test fixture providing services and configuration.</param>
    public IntegrationTestBase(TFixture fixture)
    {
        _fixture = fixture;
        _fixture.BeginTest();
        _lazySut = new Lazy<TSut>(() => _fixture.Services.GetRequiredService<TSut>());
    }

    /// <summary>
    /// Gets the configuration from the test fixture.
    /// </summary>
    protected IConfiguration Configuration => _fixture.Configuration;

    /// <summary>
    /// Gets the dependency injection service provider from the test fixture.
    /// </summary>
    protected IServiceProvider Services => _fixture.Services;

    /// <summary>
    /// Gets the cancellation token from the current test context.
    /// </summary>
    protected CancellationToken CancellationToken => TestContext.Current.CancellationToken;

    /// <summary>
    /// Gets the System Under Test (SUT) instance from the service provider.
    /// </summary>
    /// <remarks>
    /// The SUT is lazily instantiated on first access.
    /// </remarks>
    protected TSut SUT => _lazySut.Value;

    /// <summary>
    /// Releases the unmanaged resources used by the test and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing"><see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release only unmanaged resources.</param>
    protected virtual void Dispose(Boolean disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Managed resources are disposed by the fixture
            }

            _disposed = true;
        }
    }

    /// <summary>
    /// Releases all resources used by the test.
    /// </summary>
    public void Dispose()
    {
        _fixture.EndTest();
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
