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
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace ProvisionData.ResultPattern.Generators;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ResultPatternConfigurationAnalyzer : DiagnosticAnalyzer
{

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [ResultPatternDiagnosticDefinitions.Missing_AddResultPattern_Invocation];

    public override void Initialize(AnalysisContext context)
    {
        //if (!Debugger.IsAttached)
        //{
        //    Debugger.Launch();
        //}

        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterCompilationStartAction(Start);
    }

    private static void Start(CompilationStartAnalysisContext context)
    {
        var usedResultPatternTypes = false;
        var addResultPatternCalled = false;

        context.RegisterSyntaxNodeAction(syntaxContext =>
        {
            var invocation = (InvocationExpressionSyntax)syntaxContext.Node;
            if (syntaxContext.SemanticModel.GetSymbolInfo(invocation).Symbol is not IMethodSymbol symbol)
            {
                return;
            }

            if (symbol.Name == "AddResultPattern" &&
                symbol.ContainingType?.Name == "ResultPatternServiceCollectionExtensions")
            {
                addResultPatternCalled = true;
            }
        }, SyntaxKind.InvocationExpression);

        context.RegisterSyntaxNodeAction(syntaxContext =>
        {
            if (usedResultPatternTypes)
            {
                return;
            }

            var identifier = (IdentifierNameSyntax)syntaxContext.Node;
            var symbol = syntaxContext.SemanticModel.GetSymbolInfo(identifier).Symbol;
            if (symbol is ITypeSymbol typeSymbol)
            {
                if (typeSymbol.ContainingNamespace?.ToDisplayString().StartsWith(GeneratorConsts.ResultPatternNamespace) is true)
                {
                    usedResultPatternTypes = true;
                }
            }
        }, SyntaxKind.IdentifierName);

        context.RegisterCompilationEndAction(endContext =>
        {
            if (usedResultPatternTypes && !addResultPatternCalled)
            {
                // Report once on the assembly
                var location = Location.None;
                var diagnostic = Diagnostic.Create(ResultPatternDiagnosticDefinitions.Missing_AddResultPattern_Invocation, location);
                endContext.ReportDiagnostic(diagnostic);
            }
        });
    }
}
