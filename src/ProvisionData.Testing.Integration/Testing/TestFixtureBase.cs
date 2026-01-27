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

public abstract class TestFixtureBase : IDisposable, ITestFixture
{
    private Boolean _disposedValue;

    public abstract IConfiguration Configuration { get; }
    public abstract IServiceProvider Services { get; }

    public abstract void BeginTest();
    public abstract void EndTest();

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

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
