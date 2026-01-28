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

namespace ProvisionData.Testing;

/// <summary>
/// Provides a base implementation for test fixtures with standard disposal patterns.
/// </summary>
public abstract class TestFixtureBase : IDisposable, ITestFixture
{
    private Boolean _disposedValue;

    /// <summary>
    /// Gets the configuration for the test fixture.
    /// </summary>
    public abstract IConfiguration Configuration { get; }

    /// <summary>
    /// Gets the dependency injection service provider for the test fixture.
    /// </summary>
    public abstract IServiceProvider Services { get; }

    /// <summary>
    /// Called before each test to initialize the test fixture.
    /// </summary>
    public abstract void BeginTest();

    /// <summary>
    /// Called after each test to clean up the test fixture.
    /// </summary>
    public abstract void EndTest();

    /// <summary>
    /// Releases the unmanaged resources used by the test fixture and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing"><see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release only unmanaged resources.</param>
    /// <remarks>
    /// Derived classes should override this method to dispose of their specific managed resources.
    /// </remarks>
    protected virtual void Dispose(Boolean disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                // Dispose managed resources in derived classes
            }

            _disposedValue = true;
        }
    }

    /// <summary>
    /// Releases all resources used by the test fixture.
    /// </summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
