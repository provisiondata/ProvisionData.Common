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

namespace ProvisionData.HaloPSA;

/// <summary>
/// Internal token model for OAuth authentication responses.
/// </summary>
public sealed class AuthToken
{
    /// <summary>
    /// Gets or sets the OAuth access token used for API authentication.
    /// </summary>
    [JsonPropertyName("access_token")]
    public String AccessToken { get; set; } = String.Empty;

    /// <summary>
    /// Gets or sets the token type (typically "Bearer") from the authentication response.
    /// </summary>
    [JsonPropertyName("token_type")]
    public String TokenType { get; set; } = String.Empty;

    /// <summary>
    /// Gets or sets the lifetime of the access token in seconds.
    /// </summary>
    [JsonPropertyName("expires_in")]
    public Int32 ExpiresIn { get; set; }

    /// <summary>
    /// Gets or sets the refresh token used to obtain a new access token without re-authenticating.
    /// </summary>
    [JsonPropertyName("refresh_token")]
    public String RefreshToken { get; set; } = String.Empty;

    /// <summary>
    /// Gets or sets the date and time when this token was issued.
    /// </summary>
    public DateTimeOffset IssuedAt { get; set; }

    /// <summary>
    /// Determines whether the token has expired at the specified point in time.
    /// </summary>
    /// <param name="now">The current date and time to check expiration against.</param>
    /// <returns><c>true</c> if the token has expired; otherwise, <c>false</c>.</returns>
    public Boolean IsExpired(DateTimeOffset now)
        => IssuedAt.AddSeconds(ExpiresIn) <= now;
}
