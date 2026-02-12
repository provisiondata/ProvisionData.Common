// Provision Data Application Framework
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

using Microsoft.CodeAnalysis;

namespace ProvisionData.ResultPattern.Generators;

public static class DiagnosticDescriptors
{
    public const String MissingAddResultPatternId = "PDRP0001";
    public const String ErrorTypesMustHaveSingleStringConstructorId = "PDRP0002";

    public static readonly DiagnosticDescriptor Missing_AddResultPattern_Invocation = new(
        id: MissingAddResultPatternId,
        title: "ResultPattern services are not configured",
        messageFormat: "ResultPattern types are used but AddResultPattern() is not called on IServiceCollection",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Call services.AddResultPattern() during service registration to enable ResultPattern JSON polymorphism and error handling.",
        customTags: [WellKnownDiagnosticTags.CompilationEnd]
    );

    public static readonly DiagnosticDescriptor DerivedErrorTypesMustHaveSingleStringConstructor = new(
        id: MissingAddResultPatternId,
        title: "ResultPattern services are not configured",
        messageFormat: "ResultPattern types are used but AddResultPattern() is not called on IServiceCollection",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Call services.AddResultPattern() during service registration to enable ResultPattern JSON polymorphism and error handling.",
        customTags: [WellKnownDiagnosticTags.CompilationEnd]
    );
}
