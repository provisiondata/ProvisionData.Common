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

using ProvisionData.HaloPSA.ModelGenerator.Models;
using System.Text;

namespace ProvisionData.HaloPSA.ModelGenerator.Services;

public class ModelChangeValidator : IModelChangeValidator
{
    public Boolean IsValid(ModelChange change, out String error)
    {
        error = String.Empty;

        if (String.IsNullOrWhiteSpace(change.JsonModelName))
        {
            error = "- JsonModelName is required.";
            return false;
        }

        var sb = new StringBuilder();

        // Model Name Change requires ClientDtoName.
        if (!String.IsNullOrEmpty(change.ClientDtoName))
        {
            // Model Name Change should have ClientDtoName and nothing else.
            if (!String.IsNullOrEmpty(change.JsonPropertyName))
            {
                sb.Append("- When ClientDtoName is provided, JsonPropertyName MUST be null (default). ");
            }

            if (!String.IsNullOrEmpty(change.ClientPropertyName))
            {
                sb.Append("- When ClientDtoName is provided, ClientPropertyName MUST be null (default). ");
            }

            if (change.DefaultValue is not null)
            {
                sb.Append("- When ClientDtoName is provided, DefaultValue MUST be null (default).");
            }

            if (change.Ignore == true)
            {
                sb.Append("- When ClientDtoName is provided, Ignore MUST be false (default).");
            }

            if (change.Nullable is not null)
            {
                sb.Append("- When ClientDtoName is provided, Nullable MUST be null (default).");
            }

            if (change.Required == true)
            {
                sb.Append("- When ClientDtoName is provided, Required MUST be false (default).");
            }
        }
        else if (!String.IsNullOrEmpty(change.JsonPropertyName))
        {
            // Property Change
            // If JsonPropertyName is provided, one or more of ClientPropertyName, DefaultValue, Ignore, Nullable, or Required must also be provided
            if (String.IsNullOrWhiteSpace(change.ClientPropertyName)
                && change.DefaultValue is null
                && change.Ignore != true
                && change.Nullable is null
                && change.Required == false
            )
            {
                sb.Append("- When JsonPropertyName is provided, one or more of ClientPropertyName, DefaultValue, Ignore, Nullable, or Required MUST be provided.");
            }
        }
        else
        {
            sb.Append("- Either ClientDtoName or JsonPropertyName MUST be provided.");
        }

        error = sb.ToString();

        return error.Length == 0;
    }
}
