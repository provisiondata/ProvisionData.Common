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

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ProvisionData.Testing;

public abstract class IntegrationTestBase<TSut, TFixture> :
    IClassFixture<TFixture>,
    IDisposable
    where TSut : notnull
    where TFixture : class, ITestFixture
{
    private readonly TFixture _fixture;
    private readonly Lazy<TSut> _lazySut;
    private Boolean _disposed;

    public IntegrationTestBase(TFixture fixture)
    {
        _fixture = fixture;
        _fixture.BeginTest();
        _lazySut = new Lazy<TSut>(() => _fixture.Services.GetRequiredService<TSut>());
    }

    protected IConfiguration Configuration => _fixture.Configuration;
    protected IServiceProvider Services => _fixture.Services;
    protected CancellationToken CancellationToken => TestContext.Current.CancellationToken;
    protected TSut SUT => _lazySut.Value;

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

    public void Dispose()
    {
        _fixture.EndTest();
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
