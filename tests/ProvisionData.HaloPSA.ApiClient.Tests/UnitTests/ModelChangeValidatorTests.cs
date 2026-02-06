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
using ProvisionData.HaloPSA.ModelGenerator.Services;

namespace ProvisionData.HaloPSA.UnitTests;

public class ModelChangeValidatorTests
{
    private readonly ModelChangeValidator _validator = new();

    [Fact]
    public void IsValid_ShouldReturnFalse_WhenJsonModelNameIsNull()
    {
        var change = new ModelChange
        {
            JsonModelName = null!,
            ClientDtoName = null,
            JsonPropertyName = null,
            ClientPropertyName = null,
            Nullable = null,
            Required = false
        };

        _validator.IsValid(change, out var error).Should().BeFalse();
    }

    [Fact]
    public void IsValid_ShouldReturnFalse_WhenJsonModelNameIsEmptyOrWhiteSpace()
    {
        var change = new ModelChange
        {
            JsonModelName = "   ",
            ClientDtoName = null,
            JsonPropertyName = null,
            ClientPropertyName = null,
            Nullable = null,
            Required = false
        };

        _validator.IsValid(change, out var error).Should().BeFalse();
    }

    [Fact]
    public void IsValid_ShouldReturnTrue_ForModelRenameWithNoPropertyChanges()
    {
        var change = new ModelChange
        {
            JsonModelName = "Ticket",
            ClientDtoName = "TicketModel",
            JsonPropertyName = null,
            ClientPropertyName = null,
            Nullable = null,
            Required = false
        };

        _validator.IsValid(change, out var error).Should().BeTrue();
    }

    [Fact]
    public void IsValid_ShouldReturnFalse_ForModelRenameWithPropertyChanges()
    {
        var change = new ModelChange
        {
            JsonModelName = "Ticket",
            ClientDtoName = "TicketModel",
            JsonPropertyName = "Id",
            ClientPropertyName = null,
            Nullable = null,
            Required = false
        };

        _validator.IsValid(change, out var error).Should().BeFalse();
    }

    [Fact]
    public void IsValid_ShouldReturnTrue_ForPropertyRename()
    {
        var change = new ModelChange
        {
            JsonModelName = "Ticket",
            JsonPropertyName = "id",
            ClientPropertyName = "Id",
            Nullable = null,
            Required = false
        };

        _validator.IsValid(change, out var error).Should().BeTrue();
    }

    [Fact]
    public void IsValid_ShouldReturnTrue_ForPropertyNullableChange()
    {
        var change = new ModelChange
        {
            JsonModelName = "Ticket",
            JsonPropertyName = "description",
            ClientPropertyName = null,
            Nullable = true,
            Required = false
        };

        _validator.IsValid(change, out var error).Should().BeTrue();
    }

    [Fact]
    public void IsValid_ShouldReturnTrue_ForPropertyRequiredChange()
    {
        var change = new ModelChange
        {
            JsonModelName = "Ticket",
            JsonPropertyName = "summary",
            ClientPropertyName = null,
            Nullable = null,
            Required = true
        };

        _validator.IsValid(change, out var error).Should().BeTrue();
    }

    [Fact]
    public void IsValid_ShouldReturnFalse_ForPropertyChangeWithoutDetails()
    {
        var change = new ModelChange
        {
            JsonModelName = "Ticket",
            JsonPropertyName = "summary",
            ClientPropertyName = null,
            Nullable = null,
            Required = false
        };

        _validator.IsValid(change, out var error).Should().BeFalse();
    }

    [Fact]
    public void IsValid_ShouldReturnFalse_WhenOnlyModelNameProvided()
    {
        var change = new ModelChange
        {
            JsonModelName = "Ticket",
            ClientDtoName = null,
            JsonPropertyName = null,
            ClientPropertyName = null,
            Nullable = null,
            Required = false
        };

        _validator.IsValid(change, out var error).Should().BeFalse();
    }
}
