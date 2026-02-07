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

using System.Data;

namespace ProvisionData.Data;

/// <summary>
/// Provides methods to create database connection instances.
/// </summary>
public interface IDbConnectionFactory
{
    /// <summary>
    /// Creates and returns a new database connection instance.
    /// </summary>
    /// <remarks>The returned connection may require explicit opening, depending on the implementation. It is
    /// the caller's responsibility to close and dispose of the connection when it is no longer needed
    /// unless it's lifetime is being managed through dependency injection.</remarks>
    /// <returns>An <see cref="IDbConnection"/> representing an open or ready-to-use database connection. The caller is
    /// responsible for managing the connection's lifetime.</returns>
    IDbConnection CreateConnection();

    /// <summary>
    /// Asynchronously creates and opens a new database connection.
    /// </summary>
    /// <remarks>The returned connection is opened and ready for use. Callers are responsible for closing and
    /// disposing the connection when it is no longer needed unless it's lifetime is being managed through
    /// dependency injection.</remarks>
    /// <returns>A task that represents the asynchronous operation. The task result contains an open <see cref="IDbConnection"/>
    /// instance that must be disposed by the caller.</returns>
    Task<IDbConnection> CreateConnectionAsync();
}
