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

using ProvisionData.HaloPSA.DTO;

namespace ProvisionData.HaloPSA;

/// <summary>
/// Source-generated JSON serializer context for HaloPSA API models.
/// </summary>
/// <remarks>
/// <para>
/// This partial class contains types used directly by the ApiClient methods.
/// When you run the model generator, it will create <c>ApiJsonSerializerContext.g.cs</c>
/// which adds all generated model types as a partial class extension.
/// </para>
/// <para>
/// For better trimming support, consider using the focused contexts instead:
/// <list type="bullet">
///   <item><c>HaloPsaDeviceJsonContext</c> - Device/Asset types</item>
///   <item><c>HaloPsaClientJsonContext</c> - Client/Area types</item>
///   <item><c>HaloPsaTicketJsonContext</c> - Ticket/Fault types</item>
///   <item><c>HaloPsaInvoiceJsonContext</c> - Invoice types</item>
///   <item><c>HaloPsaItemJsonContext</c> - Item/Supplier types</item>
/// </list>
/// </para>
/// </remarks>

// Non-generated types
[JsonSerializable(typeof(InstanceInfo))]
[JsonSourceGenerationOptions(
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
public partial class ApiJsonSerializerContext : JsonSerializerContext
{
}
