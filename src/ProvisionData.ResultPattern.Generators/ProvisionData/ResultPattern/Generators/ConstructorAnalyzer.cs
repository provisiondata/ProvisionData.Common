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
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace ProvisionData.ResultPattern.Generators;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ConstructorAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        => [DiagnosticDescriptors.DerivedErrorTypesMustHaveSingleStringConstructor];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

        context.EnableConcurrentExecution();

        context.RegisterSymbolAction(AnalyzeNamedType, SymbolKind.NamedType);
    }

    private static void AnalyzeNamedType(SymbolAnalysisContext context)
    {
        // Must be a class
        if (context.Symbol is not INamedTypeSymbol typeSymbol || typeSymbol.TypeKind != TypeKind.Class)
        {
            return;
        }

        // Must derive from Error
        var baseType = typeSymbol.BaseType;
        if (baseType == null)
        {
            return;
        }

        if (!DerivesFromError(baseType))
        {
            return;
        }

        // Get all public instance constructors
        var ctors = typeSymbol.InstanceConstructors
            .Where(c => c.DeclaredAccessibility == Accessibility.Public)
            .ToArray();

        // Must have exactly one public constructor
        if (ctors.Length != 1)
        {
            Report(context, typeSymbol);
            return;
        }

        var ctor = ctors[0];
        var parameters = ctor.Parameters;

        // Must have exactly one parameter
        if (parameters.Length != 1)
        {
            Report(context, typeSymbol);
            return;
        }

        var param = parameters[0];

        // Must be string
        if (param.Type.SpecialType != SpecialType.System_String)
        {
            Report(context, typeSymbol);
            return;
        }

        // MS Copilot originally put this in but I can see some crazy developer down the line deciding that
        // the 'description' parameter should contain JSON, Markdown, or a Base64‑encoded goat.

        // Must be named "description"
        //if (!String.Equals(param.Name, "description", StringComparison.Ordinal))
        //{
        //    Report(context, typeSymbol);
        //    return;
        //}
    }

    private static Boolean DerivesFromError(INamedTypeSymbol? type)
    {
        while (type != null)
        {
            if (type.Name == "Error" && type.ContainingNamespace.ToDisplayString().EndsWith(Consts.PatternNamespace))
            {
                return true;
            }

            type = type.BaseType;
        }

        return false;
    }

    private static void Report(SymbolAnalysisContext context, INamedTypeSymbol typeSymbol)
    {
        var diagnostic = Diagnostic.Create(
            DiagnosticDescriptors.DerivedErrorTypesMustHaveSingleStringConstructor,
            typeSymbol.Locations.FirstOrDefault(),
            typeSymbol.Name);

        context.ReportDiagnostic(diagnostic);
    }
}
