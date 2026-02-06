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

namespace ProvisionData.HaloPSA;

/// <summary>
/// Defines a contract for retrieving authentication tokens asynchronously.
/// </summary>
/// <remarks>Implementations of this interface are responsible for providing valid authentication tokens, which
/// may be used for accessing protected resources or APIs. Token retrieval may involve network operations, caching, or
/// interaction with external authentication services. Implementations should ensure thread safety if tokens are shared
/// across multiple consumers.</remarks>
public interface IAuthTokenProvider
{
    /// <summary>
    /// Asynchronously retrieves an authentication token for the current user or context.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the token retrieval operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="AuthToken"/> if
    /// authentication is successful; otherwise, <see langword="null"/>.</returns>
    Task<AuthToken?> GetTokenAsync(CancellationToken cancellationToken);
}
