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

namespace ProvisionData;

/// <summary>
/// Provides a base implementation for test fixtures with standard disposal patterns.
/// </summary>
public abstract class DisposableBase : IAsyncDisposable, IDisposable
{
    private Boolean _isDisposed;

    /// <summary>
    /// Gets a value indicating whether the object has been disposed.
    /// </summary>
    protected Boolean IsDisposed => _isDisposed;

    /// <summary>
    /// Releases all resources used by the test fixture.
    /// </summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Asynchronously releases all resources used by the test fixture.
    /// </summary>
    /// <returns>A task that represents the asynchronous dispose operation.</returns>
    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);

        // Dispose unmanaged resources only; managed already handled by DisposeAsyncCore
        Dispose(disposing: false);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases the unmanaged resources used by the test fixture and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing"><see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release only unmanaged resources.</param>
    /// <remarks>
    /// Derived classes should override this method to dispose of their specific managed resources synchronously.
    /// </remarks>
    protected virtual void Dispose(Boolean disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                // Dispose managed resources in derived classes
            }

            _isDisposed = true;
        }
    }

    /// <summary>
    /// Asynchronously releases managed resources.
    /// </summary>
    /// <returns>A task that represents the asynchronous dispose operation.</returns>
    /// <remarks>
    /// Derived classes should override this method to asynchronously dispose of their specific managed resources.
    /// </remarks>
    protected virtual ValueTask DisposeAsyncCore()
    {
        // Dispose managed resources asynchronously in derived classes
        return ValueTask.CompletedTask;
    }
}
