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
using System.Globalization;

namespace ProvisionData.HaloPSA;

/// <summary>
/// Extension methods for working with mapped custom fields.
/// </summary>
public static class FieldMappingExtensions
{
    /// <summary>
    /// Gets a field value as a String.
    /// </summary>
    /// <param name="fields">The fields collection.</param>
    /// <param name="fieldId">The field ID.</param>
    /// <returns>The field value, or an empty string if not found.</returns>
    public static String GetString(this IEnumerable<IdValueDto> fields, Int32 fieldId)
    {
        ArgumentNullException.ThrowIfNull(fields);

        if (fieldId == 0)
        {
            return String.Empty;
        }

        return fields.SingleOrDefault(f => f.Id == fieldId)?.Value ?? String.Empty;
    }

    public static Int32? GetInt32(this IHasCustomFields dto, Int32 fieldId)
    {
        if (fieldId == 0)
        {
            return null;
        }

        var value = dto.Fields.SingleOrDefault(f => f.Id == fieldId)?.Value ?? String.Empty;
        if (Int32.TryParse(value, out var result))
        {
            return result;
        }

        return null;
    }

    /// <summary>
    /// Gets a field value as a DateTime.
    /// </summary>
    /// <param name="fields">The fields collection.</param>
    /// <param name="fieldId">The field ID.</param>
    /// <returns>The field value as a DateTime, or null if not found or invalid.</returns>
    public static DateTime? GetDateTime(this IEnumerable<IdValueDto> fields, Int32 fieldId)
    {
        if (fieldId == 0)
        {
            return null;
        }

        var value = fields.SingleOrDefault(f => f.Id == fieldId)?.Value ?? String.Empty;

        if (DateTime.TryParse(value, CultureInfo.InvariantCulture, out var result))
        {
            return result;
        }

        return null;
    }

    /// <summary>
    /// Gets a field value as a Boolean.
    /// </summary>
    /// <param name="fields">The fields collection.</param>
    /// <param name="fieldId">The field ID.</param>
    /// <returns>The field value as a Boolean, or null if not found or invalid.</returns>
    public static Boolean? GetBoolean(this IEnumerable<IdValueDto> fields, Int32 fieldId)
    {
        if (fieldId == 0)
        {
            return null;
        }

        var value = fields.SingleOrDefault(f => f.Id == fieldId)?.Value;

        if (Boolean.TryParse(value, out var result))
        {
            return result;
        }

        return null;
    }

    /// <summary>
    /// Gets a field value as a Decimal.
    /// </summary>
    /// <param name="fields">The fields collection.</param>
    /// <param name="fieldId">The field ID.</param>
    /// <returns>The field value as a Decimal, or null if not found or invalid.</returns>
    public static Decimal? GetDecimal(this IEnumerable<IdValueDto> fields, Int32 fieldId)
    {
        if (fieldId == 0)
        {
            return null;
        }

        var value = fields.SingleOrDefault(f => f.Id == fieldId)?.Value;

        if (Decimal.TryParse(value, CultureInfo.InvariantCulture, out var result))
        {
            return result;
        }

        return null;
    }

    /// <summary>
    /// Sets a field value in the fields collection.
    /// </summary>
    /// <param name="fields">The fields collection.</param>
    /// <param name="fieldId">The field ID.</param>
    /// <param name="value">The value to set.</param>
    public static void SetField(this ICollection<IdValueDto> fields, Int32 fieldId, String? value)
    {
        ArgumentNullException.ThrowIfNull(fields);

        if (fieldId == 0)
        {
            return;
        }

        var existingField = fields.SingleOrDefault(f => f.Id == fieldId);
        if (existingField is not null)
        {
            existingField.Value = value!;
        }
        else
        {
            fields.Add(new IdValueDto { Id = fieldId, Value = value! });
        }
    }

    /// <summary>
    /// Sets a DateTime field value in the fields collection.
    /// </summary>
    /// <param name="fields">The fields collection.</param>
    /// <param name="dto">The DTO containing the fields (needed for consistency but not used).</param>
    /// <param name="fieldId">The field ID.</param>
    /// <param name="value">The DateTime value to set.</param>
    public static void SetField(this ICollection<IdValueDto> fields, IHasCustomFields dto, Int32 fieldId, DateTime? value)
    {
        ArgumentNullException.ThrowIfNull(fields);
        ArgumentNullException.ThrowIfNull(dto);

        if (fieldId == 0)
        {
            return;
        }

        var stringValue = value?.ToString("o", CultureInfo.InvariantCulture);
        var existingField = fields.SingleOrDefault(f => f.Id == fieldId);
        if (existingField is not null)
        {
            existingField.Value = stringValue;
        }
        else
        {
            fields.Add(new IdValueDto
            {
                Id = fieldId,
                Value = stringValue
            });
        }
    }

    /// <summary>
    /// Sets a DateTime field value in the fields collection.
    /// </summary>
    /// <param name="dto">The DTO containing the custom fields.</param>
    /// <param name="fieldId">The field ID.</param>
    /// <param name="value">The DateTime value to set.</param>
    public static void SetField(this IHasCustomFields dto, Int32 fieldId, DateTime? value)
    {
        ArgumentNullException.ThrowIfNull(dto);

        if (fieldId == 0)
        {
            return;
        }

        var stringValue = value?.ToString("o", CultureInfo.InvariantCulture);
        var existingField = dto.Fields.SingleOrDefault(f => f.Id == fieldId);
        if (existingField is not null)
        {
            existingField.Value = stringValue;
        }
        else
        {
            dto.Fields.Add(new IdValueDto
            {
                Id = fieldId,
                Value = stringValue
            });
        }
    }
}
